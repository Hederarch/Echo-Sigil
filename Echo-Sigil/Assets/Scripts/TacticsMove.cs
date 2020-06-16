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

    public virtual void Start()
    {
        halfHeight = GetComponent<Collider>().bounds.extents.z;

        TurnManager.AddUnit(this);
    }

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

            TurnManager.EndTurn();
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

    public void BeginTurn()
    {
        isTurn = true;
        Camera.main.GetComponent<TacticsCamera>().foucus = this;
    }

    public void EndTurn()
    {
        isTurn = false;
    }
}
