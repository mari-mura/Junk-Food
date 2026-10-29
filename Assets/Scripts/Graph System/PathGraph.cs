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
        string startId = StartPoint != null ? StartPoint.PointId : "none";
        sb.AppendLine(string.Format("PathGraph: {0} points, start = {1}", idToPoint.Count, startId));

        foreach (KeyValuePair<string, PathPoint> entry in idToPoint)
        {
            sb.AppendLine(entry.Value.ToString());
        }
        return sb.ToString();
    }
    public PathPoint AddPoint(string pointId, PathPointType type, Vector2 position)
    {
        if (idToPoint.ContainsKey(pointId))
        {
            Debug.LogWarning("Graph already contains node " + pointId);
            return idToPoint[pointId];
        }

        PathPoint newPoint = new PathPoint(pointId, type, position);
        idToPoint.Add(pointId, newPoint);

        if (StartPoint == null)
        {
            StartPoint = newPoint;
        }
        return newPoint;
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