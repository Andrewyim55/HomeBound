using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Pathfinding : MonoBehaviour
{
    private List<Tilemap> tilemaps;
    private List<Node> openList;
    private List<Node> closedList;
    public Grid<Node> grid;

    public float nodeSize;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        InitializeGrid();
    }

    void Start()
    {
        
    }
    private void InitializeGrid()
    {
        tilemaps = new List<Tilemap>();
        tilemaps.AddRange(GetComponentsInChildren<Tilemap>());

        // Determine the size of the grid
        BoundsInt combinedBounds = GetCombinedTilemapBounds(tilemaps);
        int rows = Mathf.CeilToInt(combinedBounds.size.x / nodeSize);
        int columns = Mathf.CeilToInt(combinedBounds.size.y / nodeSize);

        // Initialize the grid
        grid = new Grid<Node>(rows, columns, nodeSize, combinedBounds.min, (Grid<Node> g, int x, int y) => new Node(g, x, y));

        // Populate the grid with node types
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(x * nodeSize) + combinedBounds.xMin, Mathf.FloorToInt(y * nodeSize) + combinedBounds.yMin, 0);
                Node.NodeType nodeType = GetNodeType(tilemaps, cellPosition);
                grid.GetGridObject(x, y).nodeType = nodeType;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    Node node = grid.GetGridObject(x, y);
                    Gizmos.color = GetColorForNodeType(node.nodeType);
                    Gizmos.DrawCube(new Vector3(node.GetPosition().x + nodeSize / 2 , node.GetPosition().y + nodeSize / 2), Vector3.one * (grid.GetCellSize() * 0.4f));
                }
            }
        }
    }

    private Color GetColorForNodeType(Node.NodeType nodeType)
    {
        switch (nodeType)
        {
            case Node.NodeType.FLOOR:
                return Color.green;
            case Node.NodeType.WALL:
                return Color.red;
            default:
                return Color.gray;
        }
    }
    private BoundsInt GetCombinedTilemapBounds(List<Tilemap> tilemaps)
    {
        if (tilemaps.Count == 0)
            return new BoundsInt();

        BoundsInt combinedBounds = tilemaps[0].cellBounds;

        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            combinedBounds.xMin = Mathf.Min(combinedBounds.xMin, bounds.xMin);
            combinedBounds.yMin = Mathf.Min(combinedBounds.yMin, bounds.yMin);
            combinedBounds.xMax = Mathf.Max(combinedBounds.xMax, bounds.xMax);
            combinedBounds.yMax = Mathf.Max(combinedBounds.yMax, bounds.yMax);
        }

        return combinedBounds;
    }

    private Node.NodeType GetNodeType(List<Tilemap> tilemaps, Vector3Int cellPosition)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            TileBase tile = tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                // Sort the nodetype by the sorting layer
                int layer = tilemap.gameObject.layer;
                if (layer == 9)
                {
                    return Node.NodeType.FLOOR;
                }
                else if (layer == 10)
                {
                    return Node.NodeType.WALL;
                }
            }
        }
        return Node.NodeType.NONE;
    }

    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        Node startNode = grid.GetGridObject(startX, startY);
        Node endNode = grid.GetGridObject(endX, endY);

        openList = new List<Node> { startNode };
        closedList = new List<Node>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Node node = grid.GetGridObject(x, y);
                node.gCost = int.MaxValue;
                node.CalculateFCost();
                node.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Reached the end node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) continue;
                if (neighborNode.nodeType == Node.NodeType.WALL)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

private int CalculateDistanceCost(Node a, Node b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return 14 * Mathf.Min(xDistance, yDistance) + 10 * remaining;
    }

    private Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<Node> GetNeighborList(Node currentNode)
    {
        List<Node> neighborList = new List<Node>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y + 1));
        }
        if (currentNode.y - 1 >= 0)
        {
            // Down
            neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
        }
        if (currentNode.y + 1 < grid.GetHeight())
        {
            // Up
            neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));
        }

        return neighborList;
    }

    private List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
}