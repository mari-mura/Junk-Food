using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class ScavengeManager : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private DumpsterSO dumpster;
    
    [SerializeField] private string overworldScene = "WorldMap";
    
    [SerializeField] private ScavengeItem itemPrefab;
    [SerializeField] private JunkPiece[] junkPrefabs;
    
    [SerializeField] private BoxCollider2D spawnArea;
    
    [SerializeField, Min(0)] private int junkCount = 15;
    [SerializeField, Min(0)] private int junkOverEachItem = 2;
    
    [SerializeField] private TMP_Text noticeText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private float noticeSeconds = 2f;
    [SerializeField] private float playSeconds = 30f;
    [SerializeField] private TMP_Text timerText;

    private float timeLeft;
    private bool timeUp;

    private Camera cam;
    private Transform dragging;
    private Vector2 dragOffset;
    private int topOrder = 10;
    private Coroutine noticeRoutine;
    private int junkLeft;
    private int junkSpawned;
    private bool junkRanShort;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cam = Camera.main;
        noticeText.text = "";
        leaveButton.onClick.AddListener(Leave);

        junkLeft = junkCount;
        int itemsSpawned = SpawnItems();
        
        while (TrySpawnJunk(RandomPoint())) { }
        timeLeft = playSeconds;
    }
    
    private void Update()
    {
        if (timeUp) return;

        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(timeLeft, 0f)).ToString();

            if (timeLeft <= 0f)
            {
                StartCoroutine(TimeUpRoutine());
                return;
            }
        }
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 world = cam.ScreenToWorldPoint(mouse.position.ReadValue());

        if (mouse.leftButton.wasPressedThisFrame) OnPress(world);

        if (dragging != null)
        {
            if (mouse.leftButton.isPressed) dragging.position = world + dragOffset;
            else dragging = null;
        }
    }

    private int SpawnItems()
    {
        int spawned = 0;
        int count = Random.Range(dumpster.minItems, dumpster.maxItems + 1);
        for (int i = 0; i < count; i++)
        {
            Rarity rarity = dumpster.RollRarity();
            if (!dumpster.lootTable.TryRoll(rarity, out Collectible_ItemSO item, out int quantity) &&
                !dumpster.lootTable.TryRoll(out item, out quantity))
            {
                continue;
            }

            Vector2 position = RandomPoint();
            ScavengeItem spawnedItem = Instantiate(itemPrefab, position, Quaternion.identity);
            spawnedItem.Setup(item, quantity);
            FitColliderToSprite(spawnedItem.gameObject);
            spawned++;

            // bury it
            for (int j = 0; j < junkOverEachItem; j++)
            {
                if (!TrySpawnJunk(position + Random.insideUnitCircle * 0.3f))
                {
                    junkRanShort = true;
                    break;
                }
            }
        }

        return spawned;
    }
    private bool TrySpawnJunk(Vector2 position)
    {
        if (junkLeft <= 0) return false;

        JunkPiece prefab = junkPrefabs[Random.Range(0, junkPrefabs.Length)];
        JunkPiece junk = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        junk.GetComponent<SpriteRenderer>().sortingOrder = ++topOrder;

        junkLeft--;
        junkSpawned++;
        return true;
    }
    private Vector2 RandomPoint()
    {
        Bounds b = spawnArea.bounds;
        return new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
    }

    private void OnPress(Vector2 world)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(world);
        Collider2D top = GetTopmost(hits);
        if (top == null) return;

        if (top.TryGetComponent(out JunkPiece junk))
        {
            dragging = junk.transform;
            dragOffset = (Vector2)junk.transform.position - world;
            junk.GetComponent<SpriteRenderer>().sortingOrder = ++topOrder;
        }
        else if (top.TryGetComponent(out ScavengeItem item))
        {
            Collect(item);
        }
    }
    private static Collider2D GetTopmost(Collider2D[] hits)
    {
        Collider2D best = null;
        int bestOrder = int.MinValue;

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<JunkPiece>() == null && hit.GetComponent<ScavengeItem>() == null) continue;

            SpriteRenderer sprite = hit.GetComponent<SpriteRenderer>();
            int order = sprite != null ? sprite.sortingOrder : int.MinValue;
            if (best == null || order > bestOrder)
            {
                best = hit;
                bestOrder = order;
            }
        }
        return best;
    }

    private void Collect(ScavengeItem found)
    {
        if (!inventoryData.Add(found.Item, found.Quantity))
        {
            ShowNotice("no room in your bag");
            return;
        }

        ShowNotice($"Found {found.Item.itemName}! ({RarityLabel(found.Item.rarity)})");
        Destroy(found.gameObject);
    }

    private static string RarityLabel(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Rare: return "Rare";
            case Rarity.UltraRare: return "Ultra-rare";
            default: return "Common";
        }
    }

    public void Leave()
    {
        if (!Application.CanStreamedLevelBeLoaded(overworldScene))
        {
            return;
        }

        SceneManager.LoadScene(overworldScene);
    }

    private void ShowNotice(string text)
    {
        if (noticeRoutine != null) StopCoroutine(noticeRoutine);
        noticeRoutine = StartCoroutine(NoticeRoutine(text));
    }

    private IEnumerator NoticeRoutine(string text)
    {
        noticeText.text = text;
        yield return new WaitForSeconds(noticeSeconds);
        noticeText.text = "";
    }
    
    // resizes the collider
    private static void FitColliderToSprite(GameObject obj)
    {
        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (box == null || sr == null || sr.sprite == null) return;

        box.size = sr.sprite.bounds.size;
        box.offset = sr.sprite.bounds.center;
    }
    private IEnumerator TimeUpRoutine()
    {
        timeUp = true;
        dragging = null;
        ShowNotice("time's up");
        yield return new WaitForSeconds(1.5f);
        Leave();
    }
}