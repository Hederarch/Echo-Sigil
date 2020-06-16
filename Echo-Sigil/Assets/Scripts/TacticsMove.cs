using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : FacesTacticticsCamera
{
    public bool moveing;


    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();

    public bool isTurn;
    public int moveDistance = 3;
    public float jumpHeight = 2;

    public Tile currentTile;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();



    public void FindSelectableTiles()
    {
        ComputeAdjacencyList();
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = ???

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();
            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < moveDistance)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void ComputeAdjacencyList()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight);
        }
    }

    public Tile GetCurrentTile()
    {
        currentTile = GetTargetTile(transform.position);
        currentTile.current = true;
        return currentTile;
    }

    public Tile GetTargetTile(Vector3 targetPosition)
    {
        Tile output = null;
        if(Physics.Raycast(targetPosition,Vector3.forward,out RaycastHit hit) && hit.collider.GetComponent<Tile>())
        {
            Debug.DrawLine(targetPosition, hit.point);
            output = hit.collider.GetComponent<Tile>();
        }
        return output;
    }

    public void MoveToTile(Tile targetTile)
    {
        path.Clear();
        targetTile.target = true;

        Tile next = targetTile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
        moveing = true;
    }

    public void Move()
    {
        if(path.Count > 0)
        {
            Tile t = path.Peek();
            if (Vector2.Distance(transform.position, t.transform.position) >= .5f)
            {
                transform.position = new Vector3(t.transform.position.x, t.transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(t.transform.position.x, t.transform.position.y, transform.position.z);
                path.Pop();
            }
        }
        else
        {
            moveing = false;
        }
    }
}
