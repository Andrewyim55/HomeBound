using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Pathfinding : MonoBehaviour
{
    public static Pathfinding instance;

    private List<Tilemap> tilemaps;
    private List<Node> openList;
    private List<Node> closedList;
    public Grid<Node> grid;

    public float nodeSize;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
        }
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
        bool isWall = false;
        bool isFloor = false;

        foreach (Tilemap tilemap in tilemaps)
        {
            TileBase tile = tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                // Determine node type by the layer
                int layer = tilemap.gameObject.layer;
                // The number is the layer number set in unity
                if (layer == 10)
                {
                    isWall = true;
                }
                else if (layer == 9)
                {
                    isFloor = true;
                }
            }
        }

        if (isWall)
        {
            return Node.NodeType.WALL;
        }
        else if (isFloor)
        {
            return Node.NodeType.FLOOR;
        }

        return Node.NodeType.NONE;
    }

    public List<Node> FindPath(int startX, int startY, int endX, int endY, float enemyColliderRadius)
    {
        Node startNode = grid.GetGridObject(startX, startY);
        Node endNode = grid.GetGridObject(endX, endY);

        // If end node is a wall, find the closest walkable node
        if (endNode.nodeType == Node.NodeType.WALL || !IsNodeWalkable(endNode, enemyColliderRadius))
        {
            endNode = GetClosestWalkableNode(endNode, enemyColliderRadius);
        }

        openList = new List<Node> { startNode };
        closedList = new List<Node>();

        // Iterate through the grid using nested for loops
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

                if (neighborNode.nodeType == Node.NodeType.WALL || !IsNodeWalkable(neighborNode, enemyColliderRadius))
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

        // No path found
        return null;
    }
    private bool IsNodeWalkable(Node node, float colliderRadius)
    {
        // Calculate how many nodes around this node should be checked based on collider radius
        int checkRadius = Mathf.CeilToInt(colliderRadius / nodeSize);

        for (int dx = -checkRadius; dx <= checkRadius; dx++)
        {
            for (int dy = -checkRadius; dy <= checkRadius; dy++)
            {
                int checkX = node.x + dx;
                int checkY = node.y + dy;

                if (checkX < 0 || checkX >= grid.GetWidth() || checkY < 0 || checkY >= grid.GetHeight())
                {
                    return false;
                }

                Node checkNode = grid.GetGridObject(checkX, checkY);
                if (checkNode.nodeType == Node.NodeType.WALL)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Node GetClosestWalkableNode(Node currentNode, float colliderRadius)
    {
        Queue<Node> nodeQueue = new Queue<Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        nodeQueue.Enqueue(currentNode);
        visitedNodes.Add(currentNode);

        while (nodeQueue.Count > 0)
        {
            Node node = nodeQueue.Dequeue();

            if (IsNodeWalkable(node, colliderRadius))
            {
                return node;
            }

            foreach (Node neighbor in GetNeighborList(node))
            {
                if (!visitedNodes.Contains(neighbor))
                {
                    nodeQueue.Enqueue(neighbor);
                    visitedNodes.Add(neighbor);
                }
            }
        }
        // If no walkable node is found, return the original node
        return currentNode;
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

    private Node GetClostestNode(Node currentNode)
    {
        Queue<Node> nodeQueue = new Queue<Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        nodeQueue.Enqueue(currentNode);
        visitedNodes.Add(currentNode);

        while (nodeQueue.Count > 0)
        {
            Node node = nodeQueue.Dequeue();

            // Check if this node is not a wall
            if (node.nodeType != Node.NodeType.WALL)
            {
                return node;
            }

            // Enqueue all non-visited neighbors
            foreach (Node neighbor in GetNeighborList(node))
            {
                if (!visitedNodes.Contains(neighbor))
                {
                    nodeQueue.Enqueue(neighbor);
                    visitedNodes.Add(neighbor);
                }
            }
        }
        return null;
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