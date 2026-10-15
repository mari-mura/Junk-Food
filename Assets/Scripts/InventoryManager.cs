using UnityEngine;
public class InventoryManager : MonoBehaviour
{
    public InventoryData inventoryData;
    public InventorySlot[] itemSlots;

    private void OnEnable()
    {
        if (inventoryData == null)
        {
            Debug.Log("inventory manager null.", this);
            return;
        }

        Trash.OnLootCollect += AddItem;
        inventoryData.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        Trash.OnLootCollect -= AddItem;
        if (inventoryData != null)
        {
            inventoryData.Changed -= Refresh;
        }
    }

    public void AddItem(Collectible_ItemSO collectibleItemSo, int quantity)
    {
        inventoryData.Add(collectibleItemSo, quantity);
    }
    
    /* i guess technically this redraws every slot but i am too dumb to change it so iterating
     over the length of the slots to count them is what we gotta do
    */
    private void Refresh()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            // we get the entry i's item and quantity to set it
            if (i < inventoryData.Entries.Count)
            {
                itemSlots[i].collectibleItemSO = inventoryData.Entries[i].item;
                itemSlots[i].quantity = inventoryData.Entries[i].quantity;
            }
            else
            {
                itemSlots[i].collectibleItemSO = null;
                itemSlots[i].quantity = 0;
            }
            itemSlots[i].UpdateUI();
        }
    }
}