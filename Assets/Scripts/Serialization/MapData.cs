using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PointData
{
    public string UniquePointId;
    public PathPointType Type;
    public bool IsStartPoint;
    public Vector2 Position;
    public List<PathConnection> Connections = new List<PathConnection>();
}

[Serializable]
public class LevelData
{
    public string MapId;
    public List<PointData> Points = new List<PointData>();
}

[Serializable]
public class WorldEdgeData
{
    public string TargetNodeId;
    public string LevelId;
}

[Serializable]
public class WorldNodeData
{
    public string NodeId;
    public Vector2 Position;
    public bool IsSavePoint;
    public bool IsCache;
    public List<WorldEdgeData> Edges = new List<WorldEdgeData>();
}

[Serializable]
public class WorldMapData
{
    public string MapId;
    public List<WorldNodeData> Nodes = new List<WorldNodeData>();
}

[Serializable]
public class MapFile
{
    public List<LevelData> LevelMaps = new List<LevelData>();
    public List<WorldMapData> WorldMaps = new List<WorldMapData>();
}

public static class MapFiles
{
    public static string GetPath()
    {
        return "Assets/StreamingAssets/maps.json";
    }

    public static MapFile ReadFile()
    {
        string json = File.ReadAllText(GetPath());
        return JsonUtility.FromJson<MapFile>(json);
    }
}

public class WorldNodeRef
{
    public string WorldMapId;
    public string NodeId;

    public WorldNodeRef(string worldMapId, string nodeId)
    {
        WorldMapId = worldMapId;
        NodeId = nodeId;
    }
}