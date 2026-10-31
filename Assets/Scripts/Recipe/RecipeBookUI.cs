using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RecipeBookUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private CookingSession session;
    [SerializeField] private RecipeBookData recipeBook;
    
    [Header("UI")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Transform entryContainer;
    [SerializeField] private RecipeEntryUI entryPrefab;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageSeconds = 2f;
    
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    private Coroutine messageRoutine;
    private void OnEnable()
    {
        if (openButton != null) openButton.onClick.AddListener(Open);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        if (openButton != null) openButton.onClick.RemoveListener(Open);
        if (closeButton != null) closeButton.onClick.RemoveListener(Close);
    }

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
    
    private void Refresh()
    {
        foreach (Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipe in recipeBook.Unlocked)
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
            ShowMessage("insufficient ingredients!");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(recipe.firstStepScene))
        {
            return;
        }
        
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