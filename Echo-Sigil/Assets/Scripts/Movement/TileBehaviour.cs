using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TileBehaviour : MonoBehaviour
{
    public Tile tile;
    public SpriteRenderer spriteRenderer;

    private void Update()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = tile.CheckColor();
        }
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
    public Func<List<ITile>, List<ITile>> OnFindNeighbors { get; set; }

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

    public ITile[] FindNeighbors()
    {
        List<ITile> tiles = new List<ITile>();

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (Mathf.Abs(x) != Mathf.Abs(y))
                {
                    Tile tile = FindNeighbor(new Vector2Int(x, y));
                    if(tile != null)
                    {
                        tiles.Add(tile);
                    }
                }
            }
        }

        if (OnFindNeighbors != null)
        {
            tiles = OnFindNeighbors(tiles);
        }

        return tiles.ToArray();
    }

    public Tile FindNeighbor(Vector2Int direction)
    {
        Tile tile = MapReader.GetTile(posInGrid + direction, topHeight);
        if(tile != null && tile.walkable)
        {
            return tile;
        }
        return null;
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
    Func<List<ITile>, List<ITile>> OnFindNeighbors { get; set; }
}