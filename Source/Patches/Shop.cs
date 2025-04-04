using HarmonyLib;
namespace Ardot.REPO.REPOverhaul;

public static class ShopPatches
{
    struct ItemFix(string name, Value value = null, int maxAmount = -1, int maxAmountInShop = -1)
    {
        public string Name = name;
        public Value Value = value;
        public int MaxAmount = maxAmount;
        public int MaxAmountInShop = maxAmountInShop;
    }

    public static void GameStart()
    {
        // Disable power crystal and health pack cost scaling

        AccessTools.Field(typeof(ShopManager), "crystalValueIncrease").SetValue(ShopManager.instance, 0f);
        AccessTools.Field(typeof(ShopManager), "healthPackValueIncrease").SetValue(ShopManager.instance, 0f);

        // Balance the cost of certain items in the shop

        ItemFix[] itemFixes =
        [
            new ("Item Power Crystal", Utils.Value(600f, 1000f)),
            new ("Item Drone Feather", Utils.Value(12000f, 15000f)),
            new ("Item Drone Indestructible", Utils.Value(15000f, 19000f)),
            new ("Item Drone Battery", Utils.Value(2500f, 3500f)),
            new ("Item Gun Tranq", Utils.Value(8000f, 11000f))
        ];

        for(int x = 0; x < itemFixes.Length; x++)
        {
            ItemFix fix = itemFixes[x];

            if(!StatsManager.instance.itemDictionary.TryGetValue(fix.Name, out Item item))
            {
                Plugin.Logger.LogWarning($"Item {fix.Name} not found while applying item fixes");
                continue;
            }

            if(fix.Value != null)
                item.value = fix.Value;
            if(fix.MaxAmount != -1)
                item.maxAmount = fix.MaxAmount;
            if(fix.MaxAmountInShop != -1)
                item.maxAmountInShop = fix.MaxAmountInShop;
        }
    }
}