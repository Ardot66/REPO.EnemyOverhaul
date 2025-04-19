using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using HarmonyLib;

namespace Ardot.REPO.EnemyOverhaul;

public static class GnomeOverhaul
{
    public static ConfigEntry<bool> OverhaulItemDamage;

    public static void Init()
    {
        OverhaulItemDamage = Plugin.BindConfig(
            "Gnome",
            "OverhaulItemDamage",
            true,
            "If true, significantly reduces the damage that Gnomes do to items",
            () => Plugin.SetPatch(
                OverhaulItemDamage.Value,
                AccessTools.Method(typeof(EnemyGnome), "Start"),
                postfix: new HarmonyMethod(typeof(GnomeOverhaul), "StartPostfix")
            ) 
        );
    }

    public static void StartPostfix(EnemyGnome __instance)
    {
        List<HurtCollider> gnomeHurtColliders = Utils.GetHurtColliders(__instance.enemy.GetComponentInParent<EnemyParent>().transform);

        for(int x = 0; x < gnomeHurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = gnomeHurtColliders[x];
            hurtCollider.physImpact = HurtCollider.BreakImpact.Light;
        }
    }
}