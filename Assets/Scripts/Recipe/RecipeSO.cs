using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe")]
public class RecipeSO : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public Collectible_ItemSO item;
        public int quantity = 1;
    }

    public string recipeName;
    public Sprite icon;
    public List<Ingredient> ingredients = new List<Ingredient>();
    public string firstStepScene;

    // true if inventory contains sufficient ingredients
    public bool CanMake(InventoryData inventory)
    {
        if (inventory == null) return false;

        foreach (Ingredient ing in ingredients)
        {
            if (ing.item == null)
            {
                return false;
            }

            if (!inventory.Has(ing.item, ing.quantity)) return false;
        }
        return true;
    }
}