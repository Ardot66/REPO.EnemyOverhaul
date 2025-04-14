using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;
using HarmonyLib;

namespace Ardot.REPO.REPOverhaul;

public static class TrudgePatches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemySlowWalker), "Start"),
            prefix: new HarmonyMethod(typeof(TrudgePatches), "StartPrefix")
        );
    }

    public static bool StartPrefix(EnemySlowWalker __instance)
    {
        TrudgeOverride trudge = __instance.gameObject.AddComponent<TrudgeOverride>();
        EnemySlowWalkerAnim trudgeAnim = __instance.animator;

        trudgeAnim.enabled = false;

        trudge.LookAtTransform = __instance.lookAtTransform;
        trudge.EnemyAnimator = trudgeAnim;
        trudge.RotationSpring = __instance.horizontalRotationSpring;
        GameObject.Destroy(__instance);

        return false;
    }
}

public class TrudgeOverride : MonoBehaviour
{

    public enum TrudgeState
    {
        Invalid,
        Spawn,
        Idle,
        ApproachTarget,
        LookAroundBegin,
        LookAround,
        Notice,
        Charge,
        Attack,
        AttackTarget,
    }
    
    public PhotonView PhotonView;
    public Enemy Enemy;
    public EnemyVision Vision;
    public EnemyNavMeshAgent EnemyNavAgent;
    public NavMeshAgent NavAgent;
    public EnemyRigidbody Rigidbody;
    public EnemySlowWalkerAnim EnemyAnimator;
    public Animator Animator;
    public SpringQuaternion RotationSpring;
    public Transform LookAtTransform;
    public MonoBehaviour Target;

    public record struct StateTimerOverride(TrudgeState State, float Time);
    public Stack<StateTimerOverride> StateTimerOverrides = new ();

    public TrudgeState State = TrudgeState.Idle;
    public TrudgeState FutureState = TrudgeState.Invalid;

    public Quaternion RotationTarget;

    public float RotationSpeed = 0.5f;

    public float StateTimer = 0f;
    public float StateInternalTimer1 = 0f;
    public float StateInternalTimer2 = 0f;

    public bool StateBeginImpulse = false;
    public bool StateEndImpulse = false;
    public bool VisionImpulse = false;
    public bool TouchImpulse = false;

    public PlayerAvatar TouchedPlayer;

    public int AnimMoving = Animator.StringToHash("moving");
	public int AnimStunned = Animator.StringToHash("stunned");
	public int AnimDespawning = Animator.StringToHash("despawning");
	public int AnimFalling = Animator.StringToHash("falling");
	public int AnimLookingUnder = Animator.StringToHash("lookingUnder");
	public int AnimStun = Animator.StringToHash("Stun");
	public int AnimNotice = Animator.StringToHash("Notice");
	public int AnimAttack = Animator.StringToHash("Attack");
	public int AnimJump = Animator.StringToHash("Jump");
	public int AnimLand = Animator.StringToHash("Land");
	public int AnimLookUnder = Animator.StringToHash("LookUnder");
	public int AnimLookUnderAttack = Animator.StringToHash("LookUnderAttack");
	public int AnimStuckAttack = Animator.StringToHash("StuckAttack");

    // public Sound SfxFootstepSmall;
	// public Sound SfxFootstepBig;
	// public Sound SfxJump;
	// public Sound SfxLand;
	// public Sound SfxMoveShort;
	// public Sound SfxMoveLong;
	// public Sound SfxAttackBuildupVoice;
	// public Sound SfxAttackImpact;
	// public Sound SfxAttackImplosionBuildup;
	// public Sound SfxAttackImplosionHitLocal;
	// public Sound SfxAttackImplosionHitGlobal;
	// public Sound SfxAttackImplosionImpactLocal;
	// public Sound SfxAttackImplosionImpactGlobal;
	// public Sound SfxDeath;
	// public Sound SfxHurt;
	// public Sound SfxNoiseShort;
	// public Sound SfxNoiseLong;
	// public Sound SfxNoticeVoice;
	// public Sound SfxSwingShort;
	// public Sound SfxSwingLong;
	// public Sound SfxMaceTrailing;
	// public Sound SfxLookUnderIntro;
	// public Sound SfxLookUnderAttack;
	// public Sound SfxLookUnderOutro;
	// public Sound SfxStunnedLoop;
	public SlowWalkerAttack SlowWalkerAttack;
	public SlowWalkerJumpEffect SlowWalkerJumpEffect;

    public float SpringSpeedMultiplier = 1f;
    public float SpringDampingMultiplier = 1f;

    public void Awake()
    {
        Enemy = GetComponent<Enemy>();
        PhotonView = GetComponent<PhotonView>();
        EnemyNavAgent = GetComponent<EnemyNavMeshAgent>(); 
        NavAgent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        Vision = (EnemyVision)Enemy.Get("Vision");
        Rigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        Animator = (Animator)EnemyAnimator.Get("animator");

        Vision.onVisionTriggered.AddListener(() => VisionImpulse = true);
        Rigidbody.onGrabbed.AddListener(() => {
            TouchImpulse = true; 
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onGrabbedPlayerAvatar");
        });
        Rigidbody.onTouchPlayer.AddListener(() => {
            TouchImpulse = true; 
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onTouchPlayerAvatar");
        });
        Rigidbody.onTouchPlayerGrabbedObject.AddListener(() => {
            TouchImpulse = true; 
            TouchedPlayer = (PlayerAvatar)Rigidbody.Get("onTouchPlayerGrabbedObjectAvatar");
        });
    }

    public void Update()
    {
        StateTimer -= Time.deltaTime;
        StateInternalTimer1 -= Time.deltaTime;
        StateInternalTimer2 -= Time.deltaTime;

        if(FutureState != TrudgeState.Invalid)
            StateEndImpulse = true;

        if(Utils.IsHost() && (OverhaulDirector.Instance.ExtractionCompletedImpulse || OverhaulDirector.Instance.ExtractionUnlockedImpulse))
            SetState(TrudgeState.Idle, 0f);
        
        HeadLookAt();

        switch(State)
        {
            case TrudgeState.Spawn:
            {


                break;
            }
            case TrudgeState.Idle:
            {
                if(ConsumeStateImpulse())
                    EnemyNavAgent.ResetPath();

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic() || NearTargetLogic()) {}
                else if(StateTimer <= 0)
                {   
                    if(OverhaulDirector.Instance.CurrentExtraction != null)
                        Target = OverhaulDirector.Instance.CurrentExtraction;
                    else
                        Target = LevelGenerator.Instance.LevelPathTruck;
                    
                    SyncTargets();
                    SetState(TrudgeState.ApproachTarget, Random.Range(15f, 35f));
                }

                break;
            }
            case TrudgeState.ApproachTarget:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target != null && Utils.FindNavPosition(Target.transform.position, out Vector3 navPosition))
                        EnemyNavAgent.SetDestination(navPosition);
                    else if (Utils.IsHost())
                        SetState(TrudgeState.Idle, 0f);
                    else
                        StateBeginImpulse = true;
                    
                    OverrideMovement(1f, 1f);
                }

                LookTowardsMovement();

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic() || NearTargetLogic()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.LookAroundBegin, Random.Range(4f, 6f));
                
                if(StateEndImpulse)
                    EndMovementOverride();

                break;
            }
            case TrudgeState.LookAroundBegin:
            {
                if(ConsumeStateImpulse())
                    EnemyNavAgent.ResetPath();
                
                // Grunting sound or some kind of animation?

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.LookAround, Random.Range(2f, 3f));

                break;
            }
            case TrudgeState.LookAround:
            {
                if(ConsumeStateImpulse())
                {
                    RotationSpeed = 0.3f;
                    Look(Random.insideUnitCircle);
                }

                if(!Utils.IsHost()) {}
                else if(GetPlayerViewed(out PlayerAvatar player))
                {
                    Target = player;
                    SyncTargets();
                    SetState(TrudgeState.Notice, 2f);
                }
                else if(PlayerTouchedNoticeLogic()) {}
                else if(StateTimer <= 0)
                {
                    float random = Random.Range(0f, 1f);
                    if(random < 0.6f)
                        SetState(TrudgeState.LookAround, Random.Range(2f, 3f));
                    else
                        SetState(TrudgeState.Idle, 2f);
                }

                if(StateEndImpulse)
                    RotationSpeed = 0.5f;

                break;
            }
            case TrudgeState.Notice:
            {
                if(ConsumeStateImpulse())
                {
                    EnemyNavAgent.ResetPath();
                    Animator.SetTrigger(AnimNotice);
                }

                if(Target != null)
                    Look(Target.transform.position - transform.position);

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Charge, 8f);

                break;
            }
            case TrudgeState.Charge:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target != null && Utils.FindNavPosition(Target.transform.position, out Vector3 navPosition))
                        EnemyNavAgent.SetDestination(navPosition);
                    else if (Utils.IsHost())
                        SetState(TrudgeState.Idle, 0f);
                    else
                        StateBeginImpulse = true;

                    OverrideMovement(10, 2f);
                    StateInternalTimer1 = 0.5f;
                    StateInternalTimer2 = 2f;
                }

                if(StateInternalTimer1 <= 0)
                {
                    StateInternalTimer1 = 0.1f;

                    if(Utils.FindNavPosition(transform.position + NavAgent.velocity.normalized, out Vector3 navPosition, 1f))
                        EnemyNavAgent.SetDestination(navPosition);
                }

                LookTowardsMovement();

                if(!Utils.IsHost()) {}
                else if (
                    StateTimer <= 0 || 
                    (Target != null && Utils.WeightedDistance(transform.position, Target.transform.position, y:0.2f) < 2f) || 
                    (StateInternalTimer2 <= 0 && Vector3.Magnitude(NavAgent.velocity) < 2f))
                    SetState(TrudgeState.Attack, 1f);

                if(StateEndImpulse)
                    EndMovementOverride();

                break;
            }
            case TrudgeState.Attack:
            {
                if(ConsumeStateImpulse())
                {
                    EnemyNavAgent.ResetPath();
                    Animator.SetTrigger(AnimAttack);
                }

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 1f);
                    
                if(StateEndImpulse)
                    Target = null;

                break;
            }
            case TrudgeState.AttackTarget:
            {
                if(ConsumeStateImpulse())
                {
                    ExtractionPoint targetExtraction = Target.GetComponent<ExtractionPoint>();
                    
                    if(targetExtraction != null)
                    {
                        targetExtraction.enabled = false;
                        RoundDirector.instance.ExtractionCompleted();
                        RoundDirector.instance.ExtractionCompletedAllCheck();
                        //targetExtraction.Set("currentState", ExtractionPoint.State.Success);
                    }
                }

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);

                break;
            }
        }            

		transform.rotation = SemiFunc.SpringQuaternionGet(RotationSpring, RotationTarget, RotationSpeed * Time.deltaTime);

        StateEndImpulse = false;
        TouchImpulse = false;
        VisionImpulse = false;

        if(FutureState != TrudgeState.Invalid)
        {
            StateBeginImpulse = true;
            State = FutureState;
        }
    }

    public void SetState(TrudgeState state, float time)
    {
        if(StateTimerOverrides.Count > 0)
        {
            StateTimerOverride timerOverride = StateTimerOverrides.Pop();
            if(timerOverride.State == state)
                time = timerOverride.Time;
        }
        
        StateBeginImpulse = true;
        StateEndImpulse = true;
        StateTimer = time;
        State = state;

        Plugin.Logger.LogInfo($"{state}, {time}");

        PhotonView.RPC("SetStateRPC", RpcTarget.Others, state, time);
    }

    private void SetStateRPC(TrudgeState state, float time)
    {
        FutureState = state;
        StateTimer = time;
    }

    public void SyncTargets()
    {
        PhotonView targetPhotonView = Target.GetComponent<PhotonView>();
        int targetID;

        if(targetPhotonView != null)
            targetID = targetPhotonView.ViewID;
        else if(Target != null)
            targetID = -2;
        else
            targetID = -1;

        PhotonView.RPC("SyncTargetsRPC", RpcTarget.Others, targetID);
    }

    [PunRPC]
    private void SyncTargetsRPC(int targetID)
    {
        if(targetID == -1)
            Target = null;
        else if (targetID == -2)
            Target = LevelGenerator.Instance.LevelPathTruck;
        else
            Target = PhotonView.Find(targetID);
    }

    public bool ConsumeStateImpulse()
    {
        if(StateBeginImpulse)
        {
            StateBeginImpulse = false;
            return true;
        }

        return false;
    }

    public void LookTowardsMovement()
    {
        Vector3 agentVelocity = EnemyNavAgent.Get<Vector3, EnemyNavMeshAgent>("AgentVelocity");
        if (agentVelocity.magnitude > 0.1f)
            Look(agentVelocity);
    }

    public void Look(Vector3 direction)
    {
        RotationTarget = Quaternion.LookRotation(direction.normalized);
        RotationTarget.eulerAngles = new Vector3(0f, RotationTarget.eulerAngles.y, 0f);
    }

    public bool NearTargetLogic()
    {
        if(Target == null)
            return false;

        float targetDistance = Utils.WeightedDistance(transform.position, Target.transform.position, y:0.2f);

        if(targetDistance < 3f)
        {
            SetState(TrudgeState.AttackTarget, 5f);
            return true;
        }

        return false;
    }

    public bool PlayerTouchedNoticeLogic()
    {
        if(TouchImpulse)
        {
            Target = TouchedPlayer;
            SyncTargets();
            SetState(TrudgeState.Notice, 2f);
            return true;
        }

        return false;
    }

    public bool GetPlayerViewed(out PlayerAvatar player)
    {
        player = null;

        if(VisionImpulse)
        {
            int mostVisionsID = -1;
            int mostVisions = 0;

            foreach(KeyValuePair<int, int> visionData in Vision.VisionsTriggered)
            {
                if(visionData.Value > mostVisions)
                {
                    mostVisionsID = visionData.Key;
                    mostVisions = visionData.Value;
                }
            }

            if(mostVisionsID == -1)
                return false;

            player = PhotonView.Find(mostVisionsID).GetComponent<PlayerAvatar>();
            return true;
        }

        return false;
    }

    public void HeadLookAt()
    {
        LookAtTransform.localRotation = Quaternion.Lerp(LookAtTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
		Vision.VisionTransform.localRotation = Quaternion.identity;
    }

    public void OverrideMovement(float speed, float acceleration, float time = float.PositiveInfinity)
    {
        EnemyNavAgent.OverrideAgent(speed, acceleration, time);
        Rigidbody.OverrideFollowPosition(time, speed);
    }

    public void EndMovementOverride()
    {
        OverrideMovement(0, 0, 0.001f);
    }
}