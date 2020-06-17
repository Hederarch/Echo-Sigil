using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool moveing;


    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();

    public bool isTurn;
    public int moveDistance = 3;
    public float moveSpeed = 4;
    public float jumpHeight = 2;

    public Tile currentTile;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight;

    private bool movingEdge;
    private Vector3 jumpTarget;
    private bool jummpingUp;
    private bool fallingDown;
    public float jumpVelocity = 4.5f;

    Tile actualTargetTile;

    public event Action EndMoveEvent;

    void Start()
    {
        halfHeight = GetComponent<Collider>().bounds.extents.z;
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(jumpHeight, null);
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

    public void ComputeAdjacencyList(float jumpHeight, Tile targetTile)
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, targetTile);
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
            Vector3 target = t.transform.position;

            target.z -= halfHeight + t.GetComponent<Collider>().bounds.extents.z + .1f;

            if (Vector2.Distance(transform.position, target) >= .05f)
            {
                bool jump = transform.position.z != target.z;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }

                //locomation/animation
                //transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moveing = false;

            EndMoveEvent?.Invoke();
        }
    }

    private void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    private void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    protected void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach(Tile tile in selectableTiles)
        {
            tile.ResetTile();
        }

        selectableTiles.Clear();
    }

    private void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        } 
        else if (jummpingUp)
        {
            JumpUpward(target);
        } 
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    private void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) > .5f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 3.0f;
            velocity.z = 1.5f;
        }
    }

    private void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.z > target.z)
        {
            jummpingUp = false;
            fallingDown = true;
        }
    }

    private void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.z < target.z)
        {
            fallingDown = false;

            Vector3 p = transform.position;
            p.z = target.z;
            transform.position = p;

            velocity = new Vector3();
        }

    }

    private void PrepareJump(Vector3 target)
    {
        float targetZ = target.z;

        target.z = transform.position.z;

        CalculateHeading(target);

        if(transform.position.z > targetZ)
        {
            fallingDown = false;
            jummpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + (target - transform.position) / 2;
        } 
        else
        {
            fallingDown = false;
            jummpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            float diffrence = targetZ - transform.position.z;

            velocity.z = jumpVelocity * (0.5f + diffrence / 2);
        }
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;
        if (t.DirectionCheck())
        {
            next = t;
        } 

        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= moveDistance)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i <= moveDistance; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;

    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyList(jumpHeight, target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currentTile);
        //currentTile.parent = ??
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            Tile t = FindLowestFCost(openList);

            closedList.Add(t);

            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //dont do anything, already processed
                } 
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                } 
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }

        Debug.LogError("Path Not Found!");
    }

    private Tile FindLowestFCost(List<Tile> openList)
    {
        //outmoded by a priority Queue
        Tile lowest = openList[0];

        foreach(Tile t in openList)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        openList.Remove(lowest);

        return lowest;
    }
}
