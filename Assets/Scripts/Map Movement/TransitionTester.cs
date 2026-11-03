using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionTester : MonoBehaviour
{
    [SerializeField] private string startingNodeId = "StartPoint";
    [SerializeField] private Key startLevelKey = Key.Space;
    [SerializeField] private Transform playerSprite;

    private string currentNodeId;

    private void Start()
    {
        string arrivalNodeId;

        if (MapTransitioner.TryGetArrivalNode(out arrivalNodeId))
        {
            currentNodeId = arrivalNodeId;
            Debug.Log("Arrived at world node " + currentNodeId);
        }
        else
        {
            currentNodeId = startingNodeId;
            Debug.Log("Start at world node " + currentNodeId);
        }

        MapTransitioner.ArriveAtWorldNode(SceneManager.GetActiveScene().name, currentNodeId);

        UpdatePlayerPosition();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard[startLevelKey].wasPressedThisFrame)
        {
            StartNextLevel();
        }
    }

    private void UpdatePlayerPosition()
    {
        WorldMapData worldMap = Maps.GetWorldMapData(SceneManager.GetActiveScene().name);

        if (worldMap == null)
        {
            return;
        }

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId == currentNodeId)
            {
                playerSprite.position = node.Position;
                return;
            }
        }
    }

    private void StartNextLevel()
    {
        WorldMapData worldMap = Maps.GetWorldMapData(SceneManager.GetActiveScene().name);

        if (worldMap == null)
        {
            return;
        }

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId != currentNodeId)
            {
                continue;
            }

            foreach (WorldEdgeData edge in node.Edges)
            {
                if (string.IsNullOrEmpty(edge.LevelId) == false)
                {
                    Debug.Log(currentNodeId + " to " + edge.TargetNodeId + " plays level " + edge.LevelId);

                    MapTransitioner.MoveToRhythmMap(edge.LevelId, edge.TargetNodeId);
                    return;
                }
            }

            return;
        }
    }
}