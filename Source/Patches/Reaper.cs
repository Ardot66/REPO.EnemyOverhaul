using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using Photon.Pun;

namespace Ardot.REPO.EnemyOverhaul;

public static class ReaperOverhaul
{
    public static ConfigEntry<bool> OverhaulAI;
    
    public static void Init()
    {
        OverhaulAI = Plugin.BindConfig(
            "Reaper",
            "OverhaulAI",
            true,
            "If true, overhauls the AI of the Reaper",
            () => {
                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyRunner), nameof(EnemyRunner.StateAttackPlayer)),
                    prefix: new HarmonyMethod(typeof(ReaperOverhaul), nameof(StateAttackPlayerPrefix))
                );

                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyRunner), "Awake"),
                    postfix: new HarmonyMethod(typeof(ReaperOverhaul), nameof(AwakePostfix))
                );

                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyRunner), nameof(EnemyRunner.OnHurt)),
                    postfix: new HarmonyMethod(typeof(ReaperOverhaul), nameof(OnHurtPostfix))
                );

                Plugin.SetPatch(
                    OverhaulAI.Value,
                    AccessTools.Method(typeof(EnemyRunner), nameof(EnemyRunner.StateLeave)),
                    postfix: new HarmonyMethod(typeof(ReaperOverhaul), nameof(StateLeavePostfix))
                );
            }
        );
    }

    public static void StateLeavePostfix(EnemyRunner __instance)
    {
        ReaperOverhaulState state = __instance.GetComponent<ReaperOverhaulState>();
        if(state.RunningTimer > 0f)
            state.OverrideMovement(3f, 5f, 6f, 0.2f);
    }

    public static void AwakePostfix(EnemyRunner __instance)
    {
        ReaperOverhaulState state = __instance.gameObject.AddComponent<ReaperOverhaulState>();
        state.Reaper = __instance;
        state.Enemy = __instance.GetComponent<Enemy>();
        state.UpdateState = (Action<EnemyRunner.State>)AccessTools.Method(typeof(EnemyRunner), "UpdateState").CreateDelegate(typeof(Action<EnemyRunner.State>), __instance);
        state.VisionBlocked = (Func<bool>)AccessTools.Method(typeof(EnemyRunner), "VisionBlocked").CreateDelegate(typeof(Func<bool>), __instance);
    }

    public static void OnHurtPostfix(EnemyRunner __instance)
    {
        switch(__instance.currentState)
        {
            case EnemyRunner.State.AttackPlayer:
            case EnemyRunner.State.AttackPlayerOver:
            case EnemyRunner.State.AttackPlayerBackToNavMesh:
            {
                ReaperOverhaulState state = __instance.GetComponent<ReaperOverhaulState>();
                int damage = state.PrevHealth - (int)state.EnemyHealth.Get("healthCurrent"); 
                state.Aggro -= damage;

                if(state.Aggro <= 0)
                {
                    state.UpdateState(EnemyRunner.State.Leave);
                    state.HurtAggroCooldown = 3f;
                    state.RunningTimer = 5f;
                    state.EnemyAnim.sfxJump.Play(__instance.transform.position, 1.5f);
                }    
                
                break;
            }
        }
    }

    public static bool StateAttackPlayerPrefix(EnemyRunner __instance)
    {
        ReaperOverhaulState state = __instance.GetComponent<ReaperOverhaulState>();

        if(state.HurtAggroCooldown > 0)
        {
            state.UpdateState(EnemyRunner.State.Leave);
            return false;
        }

        PlayerAvatar targetPlayer = (PlayerAvatar)__instance.Get("targetPlayer");
        bool stateImpulse = (bool)__instance.Get("stateImpulse");
        float stateTimer = (float)__instance.Get("stateTimer");

        if(stateImpulse)
        {
            __instance.Set("stateImpulse", false);
            __instance.Set("stateTimer", 2f);
            stateTimer = 2f;
            state.RetargetTimer = 0f;
            state.ChaseTime = 0f;
        }

        state.Aggro += Time.deltaTime;
        state.ChaseTime += Time.deltaTime;
        state.RetargetTimer -= Time.deltaTime;

        if(state.RetargetTimer <= 0f)
        {
            state.RetargetTimer = 0.2f;

            if((bool)targetPlayer.tumble.Get("isTumbling"))
                state.SetPlayerDamage((int)state.ChaseTime);
            else
                state.SetPlayerDamage((int)state.ChaseTime + 10);

            float speed = 1f + state.ChaseTime / 5;
            
            state.OverrideMovement(speed, speed * 3, speed * 3, 0.25f);
            state.EnemyNavAgent.SetDestination(targetPlayer.transform.position);
        }

        if (!state.EnemyNavAgent.CanReach(targetPlayer.transform.position, 0.25f) && Vector3.Distance(state.EnemyRigidbody.transform.position, state.EnemyNavAgent.GetPoint()) < 2f)
		{
			if (targetPlayer.transform.position.y > state.EnemyRigidbody.transform.position.y - 0.8f)
				state.EnemyJump.StuckTrigger(targetPlayer.transform.position - state.EnemyVision.VisionTransform.position);
		}
		if (!(bool)state.EnemyJump.Get("jumping") && (float)state.EnemyRigidbody.Get("notMovingTimer") > 2f)
			state.EnemyJump.StuckTrigger(targetPlayer.transform.position - state.FeetTransform.position);
		if (
            stateTimer > 1.5f && 
            (bool)targetPlayer.Get("isCrawling") && 
            !(bool)targetPlayer.Get("isTumbling") && 
            (double)Vector3.Distance(state.EnemyNavAgent.GetPoint(), targetPlayer.transform.position) > 0.5 &&
            Vector3.Distance(targetPlayer.transform.position, (Vector3)targetPlayer.Get("LastNavmeshPosition")) < 2f)
		{
            state.SetPlayerDamage(20);
			state.UpdateState(EnemyRunner.State.LookUnderStart);
			return false;
		}

        if(stateTimer <= 0)
        {
            state.UpdateState(EnemyRunner.State.Idle);
        }

        return false;
    }
}

public class ReaperOverhaulState : MonoBehaviour
{
    public Action<EnemyRunner.State> UpdateState;
    public Func<bool> VisionBlocked;
    public EnemyRunner Reaper;
    public EnemyRunnerAnim EnemyAnim;
    public Enemy Enemy;
    public Transform FeetTransform; 
    public PhotonView PhotonView;
    public EnemyHealth EnemyHealth;
    public EnemyVision EnemyVision;
    public EnemyJump EnemyJump;
    public EnemyNavMeshAgent EnemyNavAgent;
    public EnemyRigidbody EnemyRigidbody;
    public float RetargetTimer = 0f;
    public float ChaseTime = 0f;
    public float HurtAggroCooldown = 0f;
    public float Aggro = 0f;
    public float RunningTimer = 0f;
    public int PrevHealth = 0;
    public int PlayerDamage = 0;

    public void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    public void Start()
    {
        FeetTransform = Reaper.feetTransform;
        EnemyAnim = Reaper.animator;
        EnemyVision = (EnemyVision)Enemy.Get("Vision");
        EnemyJump = (EnemyJump)Enemy.Get("Jump");
        EnemyNavAgent = (EnemyNavMeshAgent)Enemy.Get("NavMeshAgent");
        EnemyRigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        EnemyHealth = (EnemyHealth)Enemy.Get("Health");
        Reaper.hurtCollider.physHitForce = 3f;
        Reaper.hurtCollider.physHitTorque = 10f;
        Reaper.hurtCollider.playerTumbleForce = 2f;
    }

    public void Update()
    {
        PrevHealth = (int)EnemyHealth.Get("healthCurrent");
        HurtAggroCooldown -= Time.deltaTime;
        RunningTimer -= Time.deltaTime;
        Reaper.hurtCollider.playerDamage = PlayerDamage;
    }

    public void OverrideMovement(float speed, float acceleration, float rigidbodyAcceleration, float time)
    {
        EnemyNavAgent.OverrideAgent(speed, acceleration, time);
        EnemyRigidbody.OverrideFollowPosition(time, rigidbodyAcceleration);
    }

    [PunRPC]
    public void SetPlayerDamage(int damage)
    {
        if(SemiFunc.IsMasterClient())
            PhotonView.RPC(nameof(SetPlayerDamage), RpcTarget.Others, damage);
            
        PlayerDamage = damage;
    }
}