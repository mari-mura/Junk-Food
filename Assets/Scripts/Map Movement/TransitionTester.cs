using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionTester : MonoBehaviour
{
    [SerializeField] private string startingNodeId = "StartPoint";
    
    [SerializeField] private Transform player;

    [SerializeField] private Key advanceKey = Key.Space;
    [SerializeField] private Key depositKey = Key.E;
    [SerializeField] private Key withdrawKey = Key.R;
    
    [SerializeField] private InventoryData playerInventory;

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

        MovePlayerToCurrentNode();
        
        MapTransitioner.ArriveAtWorldNode(SceneManager.GetActiveScene().name, currentNodeId);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard[depositKey].wasPressedThisFrame)
        {
            UseCache(true);
        }

        if (keyboard[withdrawKey].wasPressedThisFrame)
        {
            UseCache(false);
        }

        if (keyboard[advanceKey].wasPressedThisFrame)
        {
            AdvanceAlongFirstEdge();
        }
    }
    
    private void MovePlayerToCurrentNode()
    {

        WorldMapData worldMap = Maps.GetWorldMapData(SceneManager.GetActiveScene().name);

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId == currentNodeId)
            {
                player.position = new Vector3(node.Position.x, node.Position.y, player.position.z);
                return;
            }
        }
    }

    private void UseCache(bool deposit)
    {
        string worldMapId = SceneManager.GetActiveScene().name;

        if (CacheNodes.IsCache(worldMapId, currentNodeId) == false)
        {
            Debug.Log("Node " + currentNodeId + " is not a cache node");
            return;
        }

        int moved;
        if (deposit)
        {
            moved = CacheNodes.DepositAll(currentNodeId, playerInventory);
        }
        else
        {
            moved = CacheNodes.WithdrawAll(currentNodeId, playerInventory);
        }

        Debug.Log("Moved " + moved + " items " + (deposit ? "into" : "out of") + " cache " + currentNodeId);
    }

    private void AdvanceAlongFirstEdge()
    {
        string worldMapId = SceneManager.GetActiveScene().name;

        WorldMapData worldMap = Maps.GetWorldMapData(worldMapId);

        foreach (WorldNodeData node in worldMap.Nodes)
        {
            if (node.NodeId != currentNodeId)
            {
                continue;
            }

            if (node.Edges.Count == 0)
            {
                Debug.Log("Node " + currentNodeId + " has no way forward");
                return;
            }

            WorldEdgeData edge = node.Edges[0];

            if (string.IsNullOrEmpty(edge.LevelId))
            {
                currentNodeId = edge.TargetNodeId;
                Debug.Log("Walked to node " + currentNodeId);
                MovePlayerToCurrentNode();
                MapTransitioner.ArriveAtWorldNode(worldMapId, currentNodeId);
            }
            else
            {
                Debug.Log(currentNodeId + " -> " + edge.TargetNodeId + " plays level " + edge.LevelId);
                
                Debug.Log("Current node: " + node.NodeId);
                Debug.Log("Next level ID: " + edge.LevelId);
                Debug.Log("Next destination node: " + edge.TargetNodeId);
                
                MapTransitioner.MoveToRhythmMap(edge.LevelId, edge.TargetNodeId);
            }

            return;
        }
        
    }
}