using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory Data")]
public class InventoryData : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public Collectible_ItemSO item;
        public int quantity;
    }

    [SerializeField] private int maxSlots = 12;
    [SerializeField] private List<Entry> entries = new List<Entry>();

    public IReadOnlyList<Entry> Entries => entries;
    
    public event Action Changed;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    
    public bool Add(Collectible_ItemSO item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        // stack onto an existing entry of the same item
        Entry existing = entries.Find(e => e.item == item);
        if (existing != null)
        {
            existing.quantity += quantity;
            Changed?.Invoke();
            return true;
        }

        // full inventory
        if (entries.Count >= maxSlots) return false;

        entries.Add(new Entry { item = item, quantity = quantity });
        Changed?.Invoke();
        return true;
    }

    public bool Remove(Collectible_ItemSO item, int quantity = 1)
    {
        Entry existing = entries.Find(e => e.item == item);
        if (existing == null || existing.quantity < quantity) return false;

        existing.quantity -= quantity;
        if (existing.quantity <= 0) entries.Remove(existing);
        Changed?.Invoke();
        return true;
    }

    public int GetQuantity(Collectible_ItemSO item)
    {
        Entry e = entries.Find(x => x.item == item);
        return e != null ? e.quantity : 0;
    }

    public bool Has(Collectible_ItemSO item, int quantity = 1) => GetQuantity(item) >= quantity;

    // swaps items in inventory
    public bool TryConvert(Collectible_ItemSO from, Collectible_ItemSO to)
    {
        if (from == null || to == null || from == to) return false;

        Entry source = entries.Find(e => e.item == from);
        if (source == null || source.quantity < 1) return false;

        Entry target = entries.Find(e => e.item == to);
        bool sourceDisappears = source.quantity == 1;
        
        if (target == null && !sourceDisappears && entries.Count >= maxSlots) return false;

        source.quantity--;
        if (source.quantity <= 0) entries.Remove(source);

        if (target != null) target.quantity++;
        else entries.Add(new Entry { item = to, quantity = 1 });

        Changed?.Invoke();
        return true;
    }
    
    public void Clear()
    {
        entries.Clear();
        Changed?.Invoke();
    }
}