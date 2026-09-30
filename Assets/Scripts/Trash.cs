using UnityEngine;
using System;

public class Trash : MonoBehaviour, IItem
{
    public static event Action<int> OnTrashCollect;
    public int worth = 5;
    public void Collect()
    {
        OnTrashCollect.Invoke(worth);
        Destroy(gameObject);
    }

}
