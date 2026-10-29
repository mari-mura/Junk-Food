using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextLevel;
    bool used;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;

        Scene currentLevel = gameObject.scene;
        if (string.IsNullOrEmpty(nextLevel))
        {
            if (SceneTransporter.savedOverworld != null)
                SceneTransporter.savedOverworld.SetActive(true);
            else
                Debug.LogWarning("No Overworld");
        }
        else
        {
            SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
        }

        SceneManager.UnloadSceneAsync(currentLevel);
    }
}