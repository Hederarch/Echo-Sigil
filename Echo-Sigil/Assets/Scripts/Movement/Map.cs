using System;
using UnityEngine;
using Pathfinding;

[Serializable]
public struct Map
{
    public bool readyForSave;
    public string name;
    public string quest;
    [NonSerialized]
    public int modPathIndex;

    //map data
    public int sizeX;
    public int sizeY;
    public MapTile[] mapTiles;
    /// <summary>
    /// Number of tiles in a given grid position
    /// </summary>
    public int[] numTile;


    public MapTile[] this[int x, int y]
    {
        get
        {
            if (x > sizeX || y > sizeY)
            {
                throw new IndexOutOfRangeException();
            }
            int startIndex = 0;
            for (int i = 0; i < (y * sizeX) + x; i++)
            {
                startIndex += numTile[i];
            }

            int length = numTile[(y * sizeX) + x];
            MapTile[] output = new MapTile[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = mapTiles[startIndex + i];
            }

            return output;
        }
    }

    public MapTile GetMapTile(int x, int y, float nearestHeight)
    {
        MapTile[] mapTiles = this[x, y];
        MapTile output = mapTiles[0];
        foreach (MapTile mapTile in mapTiles)
        {
            output = Mathf.Abs(nearestHeight - output.midHeight) < Mathf.Abs(nearestHeight - mapTile.midHeight) ? output : mapTile;
        }
        return output;
    }

    public Map(Vector2Int size) : this(size.x, size.y) { }

    public Map(int _sizeX, int _sizeY)
    {
        readyForSave = false;
        name = "";
        quest = "";
        modPathIndex = -1;

        sizeX = _sizeX;
        sizeY = _sizeY;
        mapTiles = new MapTile[_sizeX * _sizeY];
        numTile = new int[_sizeX * _sizeY];
        for (int i = 0; i < numTile.Length; i++)
        {
            numTile[i] = 1;
            mapTiles[i].topHeight = 1;
            mapTiles[i].bottomHeight = 0;
            mapTiles[i].walkable = true;
        }

    }

    public Map(Tile[] tiles, int[] numTile, int sizeX, int sizeY, Unit[] units)
    {
        this.numTile = numTile;
        this.sizeX = sizeX;
        this.sizeY = sizeY;


        readyForSave = false;
        name = "";
        quest = "";
        modPathIndex = -1;

        mapTiles = new MapTile[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            mapTiles[i] = (MapTile)tiles[i];
        }

        foreach (Unit unit in units)
        {
            MapTile mapTile = GetMapTile(unit.posInGrid.x, unit.posInGrid.y, unit.curHeight);
            mapTile.unit = (MapImplement)unit;
        }
    }
}
[Serializable]
public class MapImplement
{
    public string name;
    public TilePos posInGrid;

    public bool player;

    public MovementSettings movementSettings;
    public BattleSettings battleSettings;

    [Serializable]
    public struct MovementSettings
    {





    }
    [Serializable]
    public struct BattleSettings
    {



        public BattleSettings(IBattle battle)
        {

        }

    }

    public MapImplement(string name, TilePos posInGrid, MovementSettings movementSettings, BattleSettings battleSettings, bool player = true)
    {
        this.name = name;

        this.player = player;

        this.movementSettings = movementSettings;
        this.battleSettings = battleSettings;
    }

    public MapImplement(string name, TilePos posInGrid)
    {
        this.name = name;
        this.posInGrid = posInGrid;

        player = false;

        movementSettings = new MovementSettings();
        battleSettings = new BattleSettings();
    }

    public static explicit operator MapImplement(Unit v)
    {
        throw new NotImplementedException();
    }
}
[Serializable]
public struct MapTile
{
    public int pallateIndex;

    public MapTile(float pos, int pallateIndex, bool walkable = true, int weight = 1, MapImplement mapImplement = null)
    {
        this.pallateIndex = pallateIndex;

        topHeight = pos + .5f;
        bottomHeight = pos + .5f;

        this.walkable = walkable;
        this.weight = weight;

        unit = mapImplement;
    }

    public MapTile(float top, float bottom, int pallateIndex, bool walkable = true, int weight = 1, MapImplement mapImplement = null)
    {
        this.pallateIndex = pallateIndex;

        topHeight = top;
        bottomHeight = bottom;

        this.walkable = walkable;
        this.weight = weight;

        unit = mapImplement;
    }

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

    public bool walkable;
    public int weight;

    public MapImplement unit;

    public static Tile ConvertTile(MapTile mapTile, int x, int y)
    {
        Tile tile = new Tile(x, y, mapTile.topHeight, mapTile.pallateIndex , mapTile.walkable, mapTile.weight);
        tile.bottomHeight = mapTile.bottomHeight;
        tile.spriteIndex = mapTile.pallateIndex;

        return tile;
    }

    /// <summary>
    /// This opperation does not include units
    /// </summary>
    public static explicit operator MapTile(Tile tile) => new MapTile(tile.topHeight, tile.bottomHeight, tile.spriteIndex, tile.walkable, tile.weight);
}
