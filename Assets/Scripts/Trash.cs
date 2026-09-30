using UnityEngine;
using System;

public class Trash : MonoBehaviour, IItem
{
    public static event Action<int> OnTrashCollect;
    public int worth = 10;
    public void Collect()
    {
        OnTrashCollect?.Invoke(worth);
        Destroy(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetEvent()
    {
        OnTrashCollect = null;
    }

}
