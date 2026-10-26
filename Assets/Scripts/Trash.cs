using System;
using UnityEngine;

public class Trash : MonoBehaviour, IItem
{
    public static event Action<int> OnTrashCollect;
    public static event Action<Collectible_ItemSO, int> OnLootCollect;

    public int worth = 10;
    public TrashData trashData;
    
    public Collectible_ItemSO collectibleItemSO;
    public int quantity = 1;
    public SpriteRenderer sr;

    public void Collect()
    {
        try
        {
            if (collectibleItemSO != null)
            {
                OnLootCollect?.Invoke(collectibleItemSO, quantity);
            }

            int burden = trashData != null ? trashData.burdenAmount : worth;
            OnTrashCollect?.Invoke(worth);
        }
        finally
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        if (collectibleItemSO == null || sr == null)
            return;
        sr.sprite = collectibleItemSO.icon;
        name = collectibleItemSO.itemName;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEvent()
    {
        OnTrashCollect = null;
        OnLootCollect = null;
    }
}