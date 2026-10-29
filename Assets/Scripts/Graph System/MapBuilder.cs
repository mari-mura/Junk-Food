using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public struct Connection
    {
        public string FromPointId;
        public string ToPointId;
        public string Button;

        public Connection(string fromPointId, string toPointId, string button)
        {
            FromPointId = fromPointId;
            ToPointId = toPointId;
            Button = button;
        }
    }

    public PathGraph Build()
    {
        PathGraph pathGraph = new PathGraph();

        List<GameObject> pathPointObjects =
            new List<GameObject>(GameObject.FindGameObjectsWithTag("PathPoint"));
        List<PathPoint> pathPoints = new List<PathPoint>();

        foreach (GameObject pathPointObject in pathPointObjects)
        {
            PathPoint pathPoint = pathPointObject.GetComponent<PathPoint>();
            pathGraph.AddPoint(pathPoint);
            pathPoints.Add(pathPoint);
        }

        foreach (PathPoint pathPoint in pathPoints)
        {
            foreach (PathConnection connection in pathPoint.Connections)
            {
                pathGraph.AddEdge(pathPoint.UniquePointId, connection.TargetPointId, connection.Button);
            }
        }

        return pathGraph;
    }
}
