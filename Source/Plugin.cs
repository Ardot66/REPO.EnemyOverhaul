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
    public static Harmony Harmony;
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginGUID} is loaded!");

        Harmony = new (PluginGUID);
        Patches.Patch();
        HuntsmanPatches.Patch();
    }
}