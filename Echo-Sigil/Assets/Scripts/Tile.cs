using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current;
    public bool target;
    public bool selectable;

    public List<Tile> adjacencyList = new List<Tile>();

    //BFS stuff
    public bool visited;
    public Tile parent;
    public int distance;

    private void Update()
    {
        GetComponent<SpriteRenderer>().color = CheckColor();
    }

    private Color CheckColor()
    {
        Color output = Color.white;
        if (selectable)
        {
            output = Color.green;
        }
        if (target)
        {
            output = Color.red;
        }
        if (current)
        {
            output = Color.blue;
        }
        if (!walkable)
        {
            output = Color.black;
        }
        return output;
    }

    public void FindNeighbors(float jumpHeight)
    {
        ResetTile();
        adjacencyList.Clear();
        FindNeighbor(Vector3.up, jumpHeight);
        FindNeighbor(Vector3.down, jumpHeight);
        FindNeighbor(Vector3.left, jumpHeight);
        FindNeighbor(Vector3.right, jumpHeight);
    }

    void FindNeighbor(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtents = new Vector3(.25f, .25f, (1 + jumpHeight) / 2);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);
        foreach(Collider collider in colliders)
        {
            Tile tile = collider.GetComponent<Tile>();
            if(tile != null && tile.walkable)
            {
                if (tile.DirectionCheck())
                {
                    adjacencyList.Add(tile);
                }
            } 
        }
    }

    private bool DirectionCheck()
    {
        bool output = true;
        if(Physics.Raycast(transform.position,Vector3.back,out RaycastHit hit))
        {
            output = false;
        }
        return output;
    }

    public void ResetTile()
    {
        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;
    }

}
