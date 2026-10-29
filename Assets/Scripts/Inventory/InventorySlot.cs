using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Collectible_ItemSO collectibleItemSO;
    public int quantity;

    public Image itemImage;
    public TMP_Text quantityText;

    public void UpdateUI()
    {
        if (collectibleItemSO != null)
        {
            itemImage.sprite = collectibleItemSO.icon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
       
    }
}
