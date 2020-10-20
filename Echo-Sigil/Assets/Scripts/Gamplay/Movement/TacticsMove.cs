using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour , IMovement
{
    public bool moveing;

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();

    public bool IsTurn { get; set; }
    public int moveDistance = 3;
    public float moveSpeed = 4;
    public float jumpHeight = 2;

    public Tile currentTile { get => MapReader.GetTile(MapReader.WorldToGridSpace(transform.position.x,transform.position.y)); }

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    //Jumping
    private bool movingEdge;
    private Vector3 jumpTarget;
    private bool jummpingUp;
    private bool fallingDown;
    public float jumpVelocity = 4.5f;

    Tile actualTargetTile;

    public bool CanMove => GetCanMove();

    public event Action EndEvent;

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(jumpHeight, null);

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
            Tile t = tile.GetComponent<TileBehaviour>().tile;
            t.ResetTile();
            t.FindNeighbors(jumpHeight, targetTile);
        }
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
            Vector3 target = t.PosInWorld;

            target.z -= t.topHeight + .1f;

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

            IsTurn = false;
            EndEvent?.Invoke();
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

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currentTile);
        //currentTile.parent = ??
        currentTile.h = Vector3.Distance(currentTile.PosInWorld, target.PosInWorld);
        //currentTile.f = currentTile.h;

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
                    float tempG = t.g + Vector3.Distance(tile.PosInWorld, t.PosInWorld);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                    }
                } 
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.PosInWorld, t.PosInWorld);
                    tile.h = Vector3.Distance(tile.PosInWorld, target.PosInWorld);

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

    public bool GetCanMove()
    {
        if (currentTile != null)
        {
            currentTile.FindNeighbors(jumpHeight, null);
            if (currentTile.adjacencyList.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void SetValues(MapEditor.MapImplement.MovementSettings movementSettings)
    {
        moveDistance = movementSettings.distance;
        moveSpeed = movementSettings.speed;
        jumpHeight = movementSettings.jumpHeight;
    }
}
