using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory Data")]
public class InventoryData : ScriptableObject
{
    // class for each entry of slot items
    [Serializable]
    public class Entry
    {
        public Collectible_ItemSO item;
        public int quantity;
    }

    // i set this to 6 but we can increase the number of slots for collectibles
    [SerializeField] private int maxSlots = 6; 
    [SerializeField] private List<Entry> entries = new List<Entry>();
    
    // storing the list of slots as entries
    public IReadOnlyList<Entry> Entries => entries;
    
    public event Action Changed;
    
    public bool Add(Collectible_ItemSO item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        
        // matching existing item slots
        Entry existing = entries.Find(e => e.item == item);
        if (existing != null)
        {
            existing.quantity += quantity;
            Changed?.Invoke();
            return true;
        }

        if (entries.Count >= maxSlots) return false;
        
        // creating a new item slot if it doesn't exist 
        Debug.Log($"InventoryData.Add: item={(item != null ? item.itemName : "NULL")}, qty={quantity}, maxSlots={maxSlots}, entries={entries.Count}");
        entries.Add(new Entry { item = item, quantity = quantity });
        Changed?.Invoke();
        return true;
    }

    // when we get to using items, or just resetting after a day ends we need to remove items
    public bool Remove(Collectible_ItemSO item, int quantity = 1)
    {
        Entry existing = entries.Find(e => e.item == item);
        if (existing == null || existing.quantity < quantity) return false;

        // remove the item if we find it as an existing slot
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
    
    // empty the list like if we end up resetting a level or failing we would call this
    public void Clear()
    {
        entries.Clear();
        Changed?.Invoke();
    }
    private void OnEnable()
    {
        // don't unload asset
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}