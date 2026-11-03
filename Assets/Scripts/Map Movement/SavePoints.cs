using UnityEngine;
public static class SavePoints
{
    private static WorldNodeRef current = null;

    public static void SetSavePoint(string worldMapId, string nodeId)
    {
        current = new WorldNodeRef(worldMapId, nodeId);
    }
    public static WorldNodeRef GetSavePoint()
    {
        return current;
    }
}