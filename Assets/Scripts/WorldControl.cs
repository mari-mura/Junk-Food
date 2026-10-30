using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldControl : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("DaytimeLobby");
    }
}