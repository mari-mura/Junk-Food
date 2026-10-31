using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Customer")]
public class CustomerSO : ScriptableObject
{
    public string displayName;
    public Sprite portrait;
    public RecipeSO requestedRecipe;
    public RecipeSO recipeToGive;

    [TextArea(2, 4)]
    public string[] introLines;
}