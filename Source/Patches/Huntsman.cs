using UnityEngine;
using HarmonyLib;

namespace Ardot.REPO.EnemyOverhaul;

public static class HuntsmanPatches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(AccessTools.Method(typeof(EnemyHunter), "OnTouchPlayerGrabbedObject"), postfix: new HarmonyMethod(typeof(HuntsmanPatches), "HunterOnTouchPlayerGrabbedObjectPostfix"));
        Plugin.Harmony.Patch(AccessTools.Method(typeof(EnemyHunter), "ShootRPC"), postfix: new HarmonyMethod(typeof(HuntsmanPatches), "HunterShootRPCPostFix"));
        Plugin.Harmony.Patch(AccessTools.Method(typeof(EnemyRigidbody), "OnCollisionStay"), postfix: new HarmonyMethod(typeof(HuntsmanPatches), "EnemyOnCollisionStayPostfix"));
    }

    public static void HunterOnTouchPlayerGrabbedObjectPostfix(EnemyHunter __instance)
    {
        // Makes the huntsman follow the object hitting it rather than the player

        AccessTools.Field(typeof(EnemyHunter), "investigatePointHasTransform").SetValue(__instance, false);
    }

    public static void HunterShootRPCPostFix(EnemyHunter __instance, Vector3 _hitPosition)
    {
        // Adds falloff to the hunter's shotgun

        float falloff = Mathf.Min(1f / Mathf.Pow(Vector3.Distance(_hitPosition, __instance.gunTipTransform.position), 3f/2f), 1f/2f);
        __instance.hurtCollider.playerDamage = (int)(300f * falloff);
        __instance.hurtCollider.playerTumbleForce = 30f * falloff;
        __instance.hurtCollider.playerTumbleTorque = 30f * falloff;
        __instance.hurtCollider.playerHitForce = 30f * falloff;
        __instance.hurtCollider.playerTumbleTime = 6 * falloff;
        __instance.hurtCollider.enemyDamage = (int)(300f * falloff);
        __instance.hurtCollider.enemyHitForce = 30f * falloff;
        __instance.hurtCollider.enemyHitTorque = 60f * falloff;
    }

    public static void EnemyOnCollisionStayPostfix(EnemyRigidbody __instance, Collision other)
    {
        // Makes enemies target objects hitting them rather than the player holding the object

        if (other.gameObject.CompareTag("Phys Grab Object"))
        {
            PhysGrabObject physGrabObject = other.gameObject.GetComponent<PhysGrabObject>();
            if (!physGrabObject)
                physGrabObject = other.gameObject.GetComponentInParent<PhysGrabObject>();

            if(physGrabObject && !__instance.enemy.CheckChase() && physGrabObject.playerGrabbing.Count > 0)
                AccessTools.Field(typeof(EnemyRigidbody), "onTouchPlayerGrabbedObjectPosition").SetValue(__instance, other.gameObject.transform.position);
        }
    }
}