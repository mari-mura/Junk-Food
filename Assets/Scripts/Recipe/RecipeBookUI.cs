using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecipeBookUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private CookingSession session;
    [SerializeField] private RecipeSO[] recipes;

    [Header("UI")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Transform entryContainer;
    [SerializeField] private RecipeEntryUI entryPrefab;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageSeconds = 2f;

    private Coroutine messageRoutine;

    private void Start()
    {
        bookPanel.SetActive(false);
        if (messageText != null) messageText.text = "";
    }
    
    public void Open()
    {
        bookPanel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        bookPanel.SetActive(false);
    }

    // rebuild list for inventory changes like washing
    private void Refresh()
    {
        foreach (Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipe in recipes)
        {
            if (recipe == null) continue;

            RecipeEntryUI entry = Instantiate(entryPrefab, entryContainer);
            entry.Setup(recipe, inventoryData, OnRecipeClicked);
        }
    }

    private void OnRecipeClicked(RecipeSO recipe)
    {
        if (!recipe.CanMake(inventoryData))
        {
            ShowMessage("Insufficient ingredients!");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(recipe.firstStepScene))
        {
            return;
        }

        // lock in the recipe, then move to its first step
        session.Begin(recipe);
        SceneManager.LoadScene(recipe.firstStepScene);
    }

    private void ShowMessage(string text)
    {
        Debug.Log(text);
        if (messageText == null) return;

        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(MessageRoutine(text));
    }

    private IEnumerator MessageRoutine(string text)
    {
        messageText.text = text;
        yield return new WaitForSecondsRealtime(messageSeconds);
        messageText.text = "";
        messageRoutine = null;
    }
}