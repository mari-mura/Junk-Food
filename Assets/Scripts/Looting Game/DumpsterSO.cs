using UnityEngine;

[CreateAssetMenu(fileName = "Dumpster", menuName = "Dumpster")]
public class DumpsterSO : ScriptableObject
{
    public LootTableSO lootTable;
    
    [Min(0)] public int minItems = 2;
    [Min(0)] public int maxItems = 4;
    
    [Min(0f)] public float commonWeight = 100f;
    [Min(0f)] public float rareWeight = 20f;
    [Min(0f)] public float ultraRareWeight = 3f;

    private void OnValidate()
    {
        if (maxItems < minItems) maxItems = minItems;
    }
    
    public Rarity RollRarity()
    {
        float total = commonWeight + rareWeight + ultraRareWeight;
        if (total <= 0f) return Rarity.Common;

        float roll = Random.value * total;
        if (roll < commonWeight) return Rarity.Common;
        if (roll < commonWeight + rareWeight) return Rarity.Rare;
        return Rarity.UltraRare;
    }

    [ContextMenu("Log rarity chances")]
    private void LogChances()
    {
        float total = commonWeight + rareWeight + ultraRareWeight;
        if (total <= 0f)
        {
            return;
        }

        Debug.Log($"Common {commonWeight / total * 100f:0.#}%, Rare {rareWeight / total * 100f:0.#}%, " +
                  $"Ultra-rare {ultraRareWeight / total * 100f:0.#}%", this);
    }
}