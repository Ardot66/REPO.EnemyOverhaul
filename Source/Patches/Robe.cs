using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Photon.Pun;
using System;

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
        RobeOverride robeOverride = __instance.gameObject.AddComponent<RobeOverride>();
        robeOverride.RotationSpring = __instance.rotationSpring;

        robeOverride.Animator = __instance.robeAnim.GetComponent<Animator>();
        robeOverride.deathParticles = __instance.robeAnim.deathParticles;
        robeOverride.spawnParticles = __instance.robeAnim.spawnParticles;
        robeOverride.sfxTargetPlayerLoop = __instance.robeAnim.sfxTargetPlayerLoop;
        robeOverride.sfxIdleBreak = __instance.robeAnim.sfxIdleBreak;
        robeOverride.sfxAttack = __instance.robeAnim.sfxAttack;
        robeOverride.sfxAttackGlobal = __instance.robeAnim.sfxAttackGlobal;
        robeOverride.sfxHurt = __instance.robeAnim.sfxHurt;
        robeOverride.sfxHandIdle = __instance.robeAnim.sfxHandIdle;
        robeOverride.sfxHandAggressive = __instance.robeAnim.sfxHandAggressive;
        robeOverride.sfxStunStart = __instance.robeAnim.sfxStunStart;
        robeOverride.sfxStunLoop = __instance.robeAnim.sfxStunLoop;
        robeOverride.sfxAttackUnder = __instance.robeAnim.sfxAttackUnder;
        robeOverride.sfxAttackUnderGlobal = __instance.robeAnim.sfxAttackUnderGlobal;
        GameObject.Destroy(__instance.robeAnim);
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
        Invalid,
        Spawn,
        Idle,
        Shifty,
        Roam,
        FollowPlayer,
        HelpPlayer,
        ChaseBegin,
        Chase,
        Attack,
        AttackUnderBegin,
        AttackUnder,
        GiveSpace,
    }

    public RobeState State = RobeState.Idle;
    public RobeState FutureState = RobeState.Invalid;
    public float StateTimer = 0f;
    public float StateInternalTimer = 0f;
    public float LastFocusedPlayerVisionTimer = float.PositiveInfinity;
    public float ItemBreakLogicTimer = 0f;
    public float InterestTimer = float.PositiveInfinity;
    public bool IgnoringPlayers = false;
    public bool VisionImpulse = false;
    public bool PlayerAggressiveImpulse = false;
    public bool TouchedImpulse = false;
    public bool ObjectBreakImpulse = false;
    public bool StateImpulse = true;
    public bool StateEndedImpulse = false;
    public Enemy Enemy;
    public EnemyParent EnemyParent;
    public PhotonView PhotonView;
    public EnemyVision Vision;
    public EnemyRigidbody Rigidbody;
    public Animator Animator;
    public HurtCollider HurtCollider;
    public EnemyNavMeshAgent EnemyAgent;
    public PlayerAvatar FocusedPlayer;
    public PlayerAvatar TouchedPlayer;
    public PlayerAvatar AggressivePlayer;
    public SpringQuaternion RotationSpring;
    public ValuableObject TargetValuable;
    public List<ValuableObject> TrackedValuables = new ();
    public List<PlayerAvatar> VisionTriggeredPlayers = new ();
    public Quaternion RotationTarget;

	public ParticleSystem[] deathParticles;
	public ParticleSystem spawnParticles;
	public Sound sfxTargetPlayerLoop;
	public Sound sfxIdleBreak;
	public Sound sfxAttack;
	public Sound sfxAttackGlobal;
	public Sound sfxHurt;
	public Sound sfxHandIdle;
	public Sound sfxHandAggressive;
	public Sound sfxStunStart;
	public Sound sfxStunLoop;
	public Sound sfxAttackUnder;
	public Sound sfxAttackUnderGlobal;

    public bool ConsumeStateImpulse()
    {
        bool started = StateImpulse;
        StateImpulse = false;
        return started;
    }

    public void SetState(RobeState state, float stateTimer)
    {
        if (state == State)
            return;

        StateImpulse = true;
        StateEndedImpulse = true;
        State = state;
        StateTimer = stateTimer;
        
        if(GameManager.Multiplayer())
            PhotonView.RPC("SetStateRPC", RpcTarget.Others, state, stateTimer);
    }

    public void SyncFields()
    {
        if(!GameManager.Multiplayer())
            return;

        PhotonView.RPC("SyncFieldsRPC", RpcTarget.Others, 
            FocusedPlayer ? FocusedPlayer.photonView.ViewID : -1, 
            TargetValuable ? TargetValuable.GetComponent<PhotonView>().ViewID : -1);
    }

    [PunRPC]
    private void SyncFieldsRPC(int focusedPlayerID, int targetValuableID)
    {
        FocusedPlayer = focusedPlayerID == -1 ? null : PhotonView.Find(focusedPlayerID).GetComponent<PlayerAvatar>();
        TargetValuable = targetValuableID == -1 ? null : PhotonView.Find(targetValuableID).GetComponent<ValuableObject>();
    }

    [PunRPC]
    private void SetStateRPC(RobeState state, float stateTimer)
    {
        FutureState = state;
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
        Rigidbody = (EnemyRigidbody)Enemy.Get("Rigidbody");
        EnemyAgent = (EnemyNavMeshAgent)Enemy.Get("NavMeshAgent");
        Vision = (EnemyVision)Enemy.Get("Vision");
        EnemyParent = (EnemyParent)Enemy.Get("EnemyParent");

        if(!SemiFunc.IsMasterClientOrSingleplayer()) 
            return;

        EnemyHealth health = GetComponent<EnemyHealth>();
        EnemyStateSpawn stateSpawn = GetComponent<EnemyStateSpawn>();
        EnemyStateDespawn stateDespawn = GetComponent<EnemyStateDespawn>();
        EnemyStateStunned stateStunned = GetComponent<EnemyStateStunned>();

        Vision.onVisionTriggered.AddListener(() => 
        {
            VisionImpulse = true;
            VisionTriggeredPlayers.Add((PlayerAvatar)Vision.Get("onVisionTriggeredPlayer"));
        });
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

        health.onHurt.AddListener(() =>
        {
            sfxHurt.Play(transform.position, 1f, 1f, 1f, 1f);
        });    
        health.onDeath.AddListener(() =>
        {
            Animator.SetTrigger("Death");

            ParticleSystem[] array = deathParticles;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Play();
            }

            EnemyParent.Despawn();
        });
        stateSpawn.OnSpawn.AddListener(() =>
        {
            if(SemiFunc.IsMasterClientOrSingleplayer())
                SetState(RobeState.Spawn, 0.5f);    
        });
        stateDespawn.OnDespawn.AddListener(() =>
        {
            Animator.SetTrigger("despawn");
        });
        stateStunned.onStunnedStart.AddListener(() =>
        {
            Animator.SetTrigger("Stun");
            Animator.SetBool("Stunned", true);
			sfxStunLoop.PlayLoop(true, 2f, 2f, 1f);
        });
        stateStunned.onStunnedEnd.AddListener(() =>
        {
            Animator.SetBool("Stunned", false);
			sfxStunLoop.PlayLoop(false, 2f, 2f, 1f);
        });
    }

    public void Update()
    {
        if(Enemy.CurrentState == EnemyState.Stunned || Enemy.CurrentState == EnemyState.Despawn)
            return;

        StateTimer -= Time.deltaTime;
        StateInternalTimer -= Time.deltaTime;
        InterestTimer -= Time.deltaTime;
        LastFocusedPlayerVisionTimer += Time.deltaTime;

        if(FutureState != RobeState.Invalid)
            StateEndedImpulse = true;

        if(SemiFunc.IsMasterClientOrSingleplayer())
        {
            if(VisionImpulse && !IgnoringPlayers && VisionTriggeredPlayers.Contains(FocusedPlayer) || TouchedImpulse && TouchedPlayer == FocusedPlayer)
            {
                LastFocusedPlayerVisionTimer = 0;
                InterestTimer = UnityEngine.Random.Range(30f, 140f);
            }

            if(InterestTimer <= 0)
            {
                if(!IgnoringPlayers)
                    InterestTimer = UnityEngine.Random.Range(20f, 40f);
                else
                    InterestTimer = float.PositiveInfinity;

                IgnoringPlayers = !IgnoringPlayers;
            }

            ItemBreakTrackingLogic();
            PlayerTrackingLogic();
        }

        switch (State)
        {
            case RobeState.Spawn:
            {
                if(ConsumeStateImpulse())
                {
                    if(!SemiFunc.EnemySpawn(Enemy))
                        SetState(RobeState.Spawn, 0.5f);
                    else
                    {
                        Animator.Play("Robe Spawn", 0, 0f);
                        spawnParticles.Play();
                    }
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                if(StateTimer <= 0)
                    SetState(RobeState.Idle, 1f);

                break;
            }
            case RobeState.Idle:
            {
                if(ConsumeStateImpulse())
                    EnemyAgent.ResetPath();

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(PlayerAggroLogic() || LosePlayerLogic()) {}
                else if(StateTimer <= 0)
                {
                    if(FocusedPlayer != null)
                    {
                        float playerDistance = Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f);
    
                        if (FocusedPlayer.physGrabber.grabbed && FocusedPlayer.physGrabber.Get("grabbedPhysGrabObject") is PhysGrabObject playerGrabbedObject && (TargetValuable = playerGrabbedObject.GetComponent<ValuableObject>()) != null)
                        {
                            SetState(RobeState.HelpPlayer, 30f);
                            SyncFields();
                        }
                        else if(playerDistance > 5f || playerDistance < 2.5f)
                            SetState(RobeState.FollowPlayer, UnityEngine.Random.Range(10f, 12f));
                        else
                            SetState(RobeState.Shifty, 3f);
                    }
                    else
                        SetState(RobeState.Roam, UnityEngine.Random.Range(4f, 9f));
                }

                break;
            }
            case RobeState.Shifty:
            {
                if(ConsumeStateImpulse())
                {
                    Vector3 targetPosition = transform.position + UnityEngine.Random.insideUnitSphere * 4f;

                    if(Utils.FindNavPosition(targetPosition, out Vector3 navPosition, 5f))
                        EnemyAgent.SetDestination(navPosition);

                    OverrideMovement(0.75f, 1f, StateTimer);
                }

                NormalRotationLogic();

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 3f;
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LosePlayerLogic())
                    SetState(RobeState.Idle, UnityEngine.Random.Range(0.25f, 0.5f));
                else if(PlayerAggroLogic()) {}
                else if(playerDistance < 2f || playerDistance > 5f || StateTimer <= 0)
                    SetState(RobeState.Idle, 1f);
                    
                if(StateEndedImpulse)
                    EndMovementOverride();

                break;
            }
            case RobeState.Roam:
            {
                if(ConsumeStateImpulse())
                {
                    if(Utils.FindNavPosition(transform.position + UnityEngine.Random.insideUnitSphere * 10f, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                }

                NormalRotationLogic();

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(PlayerAggroLogic()) {}
                else if(VisionImpulse && !IgnoringPlayers)
                {
                    LastFocusedPlayerVisionTimer = 0f;
                    FocusedPlayer = VisionTriggeredPlayers[UnityEngine.Random.Range(0, VisionTriggeredPlayers.Count)];
                    SetState(RobeState.FollowPlayer, 3f);
                    SyncFields();
                }
                else if(!EnemyAgent.HasPath() || StateTimer <= 0)
                    SetState(RobeState.Idle, UnityEngine.Random.Range(1f, 2f));

                break;
            }
            case RobeState.FollowPlayer:
            {
                if(ConsumeStateImpulse())
                {
                    if(FocusedPlayer != null && Utils.FindNavPosition(FocusedPlayer.transform.position + (transform.position - FocusedPlayer.transform.position).normalized * 3 + UnityEngine.Random.insideUnitSphere * 0.5f, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 3f;
                
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(PlayerAggroLogic()) {}
                else if(LosePlayerLogic())
                    SetState(RobeState.Idle, UnityEngine.Random.Range(0.25f, 0.5f));
                else if (playerDistance > 10f)
                    SetState(RobeState.ChaseBegin, UnityEngine.Random.Range(0.25f, 0.5f));
                else if((playerDistance < 3.5f && playerDistance > 2.5f) || !EnemyAgent.HasPath() || StateTimer <= 0)
                    SetState(RobeState.Idle, 0f);

                break;
            }
            case RobeState.HelpPlayer:
            {
                if(ConsumeStateImpulse())
                {
                    sfxHandIdle.PlayLoop(true, 2f, 2f, 0.4f);
                    OverrideMovement(0.5f, 2, StateTimer);
                    EnemyAgent.ResetPath();
                    StateInternalTimer = 0;
                }

                if(TargetValuable != null)
                {
                    LookAt(TargetValuable.transform.position);
                    Rigidbody targetValuableRigidbody = (Rigidbody)TargetValuable.Get("rb");

                    float distanceToValuable = Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f);;

                    if(distanceToValuable < 3.5f)
                        targetValuableRigidbody.AddForce(new Vector3(0, Mathf.Min(targetValuableRigidbody.mass, 5), 0), ForceMode.Force);

                    if((distanceToValuable < 1.8f || distanceToValuable > 2.6f) && StateInternalTimer <= 0)
                    {
                        StateInternalTimer = 0.1f;
                        Utils.FindNavPosition(TargetValuable.transform.position + (transform.position - TargetValuable.transform.position).normalized * 1.5f, out Vector3 navPosition, 3f);
                        EnemyAgent.SetDestination(navPosition);
                    }
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(PlayerAggroLogic()) {}
                else if(StateTimer <= 0 || (FocusedPlayer != null && !FocusedPlayer.physGrabber.grabbed))
                    SetState(RobeState.Idle, 1f);

                if(StateEndedImpulse)
                {
                    sfxHandIdle.PlayLoop(false, 2f, 2f, 0.4f);
                    EndMovementOverride();
                }

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
                
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);
                    
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}    
                else if(StateTimer <= 0)
                    SetState(RobeState.Chase, UnityEngine.Random.Range(10f, 14f));

                if(StateEndedImpulse)
                    HurtCollider.enabled = true;

                break;
            }
            case RobeState.Chase:
            {
                if(ConsumeStateImpulse())
                {
                    sfxTargetPlayerLoop.PlayLoop(true, 2f, 2f, 2f);
                    OverrideMovement(6, 10, StateTimer);
                    StateInternalTimer = 0;
                }

                NormalRotationLogic();

                if(StateInternalTimer <= 0)
                {
                    StateInternalTimer = 0.1f;
                    if(FocusedPlayer != null && Utils.FindNavPosition(FocusedPlayer.transform.position, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                }

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 10f;
                bool playerCrawling = FocusedPlayer ? (bool)FocusedPlayer.Get("isCrawling") : false;

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LastFocusedPlayerVisionTimer > 8f)
                { 
                    FocusedPlayer = null;
                    SetState(RobeState.GiveSpace, 2f);
                }
                else if(playerDistance < 1.5f && !playerCrawling)
                    SetState(RobeState.Attack, 1f);
                else if(playerDistance < 2f && playerCrawling)
                    SetState(RobeState.AttackUnderBegin, 0.5f);
                else if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);
                    
                if(StateEndedImpulse)
                {
                    EndMovementOverride();
                    sfxTargetPlayerLoop.PlayLoop(false, 2f, 2f, 2f);
                }

                break;
            }
            case RobeState.Attack:
            {
                if(ConsumeStateImpulse())
                {
                    AttackAnimation();
                    EnemyAgent.ResetPath();
                }

                HurtCollider.playerDamage = 120;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                break;
            }
            case RobeState.AttackUnderBegin:
            {
                if(ConsumeStateImpulse())
                {
                    EnemyAgent.ResetPath();
                    Animator.SetTrigger("LookUnder");
                    Animator.SetBool("LookingUnder", true);
                }

                HurtCollider.playerDamage = 80;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                if(StateTimer <= 0)
                    SetState(RobeState.AttackUnder, 0.5f);

                break;
            }
            case RobeState.AttackUnder:
            {
                if(ConsumeStateImpulse())
                {
                    AttackShake();
                    sfxAttackUnder.Play(transform.position, 1f, 1f, 1f, 1f);
		            sfxAttackUnderGlobal.Play(transform.position, 1f, 1f, 1f, 1f);
                    Animator.SetTrigger("LookUnderAttack");
                }

                HurtCollider.playerDamage = 80;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                if(StateEndedImpulse)
                    Animator.SetBool("LookingUnder", false);

                break;
            }
            case RobeState.GiveSpace:
            {
                if(ConsumeStateImpulse())
                {
                    sfxTargetPlayerLoop.PlayLoop(true, 2f, 2f, 2f);
                    if(Utils.FindNavPosition(EnemyAgent.transform.position + UnityEngine.Random.onUnitSphere * 4, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                NormalRotationLogic();

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(PlayerAggroLogic(aggroTouch: false)) {}
                else if(StateTimer <= 0)
                    SetState(RobeState.Idle, 2f);

                if(StateEndedImpulse)
                    sfxTargetPlayerLoop.PlayLoop(false, 2f, 2f, 2f);

                break;
            }
        }

		transform.rotation = SemiFunc.SpringQuaternionGet(RotationSpring, RotationTarget, -1f);
        
        VisionTriggeredPlayers.Clear();
        VisionImpulse = false; 
        TouchedImpulse = false;
        StateEndedImpulse = false;
        ObjectBreakImpulse = false;
        PlayerAggressiveImpulse = false;

        if(FutureState != RobeState.Invalid)
        {
            State = FutureState;
            FutureState = RobeState.Invalid;
            StateImpulse = true;
        }
    }

    public void ItemBreakTrackingLogic()
    {
        ItemBreakLogicTimer += Time.deltaTime;

        if(ItemBreakLogicTimer < 1f)
            return;

        ItemBreakLogicTimer = 0;

        for(int x = 0; x < TrackedValuables.Count; x++)
            if(TrackedValuables[x] == null)
                TrackedValuables.RemoveAt(x);

        for(int x = 0; x < ValuableDirector.instance.valuableList.Count; x++)
        {
            ValuableObject valuable = ValuableDirector.instance.valuableList[x];

            if(valuable == null)
                continue;

            float distance = Vector3.Distance(valuable.transform.position, transform.position);
            int trackedIndex = TrackedValuables.IndexOf(valuable);
            
            if(distance > 6 && trackedIndex != -1)
            {
                TrackedValuables.RemoveAt(trackedIndex);
                PhysGrabObjectImpactDetector impactDetector = (PhysGrabObjectImpactDetector)valuable
                    .Get<PhysGrabObject, ValuableObject>("physGrabObject")
                    .Get("impactDetector");
                impactDetector.onAllBreaks.RemoveListener(OnObjectBreak);
            }
            else if(distance <= 4 && trackedIndex == -1)
            {
                TrackedValuables.Add(valuable);
                PhysGrabObjectImpactDetector impactDetector = (PhysGrabObjectImpactDetector)valuable
                    .Get<PhysGrabObject, ValuableObject>("physGrabObject")
                    .Get("impactDetector");
                impactDetector.onAllBreaks.AddListener(OnObjectBreak);
            }
        }
    }

    public void PlayerTrackingLogic()
    {
        if(!VisionImpulse)
            return;

        for(int x = 0; x < VisionTriggeredPlayers.Count; x++)
        {
            PlayerAvatar player = VisionTriggeredPlayers[x];

            if(!player.physGrabber.grabbed || player.physGrabber.Get<PhysGrabObject, PhysGrabber>("grabbedPhysGrabObject").GetComponent<ItemAttributes>() is not ItemAttributes itemAttributes)
                continue;

            if (itemAttributes.item.itemType == SemiFunc.itemType.grenade || 
                itemAttributes.item.itemType == SemiFunc.itemType.melee ||
                itemAttributes.item.itemType == SemiFunc.itemType.gun ||
                itemAttributes.item.itemType == SemiFunc.itemType.mine)
            {
                PlayerAggressiveImpulse = true;
                AggressivePlayer = player;
            }
        }
    }

    public bool PlayerAggroLogic(bool aggroTouch = true, bool aggroObjectBreak = true, bool aggroPlayerAggressive = true)
    {
        bool aggro = false;

        if(TouchedImpulse && aggroTouch)
        {
            FocusedPlayer = TouchedPlayer;
            aggro = true;
        }
        else if(PlayerAggressiveImpulse && aggroPlayerAggressive)
        {
            FocusedPlayer = AggressivePlayer;
            aggro = true;
        }
        else if(FocusedPlayer == null) {}
        else if(ObjectBreakImpulse && aggroObjectBreak)
            aggro = true;
        else if(
            RoundDirector.instance.Get("extractionPointCurrent") is ExtractionPoint extraction && 
            extraction.Get<ExtractionPoint.State, ExtractionPoint>("currentState") == ExtractionPoint.State.Warning && 
            Vector3.Distance(extraction.transform.position, transform.position) < 5f)
            aggro = true;

        if(aggro)
        {
            SetState(RobeState.ChaseBegin, UnityEngine.Random.Range(0.75f, 1.25f));
            SyncFields();
            LastFocusedPlayerVisionTimer = 0;
            return true;
        }

        return false;
    }

    public bool LosePlayerLogic()
    {
        if(FocusedPlayer != null && LastFocusedPlayerVisionTimer > 10f)
        {
            FocusedPlayer = null;
            SyncFields();

            return true;
        }

        return false;
    }

    public void NormalRotationLogic()
    {
        Vector3 agentVelocity = EnemyAgent.Get<Vector3, EnemyNavMeshAgent>("AgentVelocity").normalized;
        if (agentVelocity.magnitude > 0.1f)
            LookInDirection(agentVelocity);
    }

    public void OnObjectBreak()
    {
        ObjectBreakImpulse = true;
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

    public void AttackShake()
    {
        GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, transform.position, 0.5f);
        GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, transform.position, 0.1f);
    }

    public void AttackSounds()
    {
        sfxAttack.Play(transform.position, 1f, 1f, 1f, 1f);
        sfxAttackGlobal.Play(transform.position, 1f, 1f, 1f, 1f);
    }

    public void AttackAnimation()
    {
        AttackSounds();
        AttackShake();
        Animator.SetTrigger("attack");
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