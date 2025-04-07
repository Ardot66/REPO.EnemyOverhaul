using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;

namespace Ardot.REPO.REPOverhaul;

public static class Patches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(AccessTools.Method(typeof(MainMenuOpen), "Start"), postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "GameStart")));
    }

    public static void GameStart()
    {
        Plugin.Enemies = Utils.GetEnemies();

        GnomePatches.GameStart();
        ShopPatches.GameStart();

        // RunManager.instance.ResetProgress();
        // SemiFunc.SaveFileCreate();
        // GameManager.instance.localTest = false;
		// RunManager.instance.Set("waitToChangeScene", true);
		// MainMenuOpen.instance.NetworkConnect();
		// SteamManager.instance.LockLobby();
		// DataDirector.instance.RunsPlayedAdd();
        // RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
    }   
}