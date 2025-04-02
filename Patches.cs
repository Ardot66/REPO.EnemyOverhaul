using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

namespace Ardot.REPO.REPOverhaul;

public static class Patches
{
    public static void GameStart()
    {
        Plugin.Enemies = Utils.GetEnemies();

        GameObject gnome = Plugin.Enemies["gnome"];
        List<HurtCollider> gnomeHurtColliders = Utils.GetHurtColliders(gnome);

        for(int x = 0; x < gnomeHurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = gnomeHurtColliders[x];
            hurtCollider.physImpact = HurtCollider.BreakImpact.Light;
        }
    }   

    public static void HunterOnTouchPlayerGrabbedObjectPostfix(EnemyHunter __instance)
    {
        AccessTools.Field(typeof(EnemyHunter), "investigatePointHasTransform").SetValue(__instance, false);
    }

    public static void EnemyOnCollisionStayPostfix(EnemyRigidbody __instance, Collision other)
    {
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