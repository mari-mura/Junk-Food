using UnityEngine;

public static class DemoMapBuilder
{
    public static PathGraph Build()
    {
        PathGraph graph = new PathGraph();

        // Main line, left to right
        graph.AddPoint("canStart", PathPointType.Tap, new Vector2(0.0f, 0.0f));
        graph.AddPoint("tire", PathPointType.Tap, new Vector2(2.0f, 0.0f));
        graph.AddPoint("puddleStart", PathPointType.HoldStart, new Vector2(4.0f, 0.0f));
        graph.AddPoint("puddleEnd", PathPointType.HoldEnd, new Vector2(6.0f, 0.0f));
        graph.AddPoint("rocks", PathPointType.Tap, new Vector2(8.0f, 0.0f));
        graph.AddPoint("apple", PathPointType.Chord, new Vector2(10.0f, 0.0f));
        graph.AddPoint("pathChoice", PathPointType.Choice, new Vector2(12.0f, 0.0f));

        // Path A goes up
        graph.AddPoint("canBranchA", PathPointType.Tap, new Vector2(12.0f, 2.0f));
        graph.AddPoint("afterBranchA", PathPointType.Tap, new Vector2(12.0f, 4.0f));

        // Path B goes right
        graph.AddPoint("bag", PathPointType.Tap, new Vector2(14.0f, 0.0f));
        graph.AddPoint("afterBranchB", PathPointType.Tap, new Vector2(16.0f, 0.0f));

        // Edges on the main line (the choice button is ignored on these)
        string unusedButton = "ButtonOne";
        graph.AddEdge("canStart", "tire", unusedButton);
        graph.AddEdge("tire", "puddleStart", unusedButton);
        graph.AddEdge("puddleStart", "puddleEnd", unusedButton);
        graph.AddEdge("puddleEnd", "rocks", unusedButton);
        graph.AddEdge("rocks", "apple", unusedButton);
        graph.AddEdge("apple", "pathChoice", unusedButton);

        // The fork. The first edge added is also the fallback if the player misses the choice
        graph.AddEdge("pathChoice", "canBranchA", "ButtonOne");
        graph.AddEdge("pathChoice", "bag", "ButtonTwo");

        // Continue each branch
        graph.AddEdge("canBranchA", "afterBranchA", unusedButton);
        graph.AddEdge("bag", "afterBranchB", unusedButton);

        Debug.Log(graph.ToString());
        return graph;
    }
}