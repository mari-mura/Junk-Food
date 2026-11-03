using System.Collections.Generic;
using UnityEngine;

public static class CacheNodes
{
    private static Dictionary<string, InventoryData> inventories = new Dictionary<string, InventoryData>();
    
    public static bool IsCache(string worldMapId, string nodeId)
    {
        WorldMapData worldMap = Maps.GetWorldMapData(worldMapId);

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId == nodeId)
            {
                return node.IsCache;
            }
        }

        return false;
    }
    
    public static InventoryData GetInventory(string nodeId)
    {
        InventoryData inventory;
        if (inventories.TryGetValue(nodeId, out inventory) && inventory != null)
        {
            return inventory;
        }

        inventory = ScriptableObject.CreateInstance<InventoryData>();
        inventory.name = "Cache_" + nodeId;
        inventories[nodeId] = inventory;
        return inventory;
    }
    
    public static int DepositAll(string nodeId, InventoryData playerInventory)
    {
        return MoveAll(playerInventory, GetInventory(nodeId));
    }
    
    public static int WithdrawAll(string nodeId, InventoryData playerInventory)
    {
        return MoveAll(GetInventory(nodeId), playerInventory);
    }


    private static int MoveAll(InventoryData from, InventoryData to)
    {
        List<Collectible_ItemSO> items = new List<Collectible_ItemSO>();
        List<int> quantities = new List<int>();

        foreach (InventoryData.Entry entry in from.Entries)
        {
            items.Add(entry.item);
            quantities.Add(entry.quantity);
        }

        int totalMoved = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (to.Add(items[i], quantities[i]))
            {
                from.Remove(items[i], quantities[i]);
                totalMoved += quantities[i];
            }
        }

        return totalMoved;
    }
}