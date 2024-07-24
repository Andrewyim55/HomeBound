using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class Node : MonoBehaviour
{
    public enum NodeType
    {
        NONE,
        FLOOR,
        WALL
    }

    public Vector3 position;
    public NodeType nodeType;
    public Node cameFromNode;

    public int gCost;
    public int hCost;
    public int fCost;

    public int x;
    public int y;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public Node(Vector3 pos, NodeType type)
    {
        position = pos;
        nodeType = type;
    }
}