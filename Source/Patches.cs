using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;

namespace Ardot.REPO.EnemyOverhaul;

public static class Patches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(AccessTools.Method(typeof(MainMenuOpen), "Start"), postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "GameStart")));
        // Plugin.Harmony.Patch(
        //     AccessTools.Method(typeof(RunManager), "ResetProgress"),
        //     postfix: new HarmonyMethod(typeof(Patches), "ResetProgressPostfix")
        // );
    }

    // public static void ResetProgressPostfix()
    // {
    //     StatsManager.instance.runStats["level"] = 2;
    //     RunManager.instance.levelsCompleted = 2;
    //     RunManager.instance.Set("loadLevel", 2);
    //     RunManager.instance.Set("saveLevel", 0);
    // }

    public static void GameStart()
    {
        Plugin.Enemies = Utils.GetEnemies();

        GnomePatches.GameStart();
    }   
}