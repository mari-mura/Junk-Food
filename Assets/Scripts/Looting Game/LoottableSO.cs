using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Loot Table")]
public class LootTableSO : ScriptableObject
{
    [System.Serializable]
    public class Drop
    {
        public Collectible_ItemSO item;
        [Min(0f)] public float weight = 1f;
        [Min(1)] public int minQuantity = 1;
        [Min(1)] public int maxQuantity = 1;
    }

    public List<Drop> drops = new List<Drop>();
    
    public bool TryRoll(out Collectible_ItemSO item, out int quantity)
    {
        return Pick(null, out item, out quantity);
    }
    public bool TryRoll(Rarity rarity, out Collectible_ItemSO item, out int quantity)
    {
        return Pick(rarity, out item, out quantity);
    }

    private bool Pick(Rarity? only, out Collectible_ItemSO item, out int quantity)
    {
        item = null;
        quantity = 0;

        float total = 0f;
        foreach (Drop d in drops)
        {
            if (IsEligible(d, only)) total += d.weight;
        }
        if (total <= 0f) return false;

        float roll = Random.value * total;
        Drop chosen = null;
        foreach (Drop d in drops)
        {
            if (!IsEligible(d, only)) continue;

            chosen = d;
            roll -= d.weight;
            if (roll <= 0f) break;
        }

        item = chosen.item;
        quantity = Random.Range(chosen.minQuantity, Mathf.Max(chosen.minQuantity, chosen.maxQuantity) + 1);
        return true;
    }

    private static bool IsEligible(Drop d, Rarity? only)
    {
        if (d.item == null || d.weight <= 0f) return false;
        return only == null || d.item.rarity == only.Value;
    }
    
    [ContextMenu("Log drop chances")]
    private void LogChances()
    {
        float total = 0f;
        foreach (Drop d in drops)
        {
            if (d.item != null) total += d.weight;
        }

        if (total <= 0f)
        {
            Debug.Log("loot table is empty", this);
            return;
        }

        foreach (Drop d in drops)
        {
            if (d.item == null) continue;
            Debug.Log($"{d.item.itemName}: {d.weight / total * 100f:0.#}%", this);
        }
    }
}