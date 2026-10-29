using System.Reflection.Metadata;
using UnityEngine;

public static class DemoMapBuilder
{
    public static PathGraph Build()
    {
        PathGraph graph = new PathGraph();

        // Main line, left to right
        graph.AddPoint("canStart", PathPointType.Tap, new Vector2(0.0f, 0.0f));
        graph.AddPoint("chord", PathPointType.Chord, new Vector2(3.0f, 0.0f));
        graph.AddPoint("puddleStart", PathPointType.HoldStart, new Vector2(6.0f, 0.0f));
        graph.AddPoint("puddleEnd", PathPointType.HoldEnd, new Vector2(9.0f, 0.0f));
        graph.AddPoint("pathChoice", PathPointType.Choice, new Vector2(12.0f, 0.0f));
        graph.AddPoint("lastPoimt", PathPointType.Tap, new Vector2(20.0f, 0.0f));

        // Path A goes up
        graph.AddPoint("afterBranchA", PathPointType.Tap, new Vector2(12.0f, 4.0f));

        // Path B goes right
        graph.AddPoint("afterBranchB", PathPointType.Tap, new Vector2(16.0f, 0.0f));

        // Edges on the main line (the choice button is ignored on these)
        string unusedButton = "ButtonOne";
        graph.AddEdge("canStart", "chord", unusedButton);
        graph.AddEdge("chord", "puddleStart", unusedButton);
        graph.AddEdge("puddleStart", "puddleEnd", unusedButton);
        graph.AddEdge("puddleEnd", "pathChoice", unusedButton);
        graph.AddEdge("afterBranchB", "lastPoimt", unusedButton);

        // The fork. The first edge added is also the fallback if the player misses the choice
        graph.AddEdge("pathChoice", "afterBranchB", "ButtonOne");
        graph.AddEdge("pathChoice", "afterBranchA", "ButtonTwo");

        // Continue each branch

        Debug.Log(graph.ToString());
        return graph;
    }
}