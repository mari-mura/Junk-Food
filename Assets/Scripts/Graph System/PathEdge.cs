using UnityEngine;

public class PathEdge
{
    public PathPoint Source;
    public PathPoint Destination;
    public string ChoiceButton;

    public override string ToString()
    {
        return string.Format("{0} -> {1} (button: {2})", Source.UniquePointId, Destination.UniquePointId, ChoiceButton);
    }

    public PathEdge(PathPoint sourceInput, PathPoint destinationInput, string choiceButtonInput)
    {
        Source = sourceInput;
        Destination = destinationInput;
        ChoiceButton = choiceButtonInput;
    }

    public float GetLength()
    {
        return Vector2.Distance(Source.Position, Destination.Position);
    }

    public Vector2 GetDirection()
    {
        Vector2 offset = Destination.Position - Source.Position;
        return offset.normalized;
    }
}