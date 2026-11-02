using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeBookData", menuName = "Recipe Book Data")]
public class RecipeBookData : ScriptableObject
{
    [SerializeField] private List<RecipeSO> unlocked = new List<RecipeSO>();

    public IReadOnlyList<RecipeSO> Unlocked => unlocked;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    
    public bool Unlock(RecipeSO recipe)
    {
        if (recipe == null || unlocked.Contains(recipe)) return false;

        unlocked.Add(recipe);
        return true;
    }

    public bool IsUnlocked(RecipeSO recipe) => unlocked.Contains(recipe);
    
    [ContextMenu("Reset (lock all recipes)")]
    public void ResetBook()
    {
        unlocked.Clear();
    }
}