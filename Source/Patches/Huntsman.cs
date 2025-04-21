using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System;
using Photon.Pun;

namespace Ardot.REPO.EnemyOverhaul;

public static class HuntsmanOverhaul
{
    public static ConfigEntry<bool> 
        OverhaulAI,
        OverhaulDamageFalloff,
        OverhaulTargetPlayerItems;

    public static void Init()
    {
        OverhaulAI = Plugin.BindConfig(
            "Huntsman",
            "OverhaulAI",
            true,
            "If true, Huntsman AI is overhauled",
            () => {
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyHunter), "Awake"),
                    postfix: new HarmonyMethod(typeof(HuntsmanOverhaul), nameof(AwakePostfix))
                );
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyHunter), nameof(EnemyHunter.OnInvestigate)),
                    prefix: new HarmonyMethod(typeof(HuntsmanOverhaul), nameof(OnInvestigatePrefix))
                );
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyHunter), "StateAim"),
                    prefix: new HarmonyMethod(typeof(HuntsmanOverhaul), nameof(StateAimPrefix))
                );
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyHunter), "Update"),
                    postfix: new HarmonyMethod(typeof(HuntsmanOverhaul), nameof(UpdatePostfix))
                );
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyHunter), "UpdateState"),
                    prefix: new HarmonyMethod(typeof(HuntsmanOverhaul), nameof(UpdateStatePrefix))
                );
            }
        );
        OverhaulDamageFalloff = Plugin.BindConfig(
            "Huntsman",
            "OverhaulDamageFalloff",
            true,
            "If true, Huntsman damage reduces over long distances and increases at short range",
            () => Plugin.SetPatch(
                OverhaulDamageFalloff.Value,
                AccessTools.Method(typeof(EnemyHunter), "ShootRPC"), 
                postfix: new HarmonyMethod(typeof(HuntsmanOverhaul), "HunterShootRPCPostFix")
            )
        );
        OverhaulTargetPlayerItems = Plugin.BindConfig(
            "Huntsman",
            "OverhaulTargetPlayerItems",
            true,
            "If true, Huntsman shoot directly at valuables that touch it, not the players holding those valuables.",
            () => {
                Plugin.SetPatch(
                    OverhaulTargetPlayerItems.Value,
                    AccessTools.Method(typeof(EnemyHunter), "OnTouchPlayerGrabbedObject"), 
                    postfix: new HarmonyMethod(typeof(HuntsmanOverhaul), "HunterOnTouchPlayerGrabbedObjectPostfix"));
                Plugin.SetPatch(
                    OverhaulTargetPlayerItems.Value,
                    AccessTools.Method(typeof(EnemyRigidbody), "OnCollisionStay"), 
                    postfix: new HarmonyMethod(typeof(HuntsmanOverhaul), "EnemyOnCollisionStayPostfix"));
            }
        );
    }

    public static void AwakePostfix(EnemyHunter __instance)
    {
        HuntsmanOverhaulState state = __instance.gameObject.AddComponent<HuntsmanOverhaulState>();
        state.Hunter = __instance;
    }

    public static void UpdateStatePrefix(EnemyHunter __instance, EnemyHunter.State _state)
    {
        HuntsmanOverhaulState state = __instance.GetComponent<HuntsmanOverhaulState>();
        if(_state != EnemyHunter.State.Investigate && __instance.currentState != EnemyHunter.State.Investigate)
            state.InvestigateFrames = 0;
    }

    public static void UpdatePostfix(EnemyHunter __instance)
    {
        HuntsmanOverhaulState state = __instance.GetComponent<HuntsmanOverhaulState>();
        state.InvestigateFrames -= 1;
    }

    public static void OnInvestigatePrefix(EnemyHunter __instance)
    {
        HuntsmanOverhaulState state = __instance.GetComponent<HuntsmanOverhaulState>();
        if (SemiFunc.IsMasterClientOrSingleplayer() && (float)state.EnemyRigidbody.Get("timeSinceStun") > 1.5f)
		{
            state.InvestigateFrames = 2;

            if(__instance.currentState == EnemyHunter.State.Aim && state.InvestigateCooldown <= 0f)
            {
                state.InvestigateTriggeredImpulse = true;
                Vector3 investigatePoint = (Vector3)state.StateInvestigate.Get("onInvestigateTriggeredPosition");
                __instance.Set("investigatePoint", investigatePoint);
                __instance.Set("shootFast", Vector3.Distance(investigatePoint, state.transform.position) < 4f);
                state.InvestigateTransformGet();

                if (SemiFunc.IsMultiplayer())
                    state.PhotonView.RPC("UpdateInvestigationPoint", RpcTarget.Others, investigatePoint);
            }
        }
    }

    public static bool StateAimPrefix(EnemyHunter __instance)
    {
        HuntsmanOverhaulState state = __instance.GetComponent<HuntsmanOverhaulState>();

        if(state.InvestigateFrames <= 0)
            return true;

        float stateTimer = (float)__instance.Get("stateTimer");
        state.InvestigateFrames = 2;

        if((bool)__instance.Get("stateImpulse"))
        {
            state.EnemyNavAgent.Warp(state.EnemyRigidbody.transform.position);
			state.EnemyNavAgent.ResetPath();
            __instance.Set("stateImpulse", false);
            stateTimer = 5f;
            state.InvestigateCooldown = 1f;
            __instance.Set("investigatePointTransform", null);
            __instance.Set("investigatePointHasTransform", false);
        }

        if(state.InvestigateTriggeredImpulse && !state.Aiming)
        {
            state.InvestigateTriggeredImpulse = false;
            state.Aiming = true;
            state.InvestigateCooldown = 0.5f;
            stateTimer = 0.5f;
            __instance.Set("investigatePointSpread", Vector3.zero);
			__instance.Set("investigatePointSpreadTarget", Vector3.zero);
			__instance.Set("investigatePointSpreadTimer", 0f);
        }

        stateTimer -= Time.deltaTime;

        if(stateTimer <= 0)
        {
            if(state.Aiming)
            {
                state.Aiming = false;
                state.UpdateState(EnemyHunter.State.Shoot);
            }
            else
            {
                state.Animator.CrossFade("Leave Start", 1f);
                state.UpdateState(EnemyHunter.State.LeaveStart);
            }
        }

        __instance.Set("stateTimer", stateTimer);
        return false;
    }

    public static void HunterOnTouchPlayerGrabbedObjectPostfix(EnemyHunter __instance)
    {
        // Makes the huntsman follow the object hitting it rather than the player

        AccessTools.Field(typeof(EnemyHunter), "investigatePointHasTransform").SetValue(__instance, false);
    }

    public static void HunterShootRPCPostFix(EnemyHunter __instance, Vector3 _hitPosition)
    {
        // Adds falloff to the hunter's shotgun

        float falloff = Mathf.Min(2f / Mathf.Pow(Vector3.Distance(_hitPosition, __instance.gunTipTransform.position), 3f/2f), 1f/2f);
        __instance.hurtCollider.playerDamage = (int)(300f * falloff);
        __instance.hurtCollider.playerTumbleForce = 30f * falloff;
        __instance.hurtCollider.playerTumbleTorque = 30f * falloff;
        __instance.hurtCollider.playerHitForce = 30f * falloff;
        __instance.hurtCollider.playerTumbleTime = 6 * falloff;
        __instance.hurtCollider.enemyDamage = (int)(300f * falloff);
        __instance.hurtCollider.enemyHitForce = 30f * falloff;
        __instance.hurtCollider.enemyHitTorque = 60f * falloff;
    }

    public static void EnemyOnCollisionStayPostfix(EnemyRigidbody __instance, Collision other)
    {
        // Makes enemies target objects hitting them rather than the player holding the object

        if (other.gameObject.CompareTag("Phys Grab Object"))
        {
            PhysGrabObject physGrabObject = other.gameObject.GetComponent<PhysGrabObject>();
            if (!physGrabObject)
                physGrabObject = other.gameObject.GetComponentInParent<PhysGrabObject>();

            if(physGrabObject && !__instance.enemy.CheckChase() && physGrabObject.playerGrabbing.Count > 0)
                __instance.Set("onTouchPlayerGrabbedObjectPosition", other.transform.position);
        }
    }
}

public class HuntsmanOverhaulState : MonoBehaviour
{
    public EnemyHunter Hunter;
    public EnemyHunterAnim EnemyAnim;
    public Animator Animator;
    public Enemy Enemy;
    public EnemyNavMeshAgent EnemyNavAgent;
    public EnemyRigidbody EnemyRigidbody;
    public EnemyStateInvestigate StateInvestigate;
    public PhotonView PhotonView;
    public Action<EnemyHunter.State> UpdateState;
    public Action InvestigateTransformGet;
    public bool InvestigateTriggeredImpulse;
    public float InvestigateCooldown;
    public bool Aiming;
    public int InvestigateFrames;
    
    public void Awake()
    {
        EnemyNavAgent = GetComponent<EnemyNavMeshAgent>();
        PhotonView = GetComponent<PhotonView>();
        StateInvestigate = GetComponent<EnemyStateInvestigate>();
        Enemy = GetComponent<Enemy>();
    }

    public void Start()
    {
        UpdateState = (Action<EnemyHunter.State>)AccessTools.Method(typeof(EnemyHunter), "UpdateState").CreateDelegate(typeof(Action<EnemyHunter.State>), Hunter);
        InvestigateTransformGet = (Action)AccessTools.Method(typeof(EnemyHunter), "InvestigateTransformGet").CreateDelegate(typeof(Action), Hunter);
        EnemyAnim = Hunter.enemyHunterAnim;
        Animator = (Animator)EnemyAnim.Get("animator");
        EnemyRigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
    }

    public void Update()
    {
        InvestigateCooldown -= Time.deltaTime;
    }
}