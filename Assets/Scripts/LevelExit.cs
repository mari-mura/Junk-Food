using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 1f;

    private bool isLeaving = false;

    private void OnEnable()
    {
        PathSession.OnPathFinished += OnPathFinished;
        NoiseTracker.Caught += OnCaught;
    }

    private void OnDisable()
    {
        PathSession.OnPathFinished -= OnPathFinished;
        NoiseTracker.Caught -= OnCaught;
    }

    private void OnPathFinished()
    {
        if (isLeaving)
        {
            return;
        }

        isLeaving = true;
        Invoke(nameof(LeaveLevel), delaySeconds);
    }

    private void OnCaught()
    {
        if (isLeaving)
        {
            return;
        }

        isLeaving = true;
        Invoke(nameof(RespawnAtSavePoint), delaySeconds);
    }

    private void LeaveLevel()
    {
        MapTransitioner.MoveToWorldMapNode(SceneManager.GetActiveScene().name);
    }

    private void RespawnAtSavePoint()
    {
        MapTransitioner.MoveToSavePoint();
    }
}