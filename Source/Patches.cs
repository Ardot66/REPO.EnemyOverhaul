using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

namespace Ardot.REPO.REPOverhaul;

public static class Patches
{
    struct ItemFix(string name, Value value = null, int maxAmount = -1, int maxAmountInShop = -1)
    {
        public string Name = name;
        public Value Value = value;
        public int MaxAmount = maxAmount;
        public int MaxAmountInShop = maxAmountInShop;
    }

    public static void GameStart()
    {
        Plugin.Enemies = Utils.GetEnemies();

        // Reduces the damage gnomes do to items to a more reasonable amount

        GameObject gnome = Plugin.Enemies["gnome"];
        List<HurtCollider> gnomeHurtColliders = Utils.GetHurtColliders(gnome);

        for(int x = 0; x < gnomeHurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = gnomeHurtColliders[x];
            hurtCollider.physImpact = HurtCollider.BreakImpact.Light;
        }

        // Disable power crystal and health pack cost scaling

        AccessTools.Field(typeof(ShopManager), "crystalValueIncrease").SetValue(ShopManager.instance, 0f);
        AccessTools.Field(typeof(ShopManager), "healthPackValueIncrease").SetValue(ShopManager.instance, 0f);

        // Balance the cost of certain items in the shop

        ItemFix[] itemFixes =
        [
            new ("EnergyCrystal", value: Utils.Value(600f, 1000f)),
            new ("FeatherDrone", value: Utils.Value(8000f, 12000f)),
            new ("")
        ];

        for(int x = 0; x < itemFixes.Length; x++)
        {
            ItemFix fix = itemFixes[x];

            if(!StatsManager.instance.itemDictionary.TryGetValue(fix.Name, out Item item))
                Plugin.Logger.LogWarning($"Item {fix.Name} not found while applying item fixes");

            if(fix.Value != null)
                item.value = fix.Value;
            if(fix.MaxAmount != -1)
                item.maxAmount = fix.MaxAmount;
            if(fix.MaxAmountInShop != -1)
                item.maxAmountInShop = fix.MaxAmountInShop;
        }
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