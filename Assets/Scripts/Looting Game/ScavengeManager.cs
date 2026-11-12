using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ScavengeManager : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private DumpsterSO testDumpster;
    
    [SerializeField] private ScavengeItem itemPrefab;
    [SerializeField] private JunkPiece[] junkPrefabs;
    
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField, Min(0)] private int junkOverEachItem = 2;
    [SerializeField, Min(0)] private int extraJunk = 8;

    [SerializeField] private TMP_Text noticeText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private float noticeSeconds = 2f;

    private Camera cam;
    private Transform dragging;
    private Vector2 dragOffset;
    private int topOrder = 10;
    private Coroutine noticeRoutine;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cam = Camera.main;
        noticeText.text = "";
        leaveButton.onClick.AddListener(Leave);

        DumpsterSO dumpster = ScavengeSession.Dumpster != null ? ScavengeSession.Dumpster : testDumpster;
        if (dumpster == null || dumpster.lootTable == null)
        {
            Debug.LogWarning("no dumpster", this);
            return;
        }

        SpawnItems(dumpster);
        for (int i = 0; i < extraJunk; i++)
        {
            SpawnJunk(RandomPoint());
        }
    }

    private void Update()
    {
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
    private void SpawnItems(DumpsterSO dumpster)
    {
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
            Instantiate(itemPrefab, position, Quaternion.identity).Setup(item, quantity);
            
            for (int j = 0; j < junkOverEachItem; j++)
            {
                SpawnJunk(position + Random.insideUnitCircle * 0.3f);
            }
        }
    }

    private void SpawnJunk(Vector2 position)
    {
        if (junkPrefabs.Length == 0) return;

        JunkPiece prefab = junkPrefabs[Random.Range(0, junkPrefabs.Length)];
        JunkPiece junk = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        junk.GetComponent<SpriteRenderer>().sortingOrder = ++topOrder;
    }

    private Vector2 RandomPoint()
    {
        Bounds b = spawnArea.bounds;
        return new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
    }
    private void OnPress(Vector2 world)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Collider2D top = GetTopmost(Physics2D.OverlapPointAll(world));
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
            ShowNotice("no room in inventory");
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
        string scene = ScavengeSession.ReturnScene;
        if (string.IsNullOrEmpty(scene))
        {
            return;
        }

        SceneManager.LoadScene(scene);
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
}