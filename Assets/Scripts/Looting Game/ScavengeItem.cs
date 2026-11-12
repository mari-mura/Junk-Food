using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ScavengeItem : MonoBehaviour
{
    public Collectible_ItemSO Item { get; private set; }
    public int Quantity { get; private set; }

    public void Setup(Collectible_ItemSO item, int quantity)
    {
        Item = item;
        Quantity = quantity;
        GetComponent<SpriteRenderer>().sprite = item.icon;
        name = item.itemName;
    }
}