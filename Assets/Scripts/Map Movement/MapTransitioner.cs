using UnityEngine;
using UnityEngine.SceneManagement;

public static class MapTransitioner
{
    private static string arrivalNodeId;
    private static bool hasArrivalNode = false;

    public static void MoveToRhythmMap(string levelId, string destinationNodeId)
    {
        arrivalNodeId = destinationNodeId;
        hasArrivalNode = true;

        SceneManager.LoadScene(levelId);
    }
    
    public static void MoveToWorldMapNode(string levelId)
    {
        WorldNodeRef destination = Maps.GetConnectedWorldMapNode(levelId);
        ArriveAtWorldNode(destination.WorldMapId, destination.NodeId);

        arrivalNodeId = destination.NodeId;
        hasArrivalNode = true;
        
        SceneManager.LoadScene(destination.WorldMapId);
    }
    
    public static void MoveToSavePoint()
    {
        NoiseTracker.Reset();

        WorldNodeRef savePoint = SavePoints.GetSavePoint();
        if (savePoint == null)
        {
            Debug.LogWarning("No save point");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        Debug.Log("Caught, moving to save point " + savePoint.NodeId);
        arrivalNodeId = savePoint.NodeId;
        hasArrivalNode = true;
        
        SceneManager.LoadScene(savePoint.WorldMapId);
    }
    
    public static void ArriveAtWorldNode(string worldMapId, string nodeId)
    {
        if (IsSavePoint(worldMapId, nodeId))
        {
            SavePoints.SetSavePoint(worldMapId, nodeId);
        }
    }
    
    public static bool TryGetArrivalNode(out string nodeId)
    {
        nodeId = arrivalNodeId;

        if (hasArrivalNode == false)
        {
            return false;
        }

        hasArrivalNode = false;
        return true;
    }

    private static bool IsSavePoint(string worldMapId, string nodeId)
    {
        WorldMapData worldMap = Maps.GetWorldMapData(worldMapId);
        if (worldMap == null)
        {
            return false;
        }

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId == nodeId)
            {
                return node.IsSavePoint;
            }
        }

        return false;
    }
}