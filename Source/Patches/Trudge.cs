using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;
using HarmonyLib;
using BepInEx.Configuration;

namespace Ardot.REPO.EnemyOverhaul;

public static class TrudgeOverhaul
{
    public static ConfigEntry<bool> OverhaulAI;

    public static void Init()
    {
        OverhaulAI = Plugin.Config.Bind(
            "Trudge",
            "OverhaulAI",
            true,
            "If true, Trudge AI is overhauled"
        );

        if(!OverhaulAI.Value)
            return;

        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemySlowWalker), "Start"),
            prefix: new HarmonyMethod(typeof(TrudgeOverhaul), "StartPrefix")
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

        trudge.FeetTransform = __instance.feetTransform;
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
        Notice,
        ChargeBegin,
        Charge,
        Attack,
        StuckAttack,
        DestroyTarget,
    }
    
    public PhotonView PhotonView;
    public Enemy Enemy;
    public EnemyParent EnemyParent;
    public EnemyVision Vision;
    public EnemyNavMeshAgent EnemyNavAgent;
    public NavMeshAgent NavAgent;
    public EnemyRigidbody EnemyRigidbody;
    public Rigidbody Rigidbody;
    public EnemySlowWalkerAnim EnemyAnimator;
    public EnemyStateInvestigate StateInvestigate;
    public Animator Animator;
    public SpringQuaternion RotationSpring;
    public Transform LookAtTransform;
    public Transform FeetTransform;
    public MonoBehaviour Target;

    public float PrevAggro = 0;
    public float Aggro = 0;

    public record struct StateTimerOverride(TrudgeState State, float Time);
    public Stack<StateTimerOverride> StateTimerOverrides = new ();

    public TrudgeState State = TrudgeState.Invalid;
    public TrudgeState FutureState = TrudgeState.Invalid;

    public Quaternion RotationTarget;

    public float DefaultRotationSpeed;
    public float TargetClosestApproach = float.PositiveInfinity;

    public float StateTimer = 0f;

    public object StateInternalState;

    public bool StateBeginImpulse = false;
    public bool StateEndImpulse = false;
    public bool StateSetImpulse = false;
    public bool HandleAggro = true;

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
        EnemyRigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        Animator = (Animator)EnemyAnimator.Get("animator");
        EnemyParent = (EnemyParent)Enemy.Get("EnemyParent");
        Rigidbody = (Rigidbody)EnemyRigidbody.Get("rb");
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
        
        EnemyRigidbody.onGrabbed.AddListener(() => {
            if(HandleAggro)
                Aggro += Time.deltaTime * 5f;
        });
        EnemyRigidbody.onTouchPlayer.AddListener(() => {
            if(HandleAggro)
                Aggro += Time.deltaTime * 5f;
        });
        EnemyRigidbody.onTouchPlayerGrabbedObject.AddListener(() => {
            if(HandleAggro)
                Aggro += Time.deltaTime * 5f;
        });
        StateInvestigate.onInvestigateTriggered.AddListener(() => {
            if(HandleAggro)
                Aggro += 2f / Vector3.Distance(transform.position, (Vector3)StateInvestigate.Get("onInvestigateTriggeredPosition"));
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
            {
                EnemyParent.Despawn();
                SetState(TrudgeState.Spawn, 1f);
            }
        });
        health.onHurt.AddListener(() => {
            if(HandleAggro)
                Aggro += 15f;
        });
    }

    public void Update()
    {
        if(Utils.IsHost() && State != TrudgeState.DestroyTarget && (OverhaulDirector.Instance.ExtractionCompletedImpulse || OverhaulDirector.Instance.ExtractionUnlockedImpulse))
        {
            TargetClosestApproach = float.PositiveInfinity;
            SetState(TrudgeState.Idle, 0f);
        }

        if(Enemy.CurrentState == EnemyState.Despawn || Enemy.CurrentState == EnemyState.Stunned)
            return;

        StateTimer -= Time.deltaTime;

        if(FutureState != TrudgeState.Invalid)
            StateEndImpulse = true;
        
        HeadLookAt();
        AggroLogic();

        switch(State)
        {
            case TrudgeState.Spawn:
            {
                if(ConsumeStateImpulse())
                {
                    HandleAggro = true;
                    MonoBehaviour target = GetTarget();
                    LevelPoint spawnPoint = Utils.ChooseLevelPoint(target.transform.position, 50f, 0.2f);
                    Enemy.EnemyTeleported(spawnPoint.transform.position);
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 1f);

                break;
            }
            case TrudgeState.Idle:
            {
                if(ConsumeStateImpulse())
                {
                    HandleAggro = true;
                    EnemyNavAgent.ResetPath();
                }

                NearTargetLogic();

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                {   
                    Target = GetTarget();
                    
                    SyncTargets();
                    SetState(TrudgeState.ApproachTarget, Random.Range(5f, 10f));
                }

                break;
            }
            case TrudgeState.ApproachTarget:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target != null && Utils.FindNavPosition(Target.transform.position, out Vector3 navPosition))
                    {
                        HandleAggro = true;
                        EnemyNavAgent.SetDestination(navPosition);

                        float speed;
                        float targetDistance = Vector3.Distance(transform.position, Target.transform.position);

                        if(targetDistance < 40f)
                            speed = 0.15f;
                        else if(targetDistance < 80f)
                            speed = 0.3f;
                        else
                            speed = 0.4f;

                        if(Target is LevelPoint && (int)RoundDirector.instance.Get("extractionPointsCompleted") != 1)
                            speed *= 4;

                        OverrideMovement(speed, 10f, 1f);
                    }
                    else if (Utils.IsHost())
                        SetState(TrudgeState.Idle, 0f);
                    else
                        StateBeginImpulse = true;

                    HandleAggro = true;   
                    EnemyRigidbody.Set("notMovingTimer", 0f);
                    Animator.SetBool(AnimMoving, true);
                }

                LookTowardsMovement();
                NearTargetLogic();

                if(!Utils.IsHost()) {}
                else if((float)EnemyRigidbody.Get("notMovingTimer") > 3f)
                    SetState(TrudgeState.StuckAttack, 3f);
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);
                
                if(StateEndImpulse)
                {
                    Animator.SetBool(AnimMoving, false);
                    EndMovementOverride();
                }

                break;
            }
            case TrudgeState.Notice:
            {
                if(ConsumeStateImpulse())
                {
                    HandleAggro = false;
                    EnemyNavAgent.ResetPath();
                    EnemyNavAgent.Warp(FeetTransform.position);
                    Animator.SetTrigger(AnimNotice);
                }

                if(Target != null)
                    Look(Target.transform.position - transform.position);

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Charge, 2f);

                break;
            }
            case TrudgeState.Charge:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target == null)
                        StateBeginImpulse = true;
                    else
                    {
                        StateInternalState = Target.transform.position + Random.insideUnitSphere;
                        EnemyNavAgent.SetDestination((Vector3)StateInternalState);

                        HandleAggro = false;
                        OverrideMovement(10f, 10, 5f);
                        Animator.SetTrigger(AnimJump);
                        Rigidbody.AddForce(Vector3.up * 20, ForceMode.Impulse);
                    }
                }

                Vector3 targetPosition = (Vector3)StateInternalState;
                LookTowardsMovement();

                if(!Utils.IsHost()) {}
                else if (StateTimer <= 0 || Utils.WeightedDistance(targetPosition, FeetTransform.position, y:0f) < 1f)
                    SetState(TrudgeState.Attack, 4f);

                if(StateEndImpulse)
                    EndMovementOverride();

                break;
            }
            case TrudgeState.Attack:
            {
                if(ConsumeStateImpulse())
                {
                    Aggro -= 8f;
                    EnemyNavAgent.ResetPath();
                    EnemyNavAgent.Warp(FeetTransform.position);
                    Animator.SetTrigger(AnimAttack);
                    HandleAggro = false;
                }

                if(Utils.IsHost())
                {
                    if(StateTimer <= 0)
                    {
                        if(Target != null)
                        {
                            float targetDistance = Vector3.Distance(transform.position, Target.transform.position);

                            if(Target.GetComponent<PlayerAvatar>() == null && targetDistance < 2f)
                                SetState(TrudgeState.DestroyTarget, 5f);

                            if(Aggro > 8f)
                            {
                                if(targetDistance < 12f)
                                    SetState(TrudgeState.Charge, 4f);

                                SetState(TrudgeState.Attack, 4f);
                            }
                        }

                        SetState(TrudgeState.Idle, 0f);
                    }
                }

                break;
            }
            case TrudgeState.StuckAttack:
            {
                if(ConsumeStateImpulse())
                {
                    HandleAggro = true;                    
                    Animator.SetTrigger(AnimStuckAttack);
                }

                if(!Utils.IsHost()) {}
                else if(StateTimer <= 0)
                    SetState(TrudgeState.Idle, 0f);

                break;
            }
            case TrudgeState.DestroyTarget:
            {
                if(ConsumeStateImpulse())
                {
                    if(Target == null)
                        StateBeginImpulse = true;
                    else
                    {
                        HandleAggro = true;                        
                        ExtractionPoint targetExtraction = Target.GetComponent<ExtractionPoint>();
                        
                        if(targetExtraction != null)
                            OverhaulDirector.Instance.DestroyExtractionPoint(targetExtraction);
                        else
                            OverhaulDirector.Instance.GameOver();

                        TargetClosestApproach = float.PositiveInfinity;
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

        if(FutureState != TrudgeState.Invalid && !StateSetImpulse)
        {
            StateBeginImpulse = true;
            State = FutureState;
            FutureState = TrudgeState.Invalid;
        }

        StateEndImpulse = false;
        StateSetImpulse = false;
    }

    [PunRPC]
    public void SetState(TrudgeState state, float time)
    {
        if(FutureState != TrudgeState.Invalid)
            return;
        
        StateTimer = time;
        FutureState = state;
        StateSetImpulse = true;

        if(SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient())
            PhotonView.RPC("SetState", RpcTarget.Others, state, time);
    }

    public void SyncTargets()
    {
        if(Target == null)
            return;

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

    public bool NearTargetLogic()
    {
        if(Target == null)
            return false;

        float targetDistance = Utils.WeightedDistance(transform.position, Target.transform.position, y:0.2f);

        if(Target.GetComponent<PlayerAvatar>() == null)
        {
            if(targetDistance < 6f && TargetClosestApproach > 6f)
            {
                SemiFunc.UIBigMessage("TIME IS RUNNING OUT", "{!}", 20f, Color.red, Color.red);
                BigMessageUI.instance.Set("bigMessageTimer", 3f);
            }
            else if(targetDistance < 10f && TargetClosestApproach > 10f)
            {
                SemiFunc.UIBigMessage("DANGER [REDACTED] MASS DANGER HURRY", "{!}", 20f, Color.red, Color.red);
                BigMessageUI.instance.Set("bigMessageTimer", 3f);
            }
            else if(targetDistance < 20f && TargetClosestApproach > 20f)
            {
                SemiFunc.UIBigMessage("UNIDENTIFIED MASS APPROACHING ASSETS", "{!}", 20f, Color.red, Color.red);
                BigMessageUI.instance.Set("bigMessageTimer", 3f);
            }
            else if(targetDistance < 40f && TargetClosestApproach > 40f)
            {
                SemiFunc.UIBigMessage("UNIDENTIFIED MASS GETTING CLOSER", "{!}", 20f, Color.red, Color.red);
                BigMessageUI.instance.Set("bigMessageTimer", 3f);
            }
            else if(targetDistance < 100f && TargetClosestApproach > 100f)
            {
                SemiFunc.UIBigMessage("UNIDENTIFIED MASS DETECTED", "{!}", 20f, Color.yellow, Color.red);
                BigMessageUI.instance.Set("bigMessageTimer", 3f);
            }

            TargetClosestApproach = targetDistance;
            
            if(Utils.IsHost() && targetDistance < 4f)
            {
                SetState(TrudgeState.Charge, 4f);
                return true;
            }
        }

        return false;
    }

    public void AggroLogic()
    {
        if(!HandleAggro)
            return;

        Aggro = Mathf.Max(Aggro - Time.deltaTime, 0);

        if(Aggro > 7.5f && PrevAggro < 7.5f)
            EnemyAnimator.sfxNoticeVoice.Play(transform.position);
        
        if(Utils.IsHost() && Aggro > 15f && PrevAggro < 15f)
        {
            PlayerAvatar nearestPlayer = Utils.GetNearestPlayer(transform.position, out float distance);

            if(nearestPlayer != null && distance < 12f)    
            {
                Target = nearestPlayer;
                SetState(TrudgeState.Notice, Random.Range(1f, 2f));
            }
            else
                SetState(TrudgeState.Attack, 3f);
        }

        PrevAggro = Aggro;
    }

    public void HeadLookAt()
    {
        LookAtTransform.localRotation = Quaternion.Lerp(LookAtTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
		Vision.VisionTransform.localRotation = Quaternion.identity;
    }

    public void OverrideMovement(float speed, float rigidbodySpeed, float acceleration, float time = float.PositiveInfinity)
    {
        EnemyNavAgent.OverrideAgent(speed, acceleration, time);
        EnemyRigidbody.OverrideFollowPosition(time, rigidbodySpeed);
    }

    public void EndMovementOverride()
    {
        OverrideMovement(0, 0, 0.001f);
    }
}