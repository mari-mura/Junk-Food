using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneTransporter.savedOverworld.SetActive(true);
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
