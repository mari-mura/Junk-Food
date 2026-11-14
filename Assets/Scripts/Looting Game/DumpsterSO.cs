using UnityEngine;

[CreateAssetMenu(fileName = "Dumpster", menuName = "Dumpster")]
public class DumpsterSO : ScriptableObject
{
    public LootTableSO lootTable;

    [Header("How many items are hidden")]
    [Min(0)] public int minItems = 2;
    [Min(0)] public int maxItems = 4;

    [Header("Rarity odds (bigger = more likely)")]
    [Min(0f)] public float commonWeight = 50f;
    [Min(0f)] public float rareWeight = 30f;
    [Min(0f)] public float ultraRareWeight = 20f;

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
    
    [ContextMenu("log rarity chances")]
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