using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    private List<Tilemap> tilemaps;
    // Create double array to store the data of the grids
    private Node[,] grid;

    private List<Node> openList;
    private List<Node> closeList;
    public float nodeSize = 1f;

    private int rows;
    private int columns;

    // Start is called before the first frame update
    void Start()
    {
        tilemaps = new List<Tilemap>();
        tilemaps.AddRange(GetComponentsInChildren<Tilemap>());
    }

    
}