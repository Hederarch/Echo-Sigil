using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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

public class Tile : ITile
{
    public Vector2Int posInGrid;
    Vector2Int ITile.posInGrid => posInGrid;
    public Vector2 PosInWorld { get => MapReader.GridToWorldSpace(posInGrid); }
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
    bool IPathItem<ITile>.walkable => walkable;
    public bool current;
    public bool target;
    public bool selectable;

    //Pathfinding stuff
    public ITile parent { get; set; }
    public int weight { get; set; }

    //BFS stuff
    public bool visited { get; set; }
    public int distance { get; set; }

    //A* stuff
    public int F => G + H;
    public int G { get; set; }
    public int H { get; set; }

    //Heap stuff
    public int HeapIndex { get; set; }

    

    public Tile(int x, int y, int _weight = 1) : this(new Vector2Int(x, y), _weight) { }

    public Tile(Vector2Int posInGrid, int _weight = 0)
    {
        this.posInGrid = posInGrid;
        weight = _weight;
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
    public ITile[] FindNeighbors()
    {
        return new Tile[4] {
        FindNeighbor(Vector2Int.up),
        FindNeighbor(Vector2Int.down),
        FindNeighbor(Vector2Int.left),
        FindNeighbor(Vector2Int.right)
        };
    }

    public Tile FindNeighbor(Vector2Int direction)
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

        G = 0;
        H = 0;
    }

    public int CompareTo(ITile other)
    {
        int compare = F.CompareTo(other.F);
        if(compare == 0)
        {
            compare = H.CompareTo(other.H);
        }
        return -compare;
    }

    public int GetDistance(ITile other)
    {
        return Mathf.Abs(posInGrid.x - other.posInGrid.x) + Mathf.Abs(posInGrid.y - other.posInGrid.y);
    }

    public int GetMaxSize()
    {
        return MapReader.numTiles;
    }
}

public interface ITile : IAStarItem<ITile>, IBFSItem<ITile>
{
    Vector2Int posInGrid { get; }
}