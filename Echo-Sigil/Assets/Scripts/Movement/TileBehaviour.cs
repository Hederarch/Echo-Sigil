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
        if (spriteRenderer != null)
        {
            spriteRenderer.color = tile.CheckColor();
        }
    }

    public static implicit operator Tile(TileBehaviour t) => t.tile;

}

public class Tile : ITile
{
    //Rendering help
    int x;
    int y;
    public TilePos posInGrid
    {
        get => new TilePos(x, y, topHeight);
        set
        {
            x = value.x;
            y = value.y;
            topHeight = value.z;
        }
    }

    public Vector3 PosInWorld => MapReader.GridToWorldSpace(posInGrid);
    public float topHeight;
    public float bottomHeight;
    public float midHeight => (topHeight + bottomHeight) / 2f;
    public float sideLength
    {
        get => topHeight - Mathf.Max(bottomHeight, 0);
        set
        {
            float mid = midHeight;
            topHeight = mid + (value / 2f);
            bottomHeight = mid - (value / 2f);
        }
    }

    public int spriteIndex;

    //State managment
    public bool current { get; set; }
    public bool target { get; set; }
    public bool selectable { get; set; }
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
    public void ResetTile()
    {
        current = false;
        target = false;
        selectable = false;
    }

    //Pathfinding stuff
    public ITile parent { get; set; }
    public int weight { get; set; }
    public bool walkable { get; set; } = true;
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
                    if (tile != null)
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
        Tile tile = MapReader.GetTile(posInGrid + direction);
        if (tile != null && tile.walkable)
        {
            return tile;
        }
        return null;
    }
    public Func<List<ITile>, List<ITile>> OnFindNeighbors { get; set; }

    //BFS stuff
    public int distance { get; set; }

    //A* stuff
    public int F => G + H;
    public int G { get; set; }
    public int H { get; set; }
    public int GetDistance(ITile other) => Mathf.Abs(posInGrid.x - other.posInGrid.x) + Mathf.Abs(posInGrid.y - other.posInGrid.y);

    //Heap stuff
    public int HeapIndex { get; set; }
    public int GetMaxSize() => MapReader.numTiles;
    public int CompareTo(ITile other)
    {
        int compare = F.CompareTo(other.F);
        if (compare == 0)
        {
            compare = H.CompareTo(other.H);
        }
        return -compare;
    }

    //Constructors
    public Tile(int x, int y, float z, int spriteIndex, bool walkable = true, int _weight = 1) : this(new TilePos(x, y, z), spriteIndex, walkable, _weight) { }

    public Tile(TilePos posInGrid, int spriteIndex, bool walkable = true, int _weight = 1)
    {
        this.posInGrid = posInGrid;
        this.spriteIndex = spriteIndex;
        this.walkable = walkable;
        weight = _weight;
    }

    public override string ToString()
    {
        return posInGrid + " Tile";
    }
}

public interface ITile : IAStarItem<ITile>, IBFSItem<ITile>
{
    bool current { get; set; }
    bool target { get; set; }
    bool selectable { get; set; }

    TilePos posInGrid { get; }
    Func<List<ITile>, List<ITile>> OnFindNeighbors { get; set; }
}

[Serializable]
public struct TilePos : IEquatable<TilePos>, IEquatable<Vector2Int>
{
    public int x;
    public int y;
    public float z;
    public Vector2Int PosInGrid => new Vector2Int(x, y);

    public TilePos(int x, int y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public TilePos(Vector2Int posInGrid, float z)
    {
        x = posInGrid.x;
        y = posInGrid.y;
        this.z = z;
    }

    public bool Equals(TilePos other)
    {
        return other.x == x && other.y == y && other.z == z;
    }

    public bool Equals(Vector2Int other)
    {
        return other.x == x && other.y == y;
    }

    public static implicit operator Vector2Int(TilePos t) => t.PosInGrid;

    public static implicit operator string(TilePos t) => t.ToString();

    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }

    public static TilePos operator +(TilePos a, Vector2Int b)
    {
        return new TilePos(a.x + b.x, a.y + b.y, a.z);
    }

    public static TilePos operator -(TilePos a, Vector2Int b)
    {
        return new TilePos(a.x - b.x, a.y - b.y, a.z);
    }
}