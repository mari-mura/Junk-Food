using TMPro;
using UnityEngine;

// TLDR: Shows the current node's prompt on screen
public class PromptDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;

    public void Show(string prompt)
    {
        if (promptText != null)
        {
            promptText.text = prompt;
        }
    }
}