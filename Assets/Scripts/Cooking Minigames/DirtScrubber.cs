using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class DirtScrubber : MonoBehaviour
{
    [SerializeField] private RectTransform sponge;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private RectTransform dirtArea;
    [SerializeField] private Slider progressBar;
    
    [SerializeField] private int spotCount = 60;
    [SerializeField] private Vector2 sizeRange = new Vector2(0.6f, 1.2f);
    [SerializeField] private bool spawnInEllipse = true;

    [SerializeField] private float scrubRadius = 60f;
    // i set this to 95% since it might be hard to get every spot on time
    [SerializeField, Range(0.5f, 1f)] private float cleanFraction = 0.95f;

    public event System.Action Cleaned;

    private readonly List<GameObject> spots = new List<GameObject>();
    private Canvas canvas;
    private int remaining;
    private bool active;

    private void Awake()
    {
        canvas = sponge.GetComponentInParent<Canvas>().rootCanvas;
    }

    // called by WashManager at the start of each item
    public void Begin()
    {
        if (dirtPrefab == null || dirtArea == null)
        {
            return;
        }

        SpawnSpots();
        remaining = spots.Count;
        active = true;
        UpdateBar();
    }

    public void Stop()
    {
        active = false;
    }

    private void Update()
    {
        if (!active || Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        sponge.position = mousePos;

        if (!Mouse.current.leftButton.isPressed) return;

        // clean every speck inside a circle around the sponge
        float radius = scrubRadius * canvas.scaleFactor;
        float radiusSqr = radius * radius;

        foreach (GameObject spot in spots)
        {
            if (!spot.activeSelf) continue;

            Vector2 spotPos = spot.transform.position;
            if ((spotPos - mousePos).sqrMagnitude <= radiusSqr)
            {
                spot.SetActive(false);
                remaining--;
            }
        }

        UpdateBar();

        // done once enough is clean
        if (remaining <= spots.Count * (1f - cleanFraction))
        {
            active = false;
            Cleaned?.Invoke();
        }
    }

    private void SpawnSpots()
    {
        foreach (GameObject old in spots) Destroy(old);
        spots.Clear();

        Rect area = dirtArea.rect;
        for (int i = 0; i < spotCount; i++)
        {
            Vector2 p = spawnInEllipse
                ? Random.insideUnitCircle
                : new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            GameObject spot = Instantiate(dirtPrefab, dirtArea);
            spot.transform.localPosition = new Vector3(
                area.center.x + p.x * area.width * 0.5f,
                area.center.y + p.y * area.height * 0.5f,
                0f);
            spot.transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
            spots.Add(spot);
        }
    }

    private void UpdateBar()
    {
        if (progressBar == null || spots.Count == 0) return;

        // the bar hits full at the 95% clean point
        float cleaned = 1f - (float)remaining / spots.Count;
        progressBar.value = Mathf.Clamp01(cleaned / cleanFraction);
    }
}