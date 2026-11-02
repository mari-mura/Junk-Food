using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class ApproachCircle : MonoBehaviour
{
    [SerializeField] private Transform approachRing;
    [SerializeField] private Image approachRenderer;
    [SerializeField] private float startScale = 2f; // how big it starts out
    [SerializeField] private float approachTime = 1.2f; // how early the ring starts shrinking

    private JumpTimingTracker tracker;
    private bool running;

    public void Begin(JumpTimingTracker t)
    {
        tracker = t;
        running = true;
        approachRing.gameObject.SetActive(true);
        Refresh();
    }

    public void Stop()
    {
        running = false;
        approachRing.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (running) Refresh();
    }

    private void Refresh()
    {
        float offset = tracker.GetOffset();
        float t = Mathf.Clamp01(-offset / approachTime);

        approachRing.localScale = Vector3.one * Mathf.Lerp(1f, startScale, t);
    }
}