using System.Collections.Generic;
using UnityEngine;

public static class Maps
{
    private static Dictionary<string, WorldMapData> worldMaps = new Dictionary<string, WorldMapData>();

    private static Dictionary<string, LevelData> levelMaps = new Dictionary<string, LevelData>();
    
    private static Dictionary<string, string> nodeToWorldMapId = new Dictionary<string, string>();
    
    private static Dictionary<string, string> edgeToLevel = new Dictionary<string, string>();
    
    private static Dictionary<string, WorldNodeRef> levelToEndNode = new Dictionary<string, WorldNodeRef>();

    private static bool isLoaded = false;

    public static void LoadMaps()
    {
        isLoaded = false;
        worldMaps.Clear();
        levelMaps.Clear();
        nodeToWorldMapId.Clear();
        edgeToLevel.Clear();
        levelToEndNode.Clear();

        MapFile file = MapFiles.ReadFile();

        foreach (LevelData level in file.LevelMaps)
        {
            levelMaps.Add(level.MapId, level);
        }

        foreach (WorldMapData map in file.WorldMaps)
        {
            worldMaps.Add(map.MapId, map);

            foreach (WorldNodeData node in map.Nodes)
            {
                nodeToWorldMapId.Add(node.NodeId, map.MapId);
            }
        }
        
        foreach (WorldMapData map in file.WorldMaps)
        {
            foreach (WorldNodeData node in map.Nodes)
            {
                foreach (WorldEdgeData edge in node.Edges)
                {
                    LinkEdge(node, edge);
                }
            }
        }

        isLoaded = true;
    }

    public static WorldMapData GetWorldMapData(string mapId)
    {
        EnsureLoaded();

        WorldMapData map;
        if (worldMaps.TryGetValue(mapId, out map))
        {
            return map;
        }
        
        return null;
    }

    public static LevelData GetLevelData(string levelId)
    {
        EnsureLoaded();

        LevelData level;
        if (levelMaps.TryGetValue(levelId, out level))
        {
            return level;
        }
        
        return null;
    }

    public static string GetConnectedLevel(string startNodeId, string destinationNodeId)
    {
        EnsureLoaded();

        string levelId;
        if (edgeToLevel.TryGetValue(MakeEdgeKey(startNodeId, destinationNodeId), out levelId))
        {
            return levelId;
        }
        
        return null;
    }

    public static WorldNodeRef GetConnectedWorldMapNode(string levelId)
    {
        EnsureLoaded();

        WorldNodeRef endNode;
        if (levelToEndNode.TryGetValue(levelId, out endNode))
        {
            return endNode;
        }
        
        return null;
    }
    
    private static void LinkEdge(WorldNodeData node, WorldEdgeData edge)
    {
        if (string.IsNullOrEmpty(edge.LevelId))
        {
            return;
        }

        string destinationMapId;
        if (nodeToWorldMapId.TryGetValue(edge.TargetNodeId, out destinationMapId) == false)
        {
            return;
        }

        edgeToLevel[MakeEdgeKey(node.NodeId, edge.TargetNodeId)] = edge.LevelId;
        
        levelToEndNode[edge.LevelId] = new WorldNodeRef(destinationMapId, edge.TargetNodeId);
    }

    private static string MakeEdgeKey(string startNodeId, string destinationNodeId)
    {
        return startNodeId + "_" + destinationNodeId;
    }
    
    private static void EnsureLoaded()
    {
        if (isLoaded == false)
        {
            LoadMaps();
        }
    }
}