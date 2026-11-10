using UnityEngine;
public static class SavePoints
{
    private static WorldNodeRef current = null;

    public static void SetSavePoint(string worldMapId, string nodeId)
    {
        Debug.Log("SavePoints: set to " + worldMapId + " / " + nodeId);
        current = new WorldNodeRef(worldMapId, nodeId);
    }
    public static WorldNodeRef GetSavePoint()
    {
        Debug.Log("SavePoints: get -> " + (current == null ? "none yet" : current.NodeId));
        return current;
    }
}