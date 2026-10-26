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

public class PathPoint
{
    private string pointId;
    public Vector2 Position;
    public PathPointType Type;
    public List<PathEdge> Edges = new List<PathEdge>();

    public override string ToString()
    {
        string result = string.Format("{0} [{1}] at ({2}, {3})", pointId, Type, Position.x, Position.y);

        if (Edges.Count == 0)
        {
            return result + " (dead end)";
        }

        foreach (PathEdge edge in Edges)
        {
            result += "\n    " + edge;
        }
        return result;
    }
    
    public string PointId
    {
        get
        {
            return pointId;
        }
    }

    public PathPoint(string idInput, PathPointType typeInput, Vector2 positionInput)
    {
        pointId = idInput;
        Type = typeInput;
        Position = positionInput;
    }

    public bool IsDeadEnd()
    {
        return Edges.Count == 0;
    }

    public void AddEdge(PathPoint destination, string choiceButton)
    {
        PathEdge newEdge = new PathEdge(this, destination, choiceButton);
        Edges.Add(newEdge);
    }

    public PathEdge GetEdgeForButton(string pressedButton)
    {
        foreach (PathEdge edge in Edges)
        {
            if (edge.ChoiceButton == pressedButton)
            {
                return edge;
            }
        }
        return null;
    }
}