using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
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
        __instance.gameObject.AddComponent<RobeOverride>();
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
        Chase,
        Attack,
    }

    public RobeState State = RobeState.Idle;
    public float StateTimer = 0f;
    public float LastVisionTimer = float.PositiveInfinity;
    public bool VisionImpulse = false;
    public bool StateImpulse = true;
    public Enemy Enemy;
    public PhotonView PhotonView;
    public PlayerAvatar FollowPlayer;

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

        Plugin.Logger.LogInfo($"{State}");
        StateImpulse = true;
        State = state;
        StateTimer = stateTimer;

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
        EnemyVision vision = (EnemyVision)Enemy.Get("Vision");
        vision.onVisionTriggered.AddListener(() => VisionImpulse = true);
    }

    public void Update()
    {
        EnemyNavMeshAgent enemyAgent = (EnemyNavMeshAgent)Enemy.Get("NavMeshAgent");
        EnemyVision vision = (EnemyVision)Enemy.Get("Vision");
        StateTimer -= Time.deltaTime;
        LastVisionTimer += Time.deltaTime;

        

        switch (State)
        {
            case RobeState.Idle:
            {
                if(ConsumeStateImpulse())
                    enemyAgent.ResetPath();

                if(StateTimer <= 0)
                {
                    if(FollowPlayer != null)
                        SetState(RobeState.FollowPlayer, 5f);
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
                        enemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                if(VisionImpulse)
                {
                    LastVisionTimer = 0f;
                    FollowPlayer = (PlayerAvatar)vision.Get("onVisionTriggeredPlayer");
                    SetState(RobeState.FollowPlayer, 5f);
                }

                if(!enemyAgent.HasPath() || StateTimer <= 0)
                    SetState(RobeState.Idle, Random.Range(1f, 2f));

                break;
            }
            case RobeState.FollowPlayer:
            {
                if(ConsumeStateImpulse())
                {
                    if(Utils.FindNavPosition(FollowPlayer.transform.position + Random.onUnitSphere * 3, out Vector3 navPosition))
                        enemyAgent.SetDestination(navPosition);
                    else
                        StateImpulse = true;
                }

                if(StateTimer <= 0)
                {
                    SetState(RobeState.Idle, Random.Range(0.5f, 1f));
                }

                break;
            }
        }

        VisionImpulse = false; 
    }
}