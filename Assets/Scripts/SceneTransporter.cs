using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransporter : MonoBehaviour
{
    public string pathName;
    public string sceneToLoad;
    public bool takesMeBack;
    public Transform player;
    public Transform node;

    void Start()
    {
        if (pathName == SceneMemory.lastPath)
        {
            player.position = node.position;    
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;

        if (takesMeBack)
        {
            SceneManager.LoadScene(SceneMemory.lastScene);
        }
        else
        {
            if (SceneMemory.usedPaths.Contains(pathName)) return;
            
            SceneMemory.usedPaths.Add(pathName);
            SceneMemory.lastScene = SceneManager.GetActiveScene().name;
            SceneMemory.lastPath = pathName;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
