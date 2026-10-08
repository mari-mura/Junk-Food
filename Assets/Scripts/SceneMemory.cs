using UnityEngine;
using System.Collections.Generic;

public class SceneMemory : MonoBehaviour
{
    public static string lastScene;
    public static string lastPath;
    public static List<string> usedPaths = new List<string>();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetMemory()
    {
        lastScene = null;
        lastPath = null;
        usedPaths.Clear();
    }
}
