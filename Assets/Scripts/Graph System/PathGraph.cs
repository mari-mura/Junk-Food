using System.Collections.Generic;
using UnityEngine;
using StringBuilder = System.Text.StringBuilder;

public class PathGraph
{
    private Dictionary<string, PathPoint> idToPoint = new Dictionary<string, PathPoint>();
    public PathPoint StartPoint;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        string startId = StartPoint != null ? StartPoint.UniquePointId : "none";
        sb.AppendLine(string.Format("PathGraph: {0} points, start = {1}", idToPoint.Count, startId));

        foreach (KeyValuePair<string, PathPoint> entry in idToPoint)
        {
            sb.AppendLine(entry.Value.ToString());
        }
        return sb.ToString();
    }
    public void AddPoint(PathPoint point)
    {
        if (idToPoint.ContainsKey(point.UniquePointId))
        {
            Debug.LogWarning("Graph already contains point " + point.UniquePointId);
            return;
        }

        // Edges live on the component, so clear any left over from an earlier Build
        point.ClearEdges();
        idToPoint.Add(point.UniquePointId, point);

        if (point.IsStartPoint)
        {
            StartPoint = point;
        }
    }

    public PathPoint GetPoint(string pointId)
    {
        if (idToPoint.ContainsKey(pointId))
        {
            return idToPoint[pointId];
        }
        return null;
    }

    public void AddEdge(string fromId, string toId, string choiceButton)
    {
        PathPoint fromPoint = GetPoint(fromId);
        PathPoint toPoint = GetPoint(toId);

        if (fromPoint == null || toPoint == null)
        {
            Debug.LogWarning("Tried to connect unknown nodes: " + fromId + " and " + toId);
            return;
        }

        fromPoint.AddEdge(toPoint, choiceButton);
    }
}