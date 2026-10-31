using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private Button nextButton;

    private string[] lines;
    private int index;
    private System.Action onFinished;

    private void Awake()
    {
        nextButton.onClick.AddListener(Advance);
        panel.SetActive(false);
    }

    public void Show(string speaker, Sprite face, string[] dialogueLines, System.Action finished)
    {
        lines = dialogueLines;
        onFinished = finished;

        if (lines == null || lines.Length == 0)
        {
            onFinished?.Invoke();
            return;
        }

        index = 0;
        nameText.text = speaker;
        portrait.sprite = face;
        lineText.text = lines[0];
        panel.SetActive(true);
    }

    private void Advance()
    {
        index++;
        if (index < lines.Length)
        {
            lineText.text = lines[index];
            return;
        }

        panel.SetActive(false);
        onFinished?.Invoke();
    }
}