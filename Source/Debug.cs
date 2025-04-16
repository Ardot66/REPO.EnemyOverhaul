using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ardot.REPO.EnemyOverhaul;

public static class Debug
{ 
    public static void PrintTree(Transform root, bool printComponents = true, int maxDepth = -1, IEnumerable<MonoBehaviour> forceInclude = null)
    {
        List<char> message = new ();

        Utils.ForObjectsInTree(root, (Transform branch, int depth) => {
            message.Add('\n');
            for(int x = 0; x < depth; x++)
                message.AddRange("   |");
            message.AddRange($"- {branch.gameObject.name}");

            if(printComponents)
            {
                message.AddRange(" -");
                int componentCount = branch.gameObject.GetComponentCount();
                for(int x = 0; x < componentCount; x++)
                    message.AddRange($" {branch.gameObject.GetComponentAtIndex(x).GetType()},");
                message.RemoveAt(message.Count - 1);
            }
        }, maxDepth, forceInclude);

        Plugin.Logger.LogInfo(new string([.. message]));
    }

    public static void PrintItems()
    {
        Plugin.Logger.LogInfo("Items           ---");

        Dictionary<string, Item> itemDictionary = StatsManager.instance.itemDictionary;
        foreach(string itemName in itemDictionary.Keys)
        {
            Item item = itemDictionary[itemName];
            Plugin.Logger.LogInfo($"{item.itemName} ({itemName}): Value {item.value.valueMin} - {item.value.valueMax} Max {item.maxAmount} - {item.maxAmountInShop}");    
        }
    }

    public static void PrintEnemies()
    {       
        Plugin.Logger.LogInfo("Enemies          ---");

        foreach(string enemyName in Plugin.Enemies.Keys)
            Plugin.Logger.LogInfo(enemyName);
    }
}