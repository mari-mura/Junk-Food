using System.Collections.Generic;
using UnityEngine;

public enum PathPointType
{
    Tap,
    HoldStart,
    HoldEnd,
    Chord,
    Choice
}

[System.Serializable]
public class PathConnection
{
    public string TargetPointId;
    public string Button;
}

public class PathPoint : MonoBehaviour
{
    public string UniquePointId;
    public PathPointType Type;
    public bool IsStartPoint;
    public List<PathConnection> Connections = new List<PathConnection>();

    private List<PathEdge> edges = new List<PathEdge>();

    public Vector2 Position
    {
        get
        {
            return transform.position;
        }
    }

    public List<PathEdge> Edges
    {
        get
        {
            return edges;
        }
    }

    public void ClearEdges()
    {
        edges.Clear();
    }

    public bool IsDeadEnd()
    {
        return edges.Count == 0;
    }

    public void AddEdge(PathPoint destination, string choiceButton)
    {
        PathEdge newEdge = new PathEdge(this, destination, choiceButton);
        edges.Add(newEdge);
    }

    public PathEdge GetEdgeForButton(string pressedButton)
    {
        foreach (PathEdge edge in edges)
        {
            if (edge.ChoiceButton == pressedButton)
            {
                return edge;
            }
        }
        return null;
    }

    public override string ToString()
    {
        string result = string.Format("{0} [{1}] at ({2}, {3})", UniquePointId, Type, Position.x, Position.y);

        if (edges.Count == 0)
        {
            return result + " (dead end)";
        }

        foreach (PathEdge edge in edges)
        {
            result += "\n    " + edge;
        }
        return result;
    }
}