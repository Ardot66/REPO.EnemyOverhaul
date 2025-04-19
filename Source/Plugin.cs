using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace Ardot.REPO.EnemyOverhaul;

[BepInPlugin(PluginGUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public class Plugin : BaseUnityPlugin
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public const string PluginGUID = "Ardot.REPO.EnemyOverhaul";

    public new static ConfigFile Config;
    public static Harmony Harmony;
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;
        Harmony = new (PluginGUID);

        HuntsmanOverhaul.Init();
        TrudgeOverhaul.Init();
        HeadmanOverhaul.Init();
        PitOverhaul.Init();
        RobeOverhaul.Init();
        OverhaulDirector.Init();
    }
}