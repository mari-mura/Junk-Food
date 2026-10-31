using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CustomerProgress progress;
    [SerializeField] private RecipeBookData recipeBook;
    [SerializeField] private CustomerSO[] customers;

    [Header("UI")]
    [SerializeField] private Image customerImage;
    [SerializeField] private DialogueBox dialogue;
    [SerializeField] private TMP_Text orderText;
    [SerializeField] private TMP_Text noticeText;
    [SerializeField] private float noticeSeconds = 2.5f;

    private Button customerButton;
    private CustomerSO pendingCustomer;

    private void Awake()
    {
        customerImage.raycastTarget = true;
        if (!customerImage.TryGetComponent(out customerButton))
        {
            customerButton = customerImage.gameObject.AddComponent<Button>();
        }
        customerButton.targetGraphic = customerImage;
        customerButton.onClick.AddListener(OnCustomerClicked);
    }

    private void Start()
    {
        customerImage.gameObject.SetActive(false);
        orderText.text = "";
        noticeText.text = "";

        if (progress.active != null)
        {
            ShowCustomer(progress.active);
            ShowOrder(progress.active);
        }
        else
        {
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        if (progress.nextIndex >= customers.Length) return;
        pendingCustomer = customers[progress.nextIndex];
        ShowCustomer(pendingCustomer);
    }

    private void OnCustomerClicked()
    {
        if (pendingCustomer == null) return;

        CustomerSO customer = pendingCustomer;
        pendingCustomer = null;
        dialogue.Show(customer.displayName, customer.portrait, customer.introLines,
                      () => OnDialogueFinished(customer));
    }

    private void OnDialogueFinished(CustomerSO customer)
    {
        progress.Begin(customer);
        ShowOrder(customer);

        if (customer.recipeToGive != null && recipeBook.Unlock(customer.recipeToGive))
        {
            ShowNotice($"New recipe: {customer.recipeToGive.recipeName}");
        }
    }

    [ContextMenu("Complete Order (test)")]
    public void CompleteOrder()
    {
        progress.Finish();
        customerImage.gameObject.SetActive(false);
        orderText.text = "";
        SpawnNext();
    }

    private void ShowCustomer(CustomerSO customer)
    {
        customerImage.sprite = customer.portrait;
        customerImage.gameObject.SetActive(true);
    }

    private void ShowOrder(CustomerSO customer)
    {
        if (customer.requestedRecipe != null)
        {
            orderText.text = $"Order: {customer.requestedRecipe.recipeName}";
        }
    }

    private void ShowNotice(string text)
    {
        StopAllCoroutines();
        StartCoroutine(NoticeRoutine(text));
    }

    private IEnumerator NoticeRoutine(string text)
    {
        noticeText.text = text;
        yield return new WaitForSecondsRealtime(noticeSeconds);
        noticeText.text = "";
    }
}