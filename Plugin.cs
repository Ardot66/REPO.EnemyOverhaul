using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace Ardot.REPO.REPOverhaul;

[BepInPlugin(PluginGUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGUID = "Ardot.REPO.REPOverhaul";

    public static Dictionary<string, GameObject> Enemies;
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginGUID} is loaded!");

        Harmony harmony = new (PluginGUID);
        harmony.Patch(AccessTools.Method(typeof(StatsManager), "Start"), postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "GameStart")));
        harmony.Patch(AccessTools.Method(typeof(EnemyHunter), "OnTouchPlayerGrabbedObject"), postfix: new HarmonyMethod(typeof(Patches), "HunterOnTouchPlayerGrabbedObjectPostfix"));
        harmony.Patch(AccessTools.Method(typeof(EnemyRigidbody), "OnCollisionStay"), postfix: new HarmonyMethod(typeof(Patches), "EnemyOnCollisionStayPostfix"));
    }
}