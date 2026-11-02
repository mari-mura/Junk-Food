using UnityEngine;

[CreateAssetMenu(fileName = "CookingSession", menuName = "Cooking Session")]
public class CookingSession : ScriptableObject
{
    public RecipeSO currentRecipe;

    public bool IsActive => currentRecipe != null;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public void Begin(RecipeSO recipe)
    {
        currentRecipe = recipe;
    }

    // call when the recipe is finished or cancelled
    public void Clear()
    {
        currentRecipe = null;
    }
}