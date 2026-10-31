using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WashManager : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private DirtScrubber scrubber;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float washSeconds = 60f;
    [SerializeField] private string dayScene = "DayTimeLobby";

    private readonly Queue<Collectible_ItemSO> queue = new Queue<Collectible_ItemSO>();
    private Collectible_ItemSO current;
    private float timeLeft;
    private bool finished;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        
        foreach (InventoryData.Entry entry in inventoryData.Entries)
        {
            if (entry.item == null || entry.item.washedVersion == null) continue;
            for (int i = 0; i < entry.quantity; i++) queue.Enqueue(entry.item);
        }

        timeLeft = washSeconds;
        scrubber.Cleaned += OnItemCleaned;
        NextItem();
    }

    private void OnDestroy()
    {
        scrubber.Cleaned -= OnItemCleaned;
    }

    private void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeLeft)).ToString();

        if (timeLeft <= 0f) Finish();
    }

    private void NextItem()
    {
        if (queue.Count == 0)
        {
            Finish();
            return;
        }

        current = queue.Dequeue();
        itemImage.sprite = current.washedVersion.icon;
        scrubber.Begin();
    }

    private void OnItemCleaned()
    {
        if (!inventoryData.TryConvert(current, current.washedVersion))
        {
            Debug.Log("Inventory might be full");
        }
        NextItem();
    }

    private void Finish()
    {
        if (finished) return;
        finished = true;

        scrubber.Stop();
        SceneManager.LoadScene(dayScene);
    }
}