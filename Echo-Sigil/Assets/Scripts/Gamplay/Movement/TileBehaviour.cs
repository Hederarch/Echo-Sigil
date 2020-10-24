using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileBehaviour : MonoBehaviour
{
    public Tile tile;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.color = tile.CheckColor();
    }

    public static implicit operator Tile(TileBehaviour t) => t.tile;

}

public class Tile
{
    public Vector2Int PosInGrid;
    public Vector2 PosInWorld { get => MapReader.GridToWorldSpace(PosInGrid); }
    public float topHeight;
    public float bottomHeight;
    public float midHeight => (topHeight + bottomHeight) / 2f;
    public float sideLength
    {
        get => topHeight - bottomHeight;
        set
        {
            float mid = midHeight;
            topHeight = mid + (value / 2f);
            bottomHeight = mid - (value / 2f);
        }
    }

    public int spriteIndex;

    public bool walkable = true;
    public bool current;
    public bool target;
    public bool selectable;

    //BFS stuff
    public bool visited;
    public Tile parent;
    public int distance;

    //A* stuff
    public float f => g + h;
    public float g;
    public float h;

    public Tile(int x, int y) : this(new Vector2Int(x, y)) { }

    public Tile(Vector2Int posInGrid)
    {
        PosInGrid = posInGrid;
    }

    public Color CheckColor()
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
            output -= Color.white / 2f;
            output.a = 1;
        }
        return output;
    }

    /// <summary>
    /// Reset tile and then add neighbors to adjaceny list
    /// </summary>
    /// <param name="jumpHeight">Distance up and down before tiles stop being neighbors</param>
    /// <param name="target">Tile discounted for direction check.</param>
    public Tile[] FindNeighbors(float jumpHeight, Tile target = null)
    {
        return new Tile[4] {
        FindNeighbor(Vector2Int.up, jumpHeight, target),
        FindNeighbor(Vector2Int.down, jumpHeight, target),
        FindNeighbor(Vector2Int.left, jumpHeight, target),
        FindNeighbor(Vector2Int.right, jumpHeight, target)
        };
    }

    public Tile FindNeighbor(Vector2Int direction, float jumpHeight, Tile target)
    {
        throw new NotImplementedException();
    }

    public bool DirectionCheck()
    {
        bool output = true;
        //TODO
        /*if(Physics.Raycast(transform.position,Vector3.back,out RaycastHit hit))
        {
            output = false;
        }*/
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

        g = 0;
        h = 0;
    }


}

