using UnityEngine;

public class ItemJumpPoint : JumpPoint
{
    [SerializeField] private Collectible_ItemSO item;
    [SerializeField] private InventoryData inventoryData;

    private void Awake()
    {
        Type = PathPointType.Chord;
    }
    
    public override void OnActivate()
    {
    }

    public override void OnDeactivate()
    {
    }
    
    public override void OnJudged(HitGrade grade)
    {
        if (grade == HitGrade.Miss)
        {
            return;
        }

        if (item == null)
        {
            return;
        }

        InventoryData inventory = GetInventory();
        if (inventory == null)
        {
            Debug.Log("No inventory");
            return;
        }
        
        if (inventory.Add(item, 1))
        {
            Debug.Log("Picked up " + item.itemName);
        }
        else
        {
            Debug.Log("Invetnory full");
        }
    }

    private InventoryData GetInventory()
    {
        if (inventoryData != null)
        {
            return inventoryData;
        }

        InventoryManager manager = FindFirstObjectByType<InventoryManager>();
        if (manager != null)
        {
            return manager.inventoryData;
        }

        return null;
    }
}