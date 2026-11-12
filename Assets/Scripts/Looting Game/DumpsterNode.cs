using UnityEngine;
using UnityEngine.SceneManagement;
public class DumpsterNode : MonoBehaviour, IItem
{
    [SerializeField] private DumpsterSO dumpster;
    [SerializeField] private string scavengeScene = "ScavengeScene";

    public void Collect()
    {
        string level = SceneManager.GetActiveScene().name;
        string id = $"{level}/{name}";

        if (ScavengeSession.WasSearched(id))
        {
            return;
        }

        ScavengeSession.Begin(dumpster, level, id);
        SceneManager.LoadScene(scavengeScene);
    }
}