using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;
using HarmonyLib;

namespace Ardot.REPO.EnemyOverhaul;

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

        trudge.ParticleDeathImpact = __instance.particleDeathImpact;
        trudge.ParticleDeathBitsFar = __instance.particleDeathBitsFar;
        trudge.ParticleDeathBitsShort = __instance.particleDeathBitsShort;
        trudge.ParticleDeathSmoke = __instance.particleDeathSmoke;

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
        StuckAttack,
        AttackTarget,
    }
    
    public PhotonView PhotonView;
    public Enemy Enemy;
    public EnemyParent EnemyParent;
    public EnemyVision Vision;
    public EnemyNavMeshAgent EnemyNavAgent;
    public NavMeshAgent NavAgent;
    public EnemyRigidbody Rigidbody;
    public EnemySlowWalkerAnim EnemyAnimator;
    public EnemyStateInvestigate StateInvestigate;
    public Animator Animator;
    public SpringQuaternion RotationSpring;
    public Transform LookAtTransform;
    public MonoBehaviour Target;

    public record struct StateTimerOverride(TrudgeState State, float Time);
    public Stack<StateTimerOverride> StateTimerOverrides = new ();

    public TrudgeState State = TrudgeState.Invalid;
    public TrudgeState FutureState = TrudgeState.Invalid;

    public Quaternion RotationTarget;

    public float DefaultRotationSpeed;
    public float TargetClosestApproach = float.PositiveInfinity;

    public float StateTimer = 0f;
    public float StateInternalTimer1 = 0f;
    public float StateInternalTimer2 = 0f;

    public bool StateBeginImpulse = false;
    public bool StateEndImpulse = false;
    public bool VisionImpulse = false;
    public bool InvestigateImpulse = false;
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

    public ParticleSystem ParticleDeathImpact;
	public ParticleSystem ParticleDeathBitsFar;
	public ParticleSystem ParticleDeathBitsShort;
	public ParticleSystem ParticleDeathSmoke;

    public void Awake()
    {
        Enemy = GetComponent<Enemy>();
        PhotonView = GetComponent<PhotonView>();
        EnemyNavAgent = GetComponent<EnemyNavMeshAgent>(); 
        NavAgent = GetComponent<NavMeshAgent>();
        StateInvestigate = GetComponent<EnemyStateInvestigate>();
    }

    public void Start()
    {
        Vision = (EnemyVision)Enemy.Get("Vision");
        Rigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        Animator = (Animator)EnemyAnimator.Get("animator");
        EnemyParent = (EnemyParent)Enemy.Get("EnemyParent");
        EnemyStateSpawn stateSpawn = GetComponent<EnemyStateSpawn>();
        EnemyHealth health = (EnemyHealth)Enemy.Get("Health");

        StateInvestigate.enabled = false;
        DefaultRotationSpeed = RotationSpring.speed;
        NavAgent.areaMask = -1;
        EnemyAnimator.slowWalkerAttack.vacuumSphere.localScale *= 1.7f;

        List<HurtCollider> impactColliders = Utils.GetHurtColliders(EnemyAnimator.slowWalkerAttack.attackImpactHurtColliders.transform);
        for(int x = 0; x < impactColliders.Count; x++)
        {
            HurtCollider collider = impactColliders[x];
            collider.playerKill = false;
            collider.playerDamage = 30;
        }
        
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
        StateInvestigate.onInvestigateTriggered.AddListener(() => {
            InvestigateImpulse = true;
        });
        stateSpawn.OnSpawn.AddListener(() => {
            if(Utils.IsHost())
                SetState(TrudgeState.Spawn, 1f);
        });
        health.onDeath.AddListener(() => {
            ParticleDeathImpact.transform.position = Enemy.CenterTransform.position;
            ParticleDeathImpact.Play();
            ParticleDeathBitsFar.transform.position = Enemy.CenterTransform.position;
            ParticleDeathBitsFar.Play();
            ParticleDeathBitsShort.transform.position = Enemy.CenterTransform.position;
            ParticleDeathBitsShort.Play();
            ParticleDeathSmoke.transform.position = Enemy.CenterTransform.position;
            ParticleDeathSmoke.Play();
            EnemyAnimator.sfxDeath.Play(EnemyAnimator.transform.position, 1f, 1f, 1f, 1f);
            GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, transform.position, 0.5f);
            GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, transform.position, 0.05f);
            if (Utils.IsHost())
                EnemyParent.Despawn();
        });
        // health.onHurt.AddListener(() => {

        // });
    }

    public void Update()
    {
        if(Enemy.CurrentState == EnemyState.Despawn || Enemy.CurrentState == EnemyState.Stunned)
            return;

        StateTimer -= Time.deltaTime;
        StateInternalTimer1 -= Time.deltaTime;
        StateInternalTimer2 -= Time.deltaTime;

        if(FutureState != TrudgeState.Invalid)
            StateEndImpulse = true;

        if(Utils.IsHost() && State != TrudgeState.AttackTarget && (OverhaulDirector.Instance.ExtractionCompletedImpulse || OverhaulDirector.Instance.ExtractionUnlockedImpulse))
            SetState(TrudgeState.Idle, 0f);
        
        HeadLookAt();

        switch(State)
        {
            case TrudgeState.Spawn:
            {
                if(ConsumeStateImpulse())
                {
                    MonoBehaviour target = GetTarget();
                    LevelPoint spawnPoint = Utils.ChooseLevelPoint(target.transform.position, 50f, 0.2f);
                    Enemy.EnemyTeleported(spawnPoint.transform.position);
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 1f);

                break;
            }
            case TrudgeState.Idle:
            {
                if(ConsumeStateImpulse())
                    EnemyNavAgent.ResetPath();

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic() || NearTargetLogic() || InvestigateLogic()) {}
                else if(StateTimer <= 0)
                {   
                    Target = GetTarget();
                    
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
                    {
                        EnemyNavAgent.SetDestination(navPosition);
                        OverrideMovement(0.2f, 10f, 1f);
                    }
                    else if (Utils.IsHost())
                        SetState(TrudgeState.Idle, 0f);
                    else
                        StateBeginImpulse = true;
                    
                    Rigidbody.Set("notMovingTimer", 0f);
                    Animator.SetBool(AnimMoving, true);
                }

                LookTowardsMovement();

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic() || NearTargetLogic() || InvestigateLogic()) {}
                else if((float)Rigidbody.Get("notMovingTimer") > 3f)
                    SetState(TrudgeState.StuckAttack, 3f);
                else if(StateTimer <= 0)
                    SetState(TrudgeState.LookAroundBegin, Random.Range(4f, 6f));
                
                if(StateEndImpulse)
                {
                    Animator.SetBool(AnimMoving, false);
                    EndMovementOverride();
                }

                break;
            }
            case TrudgeState.LookAroundBegin:
            {
                if(ConsumeStateImpulse())
                {
                    EnemyNavAgent.ResetPath();
                    Animator.SetTrigger(AnimNotice);
                    RotationSpring.speed = 2f;
                }

                if(!Utils.IsHost()) {}
                else if(PlayerTouchedNoticeLogic()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.LookAround, Random.Range(3f, 5f));

                break;
            }
            case TrudgeState.LookAround:
            {
                if(ConsumeStateImpulse())
                {
                    RotationSpring.speed = 2f;
                    Look(Random.insideUnitCircle);
                }

                if(InvestigateImpulse)
                {
                    StateTimer += 3f;   
                    Look((Vector3)StateInvestigate.Get("onInvestigateTriggeredPosition") - transform.position);
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
                    if(random < 0.4f)
                        SetState(TrudgeState.LookAround, Random.Range(3f, 5f));
                    else
                        SetState(TrudgeState.Idle, 0f);
                }

                if(StateEndImpulse)
                    RotationSpring.speed = DefaultRotationSpeed;

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

                    OverrideMovement(10f, 10f, 2f);
                    StateInternalTimer1 = 0.5f;
                    StateInternalTimer2 = 2f;
                    Animator.SetBool(AnimMoving, true);
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
                    (Target != null && Utils.WeightedDistance(transform.position, Target.transform.position, y:0.2f) < 3f) || 
                    (StateInternalTimer2 <= 0 && Vector3.Magnitude(NavAgent.velocity) < 3f))
                    SetState(TrudgeState.Attack, 5f);

                if(StateEndImpulse)
                {
                    Animator.SetBool(AnimMoving, false);
                    EndMovementOverride();
                }

                break;
            }
            case TrudgeState.Attack:
            {
                if(ConsumeStateImpulse())
                {
                    EnemyNavAgent.ResetPath();
                    EnemyNavAgent.Warp(Rigidbody.transform.position);
                    Animator.SetTrigger(AnimAttack);
                }

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);
                    
                if(StateEndImpulse)
                    Target = null;

                break;
            }
            case TrudgeState.StuckAttack:
            {
                if(ConsumeStateImpulse())
                    Animator.SetTrigger(AnimStuckAttack);

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);

                break;
            }
            case TrudgeState.AttackTarget:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target == null)
                        StateBeginImpulse = true;
                    else
                    {
                        Animator.SetTrigger(AnimAttack);
                        ExtractionPoint targetExtraction = Target.GetComponent<ExtractionPoint>();
                        
                        if(targetExtraction != null)
                            OverhaulDirector.Instance.DestroyExtractionPoint(targetExtraction);
                        else
                            OverhaulDirector.Instance.GameOver();
                    }
                }

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);
                        
                if(StateEndImpulse)
                    Target = null;

                break;
            }
        }            

		transform.rotation = SemiFunc.SpringQuaternionGet(RotationSpring, RotationTarget, Time.deltaTime);

        StateEndImpulse = false;
        TouchImpulse = false;
        VisionImpulse = false;
        InvestigateImpulse = false;

        if(FutureState != TrudgeState.Invalid)
        {
            StateBeginImpulse = true;
            State = FutureState;
            FutureState = TrudgeState.Invalid;
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

        if(SemiFunc.IsMultiplayer())
            PhotonView.RPC("SetStateRPC", RpcTarget.Others, state, time);
    }

    [PunRPC]
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

        if(SemiFunc.IsMultiplayer())
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

    public MonoBehaviour GetTarget()
    {
        if(OverhaulDirector.Instance.CurrentExtraction != null)
            return OverhaulDirector.Instance.CurrentExtraction;
        else
            return LevelGenerator.Instance.LevelPathTruck;
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

    public bool InvestigateLogic()
    {
        if(!InvestigateImpulse || Random.Range(0f, 1f) < 0.95f)
            return false;

        SetState(TrudgeState.LookAroundBegin, 2f);
        return true;
    }

    public bool NearTargetLogic()
    {
        if(Target == null)
            return false;

        float targetDistance = Utils.WeightedDistance(transform.position, Target.transform.position, y:0.2f);

        if(targetDistance < 100f && TargetClosestApproach > 100f)
        {
            TargetClosestApproach = targetDistance;
            SemiFunc.UIBigMessage("UNIDENTIFIED MASS DETECTED", "{!}", 20f, Color.yellow, Color.red);
            BigMessageUI.instance.Set("bigMessageTimer", 3f);
        }
        if(targetDistance < 20f && TargetClosestApproach > 20f)
        {
            TargetClosestApproach = targetDistance;
            SemiFunc.UIBigMessage("UNIDENTIFIED MASS APPROACHING ASSETS", "{!}", 20f, Color.red, Color.red);
            BigMessageUI.instance.Set("bigMessageTimer", 3f);
        }
        if(targetDistance < 1f)
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

    public void OverrideMovement(float speed, float rigidbodySpeed, float acceleration, float time = float.PositiveInfinity)
    {
        EnemyNavAgent.OverrideAgent(speed, acceleration, time);
        Rigidbody.OverrideFollowPosition(time, rigidbodySpeed);
    }

    public void EndMovementOverride()
    {
        OverrideMovement(0, 0, 0.001f);
    }
}