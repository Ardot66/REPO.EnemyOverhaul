using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Photon.Pun;
using BepInEx.Configuration;

namespace Ardot.REPO.EnemyOverhaul;

public static class RobeOverhaul
{
    public static ConfigEntry<bool> OverhaulAI;

    public static void Init()
    {
        OverhaulAI = Plugin.Config.Bind(
            "Robe",
            "OverhaulAI",
            true,
            "If true, Robe AI is overhauled"
        );

        if(!OverhaulAI.Value)
            return;

        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobe), "Awake"),
            prefix: new HarmonyMethod(typeof(RobeOverhaul), "AwakePrefix")
        );  
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobePersistent), "Update"),
            prefix: new HarmonyMethod(typeof(RobeOverhaul), "PersistentUpdatePrefix")
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
        Spawn,
        Idle,
        Investigate,
        Shifty,
        Roam,
        FollowPlayer,
        HelpPlayer,
        // RampageBegin,
        // Rampage,
        ChaseBegin,
        Chase,
        Attack,
        AttackUnderBegin,
        AttackUnder,
        GiveSpace,
    }

    public enum RobeAggroMode
    {
        None,
        All,
    }

    public enum RobeFocusMode
    {
        None,
        Player,
    }

    public StateMachine<RobeState> State;

    public float LastFocusedPlayerVisionTimer = float.PositiveInfinity;
    public float ItemBreakLogicTimer = 0f;
    
    public RobeAggroMode AggroMode = RobeAggroMode.All;
    public RobeFocusMode FocusMode = RobeFocusMode.Player;
    public Enemy Enemy;
    public EnemyParent EnemyParent;
    public PhotonView PhotonView;
    public EnemyVision Vision;
    public EnemyRigidbody Rigidbody;
    public Animator Animator;
    public HurtCollider HurtCollider;
    public EnemyNavMeshAgent EnemyAgent;
    public PlayerAvatar FocusedPlayer;
    public AggroHandler Aggro = new ();
    public SpringQuaternion RotationSpring;
    EnemyStateInvestigate StateInvestigate;
    public ValuableObject TargetValuable;
    public record struct TrackedValuable(ValuableObject Valuable, UnityAction BreakListener);
    public List<TrackedValuable> TrackedValuables = new();
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

    [PunRPC]
    public void SetState(RobeState state, float stateTimer)
    {
        bool stateSet = State.SetState(state, stateTimer);

        if(stateSet && SemiFunc.IsMasterClient() && SemiFunc.IsMultiplayer())
            PhotonView.RPC("SetState", RpcTarget.Others, state, stateTimer);
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

    public void Awake()
    {
        Enemy = GetComponent<Enemy>();
        PhotonView = GetComponent<PhotonView>();

        State = new StateMachine<RobeState>(StateMachine);
        State.State = RobeState.Spawn;
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
        StateInvestigate = GetComponent<EnemyStateInvestigate>();

        Vision.onVisionTriggered.AddListener(() => 
        {
            if(FocusMode == RobeFocusMode.Player)
            {
                PlayerAvatar player = (PlayerAvatar)Vision.Get("onVisionTriggeredPlayer");

                if(FocusedPlayer == null)
                {
                    FocusedPlayer = player;
                    LastFocusedPlayerVisionTimer = 0;
                }
                else if(FocusedPlayer == player)
                    LastFocusedPlayerVisionTimer = 0;
            }
            else
                LastFocusedPlayerVisionTimer = float.PositiveInfinity;
                
            if(AggroMode == RobeAggroMode.All)
            {
                PlayerAvatar player = (PlayerAvatar)Vision.Get("onVisionTriggeredPlayer");

                if(player.physGrabber.grabbed && player.physGrabber.Get<PhysGrabObject, PhysGrabber>("grabbedPhysGrabObject").GetComponent<ItemAttributes>() is ItemAttributes itemAttributes)
                {
                    if (itemAttributes.item.itemType == SemiFunc.itemType.grenade || 
                        itemAttributes.item.itemType == SemiFunc.itemType.melee ||
                        itemAttributes.item.itemType == SemiFunc.itemType.gun ||
                        itemAttributes.item.itemType == SemiFunc.itemType.mine)
                    {
                        Aggro.GetAggro(player).Aggro += (float)Vision.Get("VisionCheckTime") * 6;
                    }
                }
            }
        });
        Rigidbody.onGrabbed.AddListener(() =>{
            if(AggroMode != RobeAggroMode.All)
                return;

            Aggro.GetAggro((PlayerAvatar)Rigidbody.Get("onGrabbedPlayerAvatar")).Aggro += Time.deltaTime * 5;
        });
        Rigidbody.onTouchPlayer.AddListener((UnityAction)(() => {
            if(AggroMode != RobeAggroMode.All)
                return;

            Aggro.GetAggro((PlayerAvatar)Rigidbody.Get("onTouchPlayerAvatar")).Aggro += Time.deltaTime * 5;
        }));
        Rigidbody.onTouchPlayerGrabbedObject.AddListener((UnityAction)(() => {
            if(AggroMode != RobeAggroMode.All)
                return;

            Aggro.GetAggro((PlayerAvatar)Rigidbody.Get("onTouchPlayerGrabbedObjectAvatar")).Aggro += Time.deltaTime * 5;
        }));

        health.onHurt.AddListener((UnityAction)(() =>
        {
            sfxHurt.Play(transform.position, 1f, 1f, 1f, 1f);
            if(AggroMode != RobeAggroMode.All)
                return;

            for(int x = 0; x < Aggro.AggroList.Count; x++)
                Aggro.AggroList[x].Aggro += 15;
        }));   
        health.onObjectHurt.AddListener((UnityAction)(() => {
            if(AggroMode != RobeAggroMode.All)
                return;

            Aggro.GetAggro((PlayerAvatar)health.Get("onObjectHurtPlayer")).Aggro += 15;
        }));
        health.onDeath.AddListener(() =>
        {
            Animator.SetTrigger("Death");

            ParticleSystem[] array = deathParticles;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Play();
            }

            if(Utils.IsHost())
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
        StateInvestigate.onInvestigateTriggered.AddListener(() => {
            switch(State.State)
            {
                case RobeState.Idle:
                case RobeState.Roam:
                case RobeState.Spawn:
                {
                    SetState(RobeState.Investigate, Random.Range(4f, 6f));
                    break;
                }
            }
        });
    }

    public void Update()
    {
        if(Enemy.CurrentState == EnemyState.Stunned || Enemy.CurrentState == EnemyState.Despawn)
            return;

        // RampageTimer -= Time.deltaTime;
        LastFocusedPlayerVisionTimer += Time.deltaTime;

        if(SemiFunc.IsMasterClientOrSingleplayer())
        {
            ItemBreakTrackingLogic();
            ExtractionAggroLogic();
        }
        
        AggroLogic();

        State.Update(Time.deltaTime);

        Aggro.LoseAggro(Time.deltaTime);
		transform.rotation = SemiFunc.SpringQuaternionGet(RotationSpring, RotationTarget, -1f);
    }

    public void StateMachine()
    {
        switch (State.State)
        {
            case RobeState.Spawn:
            {
                if(State.ConsumeStateImpulse())
                {
                    SemiFunc.EnemySpawn(Enemy);
                    AggroMode = RobeAggroMode.All;
                    Animator.Play("Robe Spawn", 0, 0f);
                    spawnParticles.Play();
                    //RampageTimer = Random.Range(30f, 60f);
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                if(State.StateTimer <= 0)
                    SetState(RobeState.Idle, 1f);

                break;
            }
            case RobeState.Idle:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    EnemyAgent.ResetPath();
                }

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LosePlayerLogic()) {}
                else if(State.StateTimer <= 0)
                {
                    // if(RampageTimer <= 0)
                    // {
                    //     RampageTimer = Random.Range(30f, 120f);
                    //     SetState(RobeState.RampageBegin, 4f);
                    // }
                    if(FocusedPlayer != null)
                    {
                        float playerDistance = Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f);
    
                        if (FocusedPlayer.physGrabber.grabbed && FocusedPlayer.physGrabber.Get("grabbedPhysGrabObject") is PhysGrabObject playerGrabbedObject && (TargetValuable = playerGrabbedObject.GetComponent<ValuableObject>()) != null)
                        {
                            SetState(RobeState.HelpPlayer, Random.Range(40f, 70f));
                            SyncFields();
                        }
                        else if(playerDistance > 5f || playerDistance < 2.5f)
                            SetState(RobeState.FollowPlayer, Random.Range(10f, 12f));
                        else
                            SetState(RobeState.Shifty, 3f);
                    }
                    else
                        SetState(RobeState.Roam, Random.Range(4f, 9f));
                }

                break;
            }
            case RobeState.Investigate:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    if(Utils.FindNavPosition((Vector3)StateInvestigate.Get("onInvestigateTriggeredPosition"), out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        State.StateStartImpulse = true;
                }

                NormalRotationLogic();

                if(!Utils.IsHost()) {}
                else if(State.StateTimer <= 0)
                    SetState(RobeState.Idle, 1f);

                break;
            }
            case RobeState.Shifty:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    if(SemiFunc.LevelPointGet(transform.position, 1f, 4f) is LevelPoint levelPoint && Utils.FindNavPosition(levelPoint.transform.position + Random.insideUnitSphere, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);

                    OverrideMovement(0.75f, 1f, 1f);
                }

                if(State.StateEndImpulse)
                    EndMovementOverride();

                NormalRotationLogic();

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 3f;
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LosePlayerLogic())
                    SetState(RobeState.Idle, Random.Range(0.25f, 0.5f));
                else if(playerDistance < 2f || playerDistance > 5f || State.StateTimer <= 0)
                    SetState(RobeState.Idle, 1f);
                    
                break;
            }
            case RobeState.Roam:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    if(SemiFunc.LevelPointGet(transform.position, 3f, 10f) is LevelPoint levelPoint && Utils.FindNavPosition(levelPoint.transform.position + Random.insideUnitSphere, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                }

                NormalRotationLogic();

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(FocusedPlayer != null)
                    SetState(RobeState.FollowPlayer, 3f);
                else if(!EnemyAgent.HasPath() || State.StateTimer <= 0)
                    SetState(RobeState.Idle, Random.Range(1f, 2f));

                break;
            }
            case RobeState.FollowPlayer:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    if(FocusedPlayer != null && Utils.FindNavPosition(FocusedPlayer.transform.position + (transform.position - FocusedPlayer.transform.position).normalized * 3 + Random.insideUnitSphere * 0.5f, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        State.StateStartImpulse = true;

                    OverrideMovement(2f, 6f, 20f);
                }

                if(State.StateEndImpulse)
                    EndMovementOverride();

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 3f;
                
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LosePlayerLogic())
                    SetState(RobeState.Idle, Random.Range(0.25f, 0.5f));
                else if (playerDistance > 10f)
                    SetState(RobeState.ChaseBegin, Random.Range(0.25f, 0.5f));
                else if((playerDistance < 3.5f && playerDistance > 2.5f) || !EnemyAgent.HasPath() || State.StateTimer <= 0)
                    SetState(RobeState.Idle, 0f);

                break;
            }
            case RobeState.HelpPlayer:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    sfxHandIdle.PlayLoop(true, 2f, 2f, 0.4f);
                    OverrideMovement(1f, 2f, 3f);
                    EnemyAgent.ResetPath();
                    State.SetStateTimer(0, 0f);
                }

                if(State.StateEndImpulse)
                {
                    sfxHandIdle.PlayLoop(false, 2f, 2f, 0.4f);
                    EndMovementOverride();
                }

                if(TargetValuable != null)
                {
                    LookAt(TargetValuable.transform.position);
                    Rigidbody targetValuableRigidbody = (Rigidbody)TargetValuable.Get("rb");

                    float distanceToValuable = Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f);;

                    if(distanceToValuable < 4.5f)
                        targetValuableRigidbody.AddForce(new Vector3(0, Mathf.Min(targetValuableRigidbody.mass, 5), 0), ForceMode.Force);
                
                    if((distanceToValuable < 1.8f || distanceToValuable > 2.6f) && State.GetStateTimer(0) <= 0)
                    {
                        State.SetStateTimer(0, 0.1f);
                        Utils.FindNavPosition(TargetValuable.transform.position + (transform.position - TargetValuable.transform.position).normalized * 1.5f, out Vector3 navPosition, 3f);
                        EnemyAgent.SetDestination(navPosition);
                    }
                }

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(State.StateTimer <= 0 || (FocusedPlayer != null && !FocusedPlayer.physGrabber.grabbed))
                    SetState(RobeState.Idle, 1f);

                break;
            }
            // case RobeState.RampageBegin:
            // {
            //     if(State.ConsumeStateImpulse())
            //     {
            //         sfxTargetPlayerLoop.PlayLoop(true, 1f, 1f);
            //         sfxHandAggressive.PlayLoop(true, 1f, 1f);
            //         FocusMode = RobeFocusMode.None;
            //         AggroMode = RobeAggroMode.None;
            //         EnemyAgent.ResetPath();
            //     }

            //     if(Utils.IsHost())
            //     {
            //         if(State.StateTimer <= 0)
            //         {
            //             FocusedPlayer = null;
            //             SyncFields();
            //             SetState(RobeState.Rampage, Random.Range(10f, 20f));
            //         }
            //     }

            //     if(State.StateEndImpulse)
            //     {
            //         sfxTargetPlayerLoop.PlayLoop(false, 1f, 1f);
            //         sfxHandAggressive.PlayLoop(false, 1f, 1f);
            //     }

            //     break;
            // }
            // case RobeState.Rampage:
            // {
            //     if(State.ConsumeStateImpulse())
            //     {
            //         AggroMode = RobeAggroMode.None;
            //         StateInternalTimer = 0;
            //         OverrideMovement(4, 10, StateTimer);
            //     }

            //     NormalRotationLogic();

            //     if(StateInternalTimer <= 0)
            //     {
            //         StateInternalTimer = Random.Range(1f, 3f);
            //         AttackAnimation();
            //         if(SemiFunc.LevelPointGet(transform.position, 3f, 10f) is LevelPoint levelPoint && Utils.FindNavPosition(levelPoint.transform.position + Random.insideUnitSphere, out Vector3 navPosition))
            //             EnemyAgent.SetDestination(navPosition);
            //     }

            //     if(Utils.IsHost())
            //     {
            //         if(State.StateTimer <= 0)
            //             SetState(RobeState.Idle, 1f);
            //     }

            //     if(State.StateEndImpulse)
            //     {
            //         EndMovementOverride();
            //         FocusMode = RobeFocusMode.Player;
            //     }

            //     break;
            // }
            case RobeState.ChaseBegin:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.None;
                    HurtCollider.enabled = false;
                    EnemyAgent.ResetPath();
                    AttackAnimation();
                }
                
                if(State.StateEndImpulse)
                    HurtCollider.enabled = true;

                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);
                    
                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}    
                else if(State.StateTimer <= 0)
                    SetState(RobeState.Chase, Random.Range(10f, 14f));

                break;
            }
            case RobeState.Chase:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.None;
                    sfxTargetPlayerLoop.PlayLoop(true, 2f, 2f, 2f);
                    OverrideMovement(6, 10, 20f);
                    State.SetStateTimer(0, 0f);
                }

                if(State.StateEndImpulse)
                {
                    EndMovementOverride();
                    sfxTargetPlayerLoop.PlayLoop(false, 2f, 2f, 2f);

                    Aggro.LoseAggro(float.PositiveInfinity);
                }

                NormalRotationLogic();

                if(State.GetStateTimer(0) <= 0)
                {
                    State.SetStateTimer(0, 0.1f);
                    if(FocusedPlayer != null && Utils.FindNavPosition(FocusedPlayer.transform.position, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                }

                float playerDistance = FocusedPlayer ? Utils.WeightedDistance(transform.position, FocusedPlayer.transform.position, y: 0.2f) : 10f;
                bool playerCrawling = FocusedPlayer ? (bool)FocusedPlayer.Get("isCrawling") : false;

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(LastFocusedPlayerVisionTimer > 8f)
                { 
                    FocusedPlayer = null;
                    SyncFields();
                    SetState(RobeState.GiveSpace, 2f);
                }
                else if(playerDistance < 1.5f && !playerCrawling)
                    SetState(RobeState.Attack, 1f);
                else if(playerDistance < 2f && playerCrawling)
                    SetState(RobeState.AttackUnderBegin, 0.5f);
                else if(State.StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);
                
                break;
            }
            case RobeState.Attack:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.None;
                    AttackAnimation();
                    EnemyAgent.ResetPath();
                }

                HurtCollider.playerDamage = 120;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(State.StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                break;
            }
            case RobeState.AttackUnderBegin:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.None;
                    EnemyAgent.ResetPath();
                    Animator.SetTrigger("LookUnder");
                    Animator.SetBool("LookingUnder", true);
                }

                HurtCollider.playerDamage = 80;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                if(State.StateTimer <= 0)
                    SetState(RobeState.AttackUnder, 0.5f);

                break;
            }
            case RobeState.AttackUnder:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.None;
                    AttackShake();
                    sfxAttackUnder.Play(transform.position, 1f, 1f, 1f, 1f);
		            sfxAttackUnderGlobal.Play(transform.position, 1f, 1f, 1f, 1f);
                    Animator.SetTrigger("LookUnderAttack");
                }

                if(State.StateEndImpulse)
                    Animator.SetBool("LookingUnder", false);

                HurtCollider.playerDamage = 80;
                if(FocusedPlayer != null)
                    LookAt(FocusedPlayer.transform.position);

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(State.StateTimer <= 0)
                    SetState(RobeState.GiveSpace, 2f);

                break;
            }
            case RobeState.GiveSpace:
            {
                if(State.ConsumeStateImpulse())
                {
                    AggroMode = RobeAggroMode.All;
                    sfxTargetPlayerLoop.PlayLoop(true, 2f, 2f, 2f);
                    if(Utils.FindNavPosition(EnemyAgent.transform.position + Random.onUnitSphere * 4, out Vector3 navPosition))
                        EnemyAgent.SetDestination(navPosition);
                    else
                        State.StateStartImpulse = true;
                }

                if(State.StateEndImpulse)
                    sfxTargetPlayerLoop.PlayLoop(false, 2f, 2f, 2f);

                NormalRotationLogic();

                if(!SemiFunc.IsMasterClientOrSingleplayer()) {}
                else if(State.StateTimer <= 0)
                    SetState(RobeState.Idle, 2f);

                break;
            }
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

            int trackedIndex = -1;
            for (int y = 0; y < TrackedValuables.Count; y++)
                if (TrackedValuables[y].Valuable == valuable)
                    trackedIndex = y;
            
            if(distance > 6 && trackedIndex != -1)
            {
                PhysGrabObjectImpactDetector impactDetector = (PhysGrabObjectImpactDetector)valuable
                    .Get<PhysGrabObject, ValuableObject>("physGrabObject")
                    .Get("impactDetector");
                impactDetector.onAllBreaks.RemoveListener(TrackedValuables[trackedIndex].BreakListener);
                TrackedValuables.RemoveAt(trackedIndex);
            }
            else if(distance <= 4 && trackedIndex == -1)
            {
                PhysGrabObject physGrabObject = (PhysGrabObject)valuable.Get("physGrabObject");
                PhysGrabObjectImpactDetector impactDetector = (PhysGrabObjectImpactDetector)physGrabObject.Get("impactDetector");

                UnityAction listener = new(() =>
                {
                    if (AggroMode != RobeAggroMode.All || (bool)impactDetector.Get("isIndestructible") || impactDetector.destroyDisable || (float)physGrabObject.Get("grabbedTimer") <= 0)
                        return;

                    PlayerAggro aggro = Aggro.GetAggro((PlayerAvatar)physGrabObject.Get("lastPlayerGrabbing"));
                    aggro.Aggro += physGrabObject.dead ? valuable.dollarValueOriginal / 100 : (valuable.dollarValueOriginal - valuable.dollarValueCurrent) / 100;
                });

                TrackedValuables.Add(new (valuable, listener));
                impactDetector.onAllBreaks.AddListener(listener);
            }
        }
    }

    public void AggroLogic()
    {
        if(AggroMode == RobeAggroMode.None)
            return;

        PlayerAggro mostAggressive = null;

        for(int x = 0; x < Aggro.AggroList.Count; x++)
        {
            PlayerAggro aggro = Aggro.AggroList[x];

            if(mostAggressive == null || aggro.Aggro > mostAggressive.Aggro)
                mostAggressive = aggro;
        }

        if(mostAggressive == null)   
        {
            sfxTargetPlayerLoop.PlayLoop(false, 1f, 1f);
            return;
        }

        if(mostAggressive.Aggro > 5f)
            sfxTargetPlayerLoop.PlayLoop(true, 1f, 1f);
        else
            sfxTargetPlayerLoop.PlayLoop(false, 1f, 1f);

        if(mostAggressive.Aggro < 10f)
            return;

        sfxTargetPlayerLoop.PlayLoop(false, 1f, 1f);

        if(Utils.IsHost() && State.State != RobeState.ChaseBegin)
        {
            LastFocusedPlayerVisionTimer = 0;
            FocusedPlayer = mostAggressive.Player;
            SyncFields();

            SetState(RobeState.ChaseBegin, 1f);
        }
    }

    public void ExtractionAggroLogic()
    {
        if(FocusedPlayer != null)
        {
            if(
                RoundDirector.instance.Get("extractionPointCurrent") is ExtractionPoint extraction && 
                extraction.Get<ExtractionPoint.State, ExtractionPoint>("currentState") == ExtractionPoint.State.Warning && 
                Vector3.Distance(extraction.transform.position, transform.position) < 5f)
            {
                Aggro.GetAggro(FocusedPlayer).Aggro += Time.deltaTime * 10;
            }
        }
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

    public void OverrideMovement(float speed, float acceleration, float rigidbodyAcceleration)
    {
        EnemyAgent.OverrideAgent(speed, acceleration, float.PositiveInfinity);
        Rigidbody.OverrideFollowPosition(float.PositiveInfinity, rigidbodyAcceleration);
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