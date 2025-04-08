using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;

namespace Ardot.REPO.REPOverhaul;

public static class RobePatches
{
    public static void Patch ()
    {
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobe), "Awake"),
            prefix: new HarmonyMethod(typeof(RobePatches), "AwakePrefix")
        );  
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobePersistent), "Update"),
            prefix: new HarmonyMethod(typeof(RobePatches), "PersistentUpdatePrefix")
        );
    }

    public static bool AwakePrefix(EnemyRobe __instance)
    {
        GameObject.Destroy(__instance.robeAnim);
        RobeOverride robeOverride = __instance.gameObject.AddComponent<RobeOverride>();
        robeOverride.RotationSpring = __instance.rotationSpring;
        robeOverride.RobeAnim = __instance.robeAnim;
        GameObject.Destroy(__instance);

        return false;
    }

    public static bool PersistentUpdatePrefix()
    {
        return false;
    }
}

public class RobeOverride : MonoBehaviour
{
    public enum RobeState
    {
        Idle,
        Roam,
        FollowPlayer,
        HelpPlayer,
        ChaseBegin,
        Chase,
        Attack,
        AttackUnder,
        GiveSpace,
    }

    public RobeState State = RobeState.Idle;
    public float StateTimer = 0f;
    public float StateInternalTimer = 0f;
    public float LastFocusedPlayerVisionTimer = float.PositiveInfinity;
    public bool VisionImpulse = false;
    public bool TouchedImpulse = false;
    public bool StateImpulse = true;
    public bool StateEndedImpulse = false;
    public Enemy Enemy;
    public PhotonView PhotonView;
    public PlayerAvatar FocusedPlayer;
    public PlayerAvatar TouchedPlayer;
    public SpringQuaternion RotationSpring;
    public EnemyNavMeshAgent EnemyAgent;
    public EnemyVision Vision;
    public EnemyRigidbody Rigidbody;
    public EnemyRobeAnim RobeAnim;
    public Animator Animator;
    public HurtCollider HurtCollider;
    public ValuableObject TargetValuable;
    public Quaternion RotationTarget;

    public bool ConsumeStateImpulse()
    {
        bool started = StateImpulse;
        StateImpulse = false;
        return started;
    }

    public void SetState(RobeState state, float stateTimer)
    {
        if (state == State || !SemiFunc.IsMasterClientOrSingleplayer())
            return;

        StateImpulse = true;
        StateEndedImpulse = true;
        State = state;
        StateTimer = stateTimer;
        Plugin.Logger.LogInfo($"{State}, {StateTimer}");

        if(GameManager.Multiplayer())
            PhotonView.RPC("SetStateRPC", RpcTarget.Others, state, stateTimer);
    }

    [PunRPC]
    private void SetStateRPC(RobeState state, float stateTimer)
    {
        StateImpulse = true;
        State = state;
        StateTimer = stateTimer;
    }

    public void Awake()
    {
        Enemy = GetComponent<Enemy>();
        PhotonView = GetComponent<PhotonView>();
    }

    public void Start()
    {
        HurtCollider = Utils.GetHurtColliders(transform.parent)[0];
        Animator = (Animator)RobeAnim.Get("animator");
        Rigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        EnemyAgent = (EnemyNavMeshAgent)Enemy.Get("NavMeshAgent");
        Vision = (EnemyVision)Enemy.Get("Vision");
        Vision.onVisionTriggered.AddListener(() => VisionImpulse = true);
        Rigidbody.onGrabbed.AddListener(() =>{
            TouchedImpulse = true;
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onGrabbedPlayerAvatar");
        });
        Rigidbody.onTouchPlayer.AddListener(() => {
            TouchedImpulse = true;
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onTouchPlayerAvatar");
        });
        Rigidbody.onTouchPlayerGrabbedObject.AddListener(() => {
            TouchedImpulse = true;
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onTouchPlayerGrabbedObjectAvatar");
        });
    }

    public void Update()
    {
        StateTimer -= Time.deltaTime;
        StateInternalTimer -= Time.deltaTime;
        LastFocusedPlayerVisionTimer += Time.deltaTime;

        if(VisionImpulse && (PlayerAvatar)Vision.Get("onVisionTriggeredPlayer") == FocusedPlayer || TouchedImpulse && TouchedPlayer == FocusedPlayer)
            LastFocusedPlayerVisionTimer = 0;

        switch (State)
        {
            case RobeState.Idle:
            {
                if(ConsumeStateImpulse())
                    EnemyAgent.ResetPath();

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(CheckTouchLogic()) {}
                else if(StateTimer <= 0)
                {
                    if(FocusedPlayer != null)
                    {
                        float playerDistance = Vector3.Distance(transform.position, FocusedPlayer.transform.position);
                        PhysGrabObject playerGrabbedObject = (PhysGrabObject)FocusedPlayer.physGrabber.Get("grabbedPhysGrabObject");
                        if(playerDistance > 3.5f || playerDistance < 2.5f)
                            SetState(RobeState.FollowPlayer, Random.Range(2f, 4f));
                        else if (playerGrabbedObject != null && (TargetValuable = playerGrabbedObject.GetComponent<ValuableObject>()) != null)
                            SetState(RobeState.HelpPlayer, 1f);
                        else
                            SetState(RobeState.Roam, Random.Range(0.25f, 1f));
                    }
                    else
                        SetState(RobeState.Roam, Random.Range(4f, 9f));
                }

                break;
            }
            case RobeState.Roam:
            {
                if(ConsumeStateImpulse())
                {
                    LevelPoint levelPoint = SemiFunc.LevelPointGet(transform.position, 5f, 15f) ?? SemiFunc.LevelPointGet(transform.position, 0f, 999f);

                    if(levelPoint != null && Utils.FindNavPosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                NormalRotationLogic();

                if(CheckTouchLogic()) {}
                else if(VisionImpulse)
                {
                    LastFocusedPlayerVisionTimer = 0f;
                    FocusedPlayer = (PlayerAvatar)Vision.Get("onVisionTriggeredPlayer");
                    SetState(RobeState.FollowPlayer, 5f);
                }
                else if(!EnemyAgent.HasPath() || StateTimer <= 0)
                    SetState(RobeState.Idle, Random.Range(1f, 2f));

                break;
            }
            case RobeState.FollowPlayer:
            {
                if(ConsumeStateImpulse())
                {
                    if(Utils.FindNavPosition(FocusedPlayer.transform.position + (transform.position - FocusedPlayer.transform.position).normalized * 3 + Random.insideUnitSphere * 0.5f, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                NormalRotationLogic();

                if(CheckTouchLogic()) {}
                else if(LastFocusedPlayerVisionTimer > 6f)
                {
                    FocusedPlayer = null;
                    SetState(RobeState.Idle, Random.Range(0.25f, 0.5f));
                }
                else if(!EnemyAgent.HasPath() || StateTimer <= 0)
                    SetState(RobeState.Idle, Random.Range(0.25f, 0.5f));

                break;
            }
            case RobeState.HelpPlayer:
            {
                

                break;
            }
            case RobeState.ChaseBegin:
            {
                if(ConsumeStateImpulse())
                {
                    HurtCollider.enabled = false;
                    EnemyAgent.ResetPath();
                    AttackAnimation();
                }
                
                LookAt(FocusedPlayer.transform.position);
                    
                if(StateTimer <= 0)
                    SetState(RobeState.Chase, Random.Range(6f, 10f));

                if(StateEndedImpulse)
                    HurtCollider.enabled = true;

                break;
            }
            case RobeState.Chase:
            {
                if(ConsumeStateImpulse())
                {
                    RobeAnim.sfxTargetPlayerLoop.PlayLoop(true, 2f, 2f, 1f);
                    OverrideMovement(4, 10, StateTimer);
                    StateInternalTimer = 0;
                }

                NormalRotationLogic();

                if(StateInternalTimer <= 0)
                {
                    StateInternalTimer = 0.1f;
                    if(Utils.FindNavPosition(FocusedPlayer.transform.position, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                }

                if(LastFocusedPlayerVisionTimer > 6f)
                { 
                    FocusedPlayer = null;
                    SetState(RobeState.GiveSpace, 2f);
                }
                else if(Vector3.Distance(transform.position, FocusedPlayer.transform.position) < 1f)
                {
                    if((bool)FocusedPlayer.Get("isCrawling"))
                        SetState(RobeState.AttackUnder, 0.5f);
                    else
                        SetState(RobeState.Attack, 0.5f);
                }
                else if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                if(StateEndedImpulse)
                {
                    EndMovementOverride();
                    RobeAnim.sfxTargetPlayerLoop.PlayLoop(false, 2f, 2f, 1f);
                }

                break;
            }
            case RobeState.Attack:
            {
                if(ConsumeStateImpulse())
                    AttackAnimation();

                LookAt(FocusedPlayer.transform.position);

                if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                break;
            }
            case RobeState.AttackUnder:
            {
                if(ConsumeStateImpulse())
                    AttackUnderAnimation();

                LookAt(FocusedPlayer.transform.position);

                if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                break;
            }
            case RobeState.GiveSpace:
            {
                if(ConsumeStateImpulse())
                {
                    if(Utils.FindNavPosition(EnemyAgent.transform.position + Random.onUnitSphere * 4, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                NormalRotationLogic();

                if(StateTimer <= 0)
                    SetState(RobeState.Idle, 2f);

                break;
            }
        }

		transform.rotation = SemiFunc.SpringQuaternionGet(RotationSpring, RotationTarget, -1f);
        
        VisionImpulse = false; 
        TouchedImpulse = false;
        StateEndedImpulse = false;
    }

    public bool CheckTouchLogic()
    {
        if(!TouchedImpulse)
            return false;

        FocusedPlayer = TouchedPlayer;
        LastFocusedPlayerVisionTimer = 0;
        SetState(RobeState.ChaseBegin, 1f);
        return true;
    }

    public void NormalRotationLogic()
    {
        Vector3 agentVelocity = EnemyAgent.Get<Vector3, EnemyNavMeshAgent>("AgentVelocity").normalized;
        if (agentVelocity.magnitude > 0.1f)
            LookInDirection(agentVelocity);
    }
    
    public void LookAt(Vector3 point)
    {
        LookInDirection((point - transform.position).normalized);
    }

    public void LookInDirection(Vector3 point)
    {
        RotationTarget = Quaternion.LookRotation(point);
        RotationTarget.eulerAngles = new Vector3(0f, RotationTarget.eulerAngles.y, 0f);
    }

    public void AttackSounds()
    {
        GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, transform.position, 0.5f);
        GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, transform.position, 0.1f);
        RobeAnim.sfxAttack.Play(transform.position, 1f, 1f, 1f, 1f);
        RobeAnim.sfxAttackGlobal.Play(transform.position, 1f, 1f, 1f, 1f);
    }

    public void AttackAnimation()
    {
        AttackSounds();
        Animator.SetTrigger("attack");
    }

    public void AttackUnderAnimation()
    {
        AttackSounds();
        Animator.SetTrigger("LookUnder");
    }

    public void OverrideMovement(float speed, float acceleration, float time)
    {
        EnemyAgent.OverrideAgent(speed, acceleration, time);
        Rigidbody.OverrideFollowPosition(time, speed);
    }

    public void EndMovementOverride()
    {
        EnemyAgent.OverrideAgent(0, 0, 0.001f);
        Rigidbody.OverrideFollowPosition(0.001f, 0);
    }

    // public List<ValuableObject> GetNearbyValuables(float undiscoveredRange, float detectedRange, float maxMass)
    // {
    //     List<ValuableObject> valuables = new ();

    //     for(int x = 0; x < ValuableDirector.instance.valuableList.Count; x++)
    //     {
    //         ValuableObject valuable = ValuableDirector.instance.valuableList[x];
    //         float distance = Vector3.Distance(valuable.transform.position, transform.position);
    //         float mass = (float)valuable.Get("rigidBodyMass");
    //         bool discovered = (bool)valuable.Get("discovered");

    //         if(mass < maxMass &&  && (distance < undiscoveredRange  || distance < detectedRange && discovered))
    //             valuables.Add(valuable);
    //     }

    //     return valuables;
    // }
}