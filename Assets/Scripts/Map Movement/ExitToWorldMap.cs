using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToWorldMap : MonoBehaviour
{
    
    [SerializeField] private float delaySeconds = 1f;

    private bool isLeaving = false;

    private void OnEnable()
    {
        PathSession.OnPathFinished += OnPathFinished;
    }

    private void OnDisable()
    {
        PathSession.OnPathFinished -= OnPathFinished;
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

    private void LeaveLevel()
    {
        MapTransitioner.MoveToWorldMapNode(SceneManager.GetActiveScene().name);
    }
}