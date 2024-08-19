using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class Node
{
    public enum NodeType
    {
        NONE = 0,
        FLOOR = 1,
        WALL = 2
    }

    public Grid<Node> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public NodeType nodeType;
    public Node cameFromNode;

    public Node(Grid<Node> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public int GetNodeTypeValue()
    {
        return (int)nodeType;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(x, y) * grid.GetCellSize() + grid.GetOriginPosition();
    }
}