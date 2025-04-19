using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    public struct PluginConfigEntry(ConfigEntryBase entry, Action changed)
    {
        public ConfigEntryBase Entry = entry;
        public Action Changed = changed;
    }

    public static List<PluginConfigEntry> ConfigEntries = new ();

    public static ConfigEntry<T> BindConfig<T>(string category, string key, T defaultValue, string description, Action onChanged)
    {
        ConfigEntry<T> entry = Config.Bind(
            category,
            key,
            defaultValue,
            description
        );

        ConfigEntries.Add(new PluginConfigEntry(entry, onChanged));

        return entry;
    }

    public static void SetPatch(bool enabled, MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
    {
        Patches patches = Harmony.GetPatchInfo(original);

        bool prefixPatched = false;
        bool postfixPatched = false;
        if(patches != null)
        {
            for(int x = 0; x < patches.Prefixes.Count; x++)
                prefixPatched |= patches.Prefixes[x].PatchMethod == prefix.method;
            for(int x = 0; x < patches.Postfixes.Count; x++)
                postfixPatched |= patches.Postfixes[x].PatchMethod == postfix.method;
        }

        bool patched = prefixPatched | postfixPatched;

        if(!patched && enabled)
            Harmony.Patch(original, prefix, postfix);
        else if(patched && !enabled)
        {
            if(prefixPatched)
                Harmony.Unpatch(original, prefix.method);
            if(postfixPatched)
                Harmony.Unpatch(original, postfix.method);
        }
    }

    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;
        Harmony = new (PluginGUID);

        Config.SettingChanged += OnSettingChanged;

        HuntsmanOverhaul.Init();
        TrudgeOverhaul.Init();
        HeadmanOverhaul.Init();
        PitOverhaul.Init();
        RobeOverhaul.Init();
        OverhaulDirector.Init();

        for(int x = 0; x < ConfigEntries.Count; x++)
        {
            PluginConfigEntry configEntry = ConfigEntries[x];
            if(configEntry.Changed != null)
                configEntry.Changed();
        }
    }

    private void OnSettingChanged(object obj, SettingChangedEventArgs args)
    {
        for(int x = 0; x < ConfigEntries.Count; x++)
        {
            PluginConfigEntry configEntry = ConfigEntries[x];
            if(configEntry.Entry == args.ChangedSetting && configEntry.Changed != null)
                configEntry.Changed();
        }
    }
}