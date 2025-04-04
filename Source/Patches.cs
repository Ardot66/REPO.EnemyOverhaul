using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

namespace Ardot.REPO.REPOverhaul;

public static class Patches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(AccessTools.Method(typeof(StatsManager), "Start"), postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "GameStart")));
    }

    public static void GameStart()
    {
        Plugin.Enemies = Utils.GetEnemies();

        GnomePatches.GameStart();
        ShopPatches.GameStart();
    }   
}