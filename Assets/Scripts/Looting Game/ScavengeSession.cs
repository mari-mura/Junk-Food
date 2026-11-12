using System.Collections.Generic;
using UnityEngine;
public static class ScavengeSession
{
    public static DumpsterSO Dumpster { get; private set; }
    public static string ReturnScene { get; private set; }

    private static readonly HashSet<string> searched = new HashSet<string>();

    public static void Begin(DumpsterSO dumpster, string returnScene, string id)
    {
        Dumpster = dumpster;
        ReturnScene = returnScene;
        searched.Add(id);
    }

    public static bool WasSearched(string id)
    {
        return searched.Contains(id);
    }

    public static void ResetAll()
    {
        Dumpster = null;
        ReturnScene = null;
        searched.Clear();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnLaunch()
    {
        ResetAll();
    }
}