using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ardot.REPO.REPOverhaul;

public static class Debug
{ 
    public static void PrintTree(GameObject root, bool printComponents = true)
    {
        List<char> message = new ();

        Utils.ForObjectsInTree(root, (GameObject branch, int depth) => {
            Transform transform = branch.GetComponent<Transform>();

            message.Add('\n');
            for(int x = 0; x < depth; x++)
                message.AddRange("   |");
            message.AddRange($"- {transform.gameObject.name}");

            if(printComponents)
            {
                message.AddRange(" -");
                int componentCount = transform.gameObject.GetComponentCount();
                for(int x = 0; x < componentCount; x++)
                    message.AddRange($" {transform.gameObject.GetComponentAtIndex(x).GetType()},");
                message.RemoveAt(message.Count - 1);
            }
        });

        Plugin.Logger.LogInfo(new string([.. message]));
    }

    public static void PrintItems()
    {
        Plugin.Logger.LogInfo("Items           ---");

        Dictionary<string, Item> itemDictionary = StatsManager.instance.itemDictionary;
        foreach(string itemName in itemDictionary.Keys)
        {
            Item item = itemDictionary[itemName];
            Plugin.Logger.LogInfo($"{item.itemName}: {item.value.valueMax}");    
        }
    }

    public static void PrintEnemies()
    {       
        Plugin.Logger.LogInfo("Enemies          ---");

        foreach(string enemyName in Plugin.Enemies.Keys)
            Plugin.Logger.LogInfo(enemyName);
    }
}