using System;
using UnityEngine;

[Serializable]
public class NodePath
{
    [SerializeField] private Transform[] nodes;

    private int currentIndex;
    private int direction = 1;

    public Vector3 CurrentPosition => nodes[currentIndex].position;

    public void ResetToStart()
    {
        currentIndex = 0;
        direction = 1;
    }
    
    public Vector3 Advance(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            AdvanceOne();
        }

        return CurrentPosition;
    }

    private void AdvanceOne()
    {
        int next = currentIndex + direction;

        if (next < 0 || next >= nodes.Length)
        {
            direction = -direction;
            next = currentIndex + direction;
        }

        currentIndex = next;
    }
}