using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace Ardot.REPO.REPOverhaul;

[BepInPlugin(PluginGUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public class Plugin : BaseUnityPlugin
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public const string PluginGUID = "Ardot.REPO.REPOverhaul";

    public static Dictionary<string, GameObject> Enemies;
    public static Harmony Harmony;
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        Harmony = new (PluginGUID);
        Patches.Patch();
        HuntsmanPatches.Patch();
        HeadmanPatches.Patch();
        PitsPatches.Patch();
        RobePatches.Patch();
        //DirectorPatches.Patch();
        //TrudgePatches.Patch();
    }
}