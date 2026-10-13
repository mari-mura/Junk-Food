using System;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] itemSlots;
    private void OnEnable()
    {
        Trash.OnLootCollect += AddItem;
    }
    
    private void OnDisable()
    {
        Trash.OnLootCollect -= AddItem;
    }

    public void AddItem(Collectible_ItemSO collectibleItemSo, int quantity)
    {
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.collectibleItemSO == null)
            {
                itemSlot.collectibleItemSO = collectibleItemSo;
                itemSlot.quantity = quantity;
                itemSlot.UpdateUI();
                return;
            }
            itemSlot.UpdateUI();
        }
    }
}
