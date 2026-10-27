using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransporter : MonoBehaviour
{
    public string levelName;
    public GameObject overworld;
    public static GameObject savedOverworld;
    bool used;

    void OnEnable()
    {
        used = false;
    }
    void OnMouseDown()
    {
        if (used) return;
        used = true;
        
        savedOverworld = overworld;

        Scene overworldScene = overworld.scene;
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != overworldScene && scene.isLoaded)
                SceneManager.UnloadSceneAsync(scene);
        }
        overworld.SetActive(false);
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }
}
