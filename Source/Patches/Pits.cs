using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Ardot.REPO.REPOverhaul;

public static class PitsPatches
{
    public const int
        MapHurtCollider = 0;

    public static void Patch()
    {
        // Plugin.Harmony.Patch(
        //     AccessTools.Method(typeof(LevelGenerator), "Start"),    
        //     prefix: new HarmonyMethod(typeof(PitsPatches), "StartPrefix")
        // );
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(LevelGenerator), "GenerateDone"),    
            postfix: new HarmonyMethod(typeof(PitsPatches), "GenerateDonePostfix")
        );
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(HurtCollider), "PlayerHurt"),
            prefix: new HarmonyMethod(typeof(PitsPatches), "PlayerHurtPrefix")
        );
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(HurtCollider), "EnemyHurt"),
            prefix: new HarmonyMethod(typeof(PitsPatches), "EnemyHurtPrefix")
        );
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(HurtCollider), "PhysObjectHurt"),
            prefix: new HarmonyMethod(typeof(PitsPatches), "PhysObjectHurtPrefix")
        );
    }

    // public static bool StartPrefix(LevelGenerator __instance)
    // {
    //     // DEBUG

    //     Level level = RunManager.instance.levelCurrent;
    //     level.ModulesNormal1 = level.ModulesNormal3;
    //     level.ModulesPassage1 = level.ModulesPassage3;
    //     level.ModulesExtraction1 = level.ModulesExtraction3;

    //     // DEBUG

    //     return true;
    // }

    public static void GenerateDonePostfix(LevelGenerator __instance)
    {
        List<HurtCollider> mapHurtColliders = Utils.GetHurtColliders(__instance.LevelParent.transform);

        for(int x = 0; x < mapHurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = mapHurtColliders[x];

            if(hurtCollider.name != "Kill Box")
                continue;

            hurtCollider.SetMetadata(MapHurtCollider, true);
            hurtCollider.physDestroy = false;
        }
    }

    public static bool PlayerHurtPrefix(HurtCollider __instance, PlayerAvatar _player)
    {
        if(!__instance.GetMetadata(MapHurtCollider, false))
            return true;

        __instance.onImpactAny.Invoke();
		__instance.onImpactPlayer.Invoke();

        LevelPoint destination = ChooseLevelPoint(_player.transform.position, 40, 0.05f);

        Vector3 finalPosition = destination.transform.position + new Vector3(0, 1, 0);
        _player.Spawn(finalPosition, _player.transform.rotation);
        Rigidbody tumbleRB = (Rigidbody)_player.tumble.Get("rb");
        tumbleRB.position = finalPosition;
        tumbleRB.transform.position = finalPosition;

        PlayerController.instance.CollisionController.Set("fallDistance", 0); 
        _player.tumble.TumbleRequest(true, false);
        _player.tumble.TumbleOverrideTime(3f);
        _player.tumble.ImpactHurtSet(0.5f, Random.Range(35, 45));

        return false;
    }

    public static bool EnemyHurtPrefix(HurtCollider __instance, Enemy _enemy)
    {
        if(!__instance.GetMetadata(MapHurtCollider, false))
            return true;

        __instance.onImpactAny.Invoke();
        __instance.onImpactEnemy.Invoke();

        LevelPoint destination = ChooseLevelPoint(_enemy.transform.position, 40, 0.05f);
        Vector3 finalPosition = destination.transform.position + new Vector3(0, 1, 0);

        _enemy.EnemyTeleported(finalPosition);
        EnemyHealth health = (EnemyHealth)_enemy.Get("Health");
        if(health != null)
            health.Hurt(Random.Range(70, 100), __instance.transform.forward);
        else if((bool)_enemy.Get("HasStateDespawn"))
        {
            _enemy.Get<EnemyParent, Enemy>("EnemyParent").SpawnedTimerSet(0f);
            _enemy.CurrentState = EnemyState.Despawn;
        }

        EnemyStateStunned stunnedState = (EnemyStateStunned)_enemy.Get("StateStunned");
        if (stunnedState != null)
            stunnedState.Set(5f);

        return false;
    }

    public static bool PhysObjectHurtPrefix(ref bool __result, HurtCollider __instance, PhysGrabObject physGrabObject)
    {
        if(!__instance.GetMetadata(MapHurtCollider, false))
            return true;

        LevelPoint destination = ChooseLevelPoint(physGrabObject.transform.position, 40, 0.05f);
        Vector3 finalPosition = destination.transform.position + new Vector3(0, 1, 0);

        physGrabObject.Teleport(finalPosition, physGrabObject.transform.rotation);

        __result = true;
        return false;
    }

    public static LevelPoint ChooseLevelPoint(Vector3 position, float idealRadius, float truckDistanceMultiplier)
    {
        List<LevelPoint> levelPoints = LevelGenerator.Instance.LevelPathPoints;
        float[] levelPointScores = new float [levelPoints.Count];
        float totalScore = 0;

        for(int x = 0; x < levelPoints.Count; x++)
        {
            LevelPoint levelPoint = levelPoints[x];
            float distance = Mathf.Abs(idealRadius - Vector2.Distance(new Vector2(levelPoint.transform.position.x, levelPoint.transform.position.z), new Vector2(position.x, position.z)));
            
            float score = Mathf.Min(distance < 5 ? 1 : 1 / (distance * distance), 1);
            score *= Vector3.Distance(LevelGenerator.Instance.LevelParent.transform.position, levelPoint.transform.position) * truckDistanceMultiplier;
            levelPointScores[x] = score;
            totalScore += score;
        }

        float chosen = Random.Range(0, totalScore);
        LevelPoint destination = levelPoints[levelPoints.Count - 1];
        for(int x = 0; x < levelPoints.Count; x++)
        {
            chosen -= levelPointScores[x];
            if(chosen <= 0)
            {
                destination = levelPoints[x];
                break;
            }
        }

        return destination;
    }
}