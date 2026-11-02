using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RecipeEntryUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text ingredientsText;
    [SerializeField] private Button button;

    public void Setup(RecipeSO recipe, InventoryData inventory, Action<RecipeSO> onClick)
    {
        iconImage.sprite = recipe.icon;
        nameText.text = recipe.recipeName;
        
        StringBuilder sb = new StringBuilder();
        foreach (RecipeSO.Ingredient ing in recipe.ingredients)
        {
            if (ing.item == null) continue;

            int have = inventory.GetQuantity(ing.item);
            string line = $"{ing.item.itemName}  {have}/{ing.quantity}";
            sb.AppendLine(have >= ing.quantity ? line : $"<color=#FF6B6B>{line}</color>");
        }
        ingredientsText.text = sb.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(recipe));
    }
}