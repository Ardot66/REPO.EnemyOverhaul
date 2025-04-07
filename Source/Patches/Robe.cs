using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace Ardot.REPO.REPOverhaul;

public static class RobePatches
{
    public const int
        RobeDataMeta = 0;

    public enum RobeState
    {
        Idle,
        Roam,
        FollowPlayer,
        HelpPlayer,
        Chase,
        Attack,
    }

    public class RobeData
    {
        public RobeState State = RobeState.Idle;
        public float StateTimer = 0f;
        public bool VisionImpulse = false;
        private bool _stateImpulse = true;

        public bool StateImpulse()
        {
            bool started = _stateImpulse;
            _stateImpulse = false;
            return started;
        }

        public void SetState(RobeState state, float stateTimer)
        {
            if (state == State || !SemiFunc.IsMasterClientOrSingleplayer())
                return;

            _stateImpulse = true;
            State = state;
            StateTimer = stateTimer;
        }

        public void SetStateRPCPrefix(RobeState state)
        {
            
        }
    }

    public static void Patch ()
    {
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobe), "Update"),
            prefix: new HarmonyMethod(typeof(RobePatches), "UpdatePrefix")
        );  
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyRobe), "OnVision"),
            prefix: new HarmonyMethod(typeof(RobePatches), "OnVisionPrefix")
        );
    }

    public static bool OnVisionPrefix(EnemyRobe __instance)
    {
        RobeData robe = __instance.GetMetadata<RobeData>(RobeDataMeta, null);
        robe.VisionImpulse = true;

        return false;
    }

    public static bool UpdatePrefix(EnemyRobe __instance, Enemy ___enemy)
    {
        RobeData data = __instance.GetMetadata<RobeData>(RobeDataMeta, null);
        
        if(data == null)
            data = new RobeData ();

        NavMeshAgent agent = (NavMeshAgent)___enemy.Get("NavMeshAgent");
        Rigidbody rigidbody = (Rigidbody)___enemy.Get("Rigidbody");
        data.StateTimer -= Time.deltaTime;

        switch (data.State)
        {
            case RobeState.Idle:
            {
                if(data.StateImpulse())
                    agent.ResetPath();

                if(data.StateTimer <= 0)
                    data.SetState(RobeState.Roam, Random.Range(1f, 4f));
                break;
            }
            case RobeState.Roam:
            {
                if(data.StateImpulse())
                {
                    LevelPoint levelPoint = SemiFunc.LevelPointGet(__instance.transform.position, 5f, 15f) ?? SemiFunc.LevelPointGet(__instance.transform.position, 0f, 999f);
                    
                    NavMeshHit navMeshHit;
                    if (levelPoint != null && 
                        NavMesh.SamplePosition(levelPoint.transform.position + Random.insideUnitSphere * 3f, out navMeshHit, 5f, -1) && 
                        Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(["Default"]))
                    )
                        agent.SetDestination(navMeshHit.position);
                }

                if(agent.pathStatus == NavMeshPathStatus.PathComplete || data.StateTimer <= 0)
                    data.SetState(RobeState.Idle, Random.Range(2f, 4f));

                break;
            }
        }

        data.VisionImpulse = false; 

        return false;
    }
}