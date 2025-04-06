using UnityEngine;
using UnityEngine.AI;
using HarmonyLib;
using System.Collections.Generic;

namespace Ardot.REPO.REPOverhaul;

public static class HeadmanPatches
{
    const int 
    VisionTimerMeta = 0,
    LastVisionTime = 1,
    State = 2,
    Started = 3;

    private enum HeadmanState
    {
        Roaming,
        Interested,
        Annoyed,
        VeryAnnoyed,
        Pissed
    }

    public static void Patch()
    {
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyHeadController), "VisionTriggered"), 
            prefix: new HarmonyMethod(typeof(HeadmanPatches), "VisionTriggeredPrefix")
        );
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(EnemyHeadController), "Update"),
            postfix: new HarmonyMethod(typeof(HeadmanPatches), "UpdatePostfix")
        );
    }

    public static void Start(EnemyHeadController instance, Enemy enemy)
    {
        List<HurtCollider> hurtColliders = Utils.GetHurtColliders(enemy.Get<EnemyParent, Enemy>("EnemyParent").transform);
        for(int x = 0; x < hurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = hurtColliders[x];
            hurtCollider.playerDamage = 100;
        }
    }

    public static void UpdatePostfix(EnemyHeadController __instance, Enemy ___Enemy)
    {
        if(!__instance.GetMetadata(Started, false)) 
        {
            __instance.SetMetadata(Started, true);
            Start(__instance, ___Enemy);
        }

        HeadmanState state = __instance.GetMetadata(State, HeadmanState.Roaming);
        float visionTimer = __instance.GetMetadata<float>(VisionTimerMeta);

        EnemyVision vision = (EnemyVision)___Enemy.Get("Vision");
        PlayerAvatar visionPlayer = vision.Get<PlayerAvatar, EnemyVision>("onVisionTriggeredPlayer");
        EnemyStateInvestigate investigate = __instance.GetComponent<EnemyStateInvestigate>();

        switch(state)
        {
            case HeadmanState.Roaming: 
                if(visionTimer > 0.1f)
                {
                    investigate.Set(Vector3.Lerp(__instance.transform.position, visionPlayer.playerTransform.position, 0.2f));
                    state = HeadmanState.Interested;
                }
                break;
            case HeadmanState.Interested:
                if(visionTimer > 0.2f)
                {
                    investigate.Set(Vector3.Lerp(__instance.transform.position, visionPlayer.playerTransform.position, 0.2f));
                    AccessTools.Method(typeof(EnemyHeadAnimationSystem), "IdleTeeth").Invoke(__instance.AnimationSystem, []);
                    state = HeadmanState.Annoyed;
                }
                if(visionTimer < 0.1f)
                    state = HeadmanState.Roaming;
                break;
            case HeadmanState.Annoyed:
                if(visionTimer > 0.4f)
                {
                    investigate.Set(Vector3.Lerp(__instance.transform.position, visionPlayer.playerTransform.position, 0.2f));
                    AccessTools.Method(typeof(EnemyHeadAnimationSystem), "IdleTeeth").Invoke(__instance.AnimationSystem, []);
                    state = HeadmanState.VeryAnnoyed;
                }
                if(visionTimer < 0.2f)
                    state = HeadmanState.Interested;
                break;
            case HeadmanState.VeryAnnoyed:
                if(visionTimer > 0.7f)
                    state = HeadmanState.Pissed;
                if(visionTimer < 0.4f)
                    state = HeadmanState.Pissed;
                break;
            case HeadmanState.Pissed:
                if(visionTimer < 0.5f)
                    state = HeadmanState.Roaming;
                break;
        }

        __instance.SetMetadata(State, state);
        float lastVisionTime = __instance.GetMetadata<float>(LastVisionTime);

        if(Time.time - lastVisionTime < 0.5f)   
            return;

        visionTimer = Mathf.Max(visionTimer - 0.2f * Time.deltaTime, 0);
        __instance.SetMetadata(VisionTimerMeta, visionTimer);
    }

    public static bool VisionTriggeredPrefix(EnemyHeadController __instance, Enemy ___Enemy)
    {
        EnemyVision vision = (EnemyVision)___Enemy.Get("Vision");

        PlayerAvatar visionPlayer = vision.Get<PlayerAvatar, EnemyVision>("onVisionTriggeredPlayer");
        ___Enemy.Set("TargetPlayerViewID", visionPlayer.photonView.ViewID);
        ___Enemy.Set("TargetPlayerAvatar", visionPlayer);

        PhysGrabber playerGrabber = visionPlayer.physGrabber;
        Rigidbody grabbedObject = playerGrabber.Get<Rigidbody, PhysGrabber>("grabbedObject");
        if(grabbedObject == null || grabbedObject.GetComponent<ValuableObject>() is not ValuableObject valuable)
            return false;

		if ((float)___Enemy.Get("DisableChaseTimer") > 0f)
			return false;

        HeadmanState state = __instance.GetMetadata(State, HeadmanState.Roaming);
        float visionTimer = __instance.GetMetadata<float>(VisionTimerMeta);        
        float visionCheckTime = (float)vision.Get("VisionCheckTime");

        visionTimer = Mathf.Min(visionTimer + visionCheckTime * (valuable.dollarValueCurrent / 1500) / Vector3.Distance(visionPlayer.playerTransform.position, __instance.transform.position), 1f);
        __instance.SetMetadata(LastVisionTime, Time.time);
        __instance.SetMetadata(VisionTimerMeta, visionTimer);

        if(state != HeadmanState.Pissed)
            return false;

		if (___Enemy.CurrentState != EnemyState.Chase && ___Enemy.CurrentState != EnemyState.LookUnder)
		{
			if (___Enemy.CurrentState == EnemyState.ChaseSlow)
				___Enemy.CurrentState = EnemyState.Chase;
			else if ((bool)vision.Get("onVisionTriggeredCulled") && !(bool)vision.Get("onVisionTriggeredNear"))
			{
				if (___Enemy.CurrentState != EnemyState.Sneak)
				{
					if (Random.Range(0f, 100f) <= 30f)
						___Enemy.CurrentState = EnemyState.ChaseBegin;
					else
						___Enemy.CurrentState = EnemyState.Sneak;
				}
			}
			else if ((float)vision.Get("onVisionTriggeredDistance") >= 7f)
			{
				___Enemy.CurrentState = EnemyState.Chase;
				___Enemy.Get<EnemyStateChase, Enemy>("StateChase").ChaseCanReach = true;
			}
			else
				___Enemy.CurrentState = EnemyState.ChaseBegin;
		}

        return false;
    }
}