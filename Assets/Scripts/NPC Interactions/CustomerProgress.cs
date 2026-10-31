using UnityEngine;

[CreateAssetMenu(fileName = "CustomerProgress", menuName = "Customer Progress")]
public class CustomerProgress : ScriptableObject
{
    public int nextIndex;
    public CustomerSO active;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public void Begin(CustomerSO customer)
    {
        active = customer;
        nextIndex++;
    }

    public void Finish()
    {
        active = null;
    }
    
    [ContextMenu("Reset")]
    public void ResetProgress()
    {
        nextIndex = 0;
        active = null;
    }
}