using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransporter : MonoBehaviour
{
    public string levelName;
    public GameObject overworld;
    public static GameObject savedOverworld;
    bool used;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;

        used = true;
        savedOverworld = overworld;
        overworld.SetActive(false);
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }
}
