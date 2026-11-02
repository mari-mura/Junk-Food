using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

public class Trash : MonoBehaviour, IItem
{
    public static event Action<int> OnTrashCollect;
    public static event Action<Collectible_ItemSO, int> OnLootCollect;

    public int worth = 10;
    public TrashData trashData;
    
    [FormerlySerializedAs("collectibleItemSO")] public Collectible_ItemSO collectibleItemSo;
    public int quantity = 1;
    public SpriteRenderer sr;

    public void Collect()
    {
        try
        {
            if (collectibleItemSo != null)
            {
                OnLootCollect?.Invoke(collectibleItemSo, quantity);
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
        if (collectibleItemSo == null || sr == null)
            return;
        sr.sprite = collectibleItemSo.icon;
        name = collectibleItemSo.itemName;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEvent()
    {
        OnTrashCollect = null;
        OnLootCollect = null;
    }
}