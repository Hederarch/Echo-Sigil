using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
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
    public int sizeY => rowIndexes.Length;
    [SerializeField]
    public MapTile[] mapTiles;
    /// <summary>
    /// Cumulative number of tiles in all rows before spesified row
    /// </summary>
    [SerializeField]
    private int[] rowIndexes;
    /// <summary>
    /// Number of tiles in a given grid position
    /// </summary>
    [SerializeField]
    private int[] numTile;


    public MapTile[] this[int x, int y]
    {
        get
        {
            int startIndex = rowIndexes[y];
            for (int i = 0; i < x; i++)
            {
                startIndex += numTile[y * sizeX + i];
            }

            int length = numTile[y * sizeX + x];
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
        mapTiles = new MapTile[_sizeX * _sizeY];
        numTile = new int[_sizeX * _sizeY];
        for (int i = 0; i < numTile.Length; i++)
        {
            numTile[i] = 1;
        }
        rowIndexes = new int[_sizeY];
        for (int i = 0; i < _sizeY; i++)
        {
            rowIndexes[i] = _sizeX * i;
        }
    }

    public Map(Tile[] tiles, int[] rowIndexes, int[] numTile, int sizeX, Unit[] units)
    {
        this.rowIndexes = rowIndexes;
        this.numTile = numTile;
        this.sizeX = sizeX;

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
    public int posX;
    public int posY;
    public Vector2Int PosInGrid
    {
        get => new Vector2Int(posX, posY); set
        {
            posX = value.x;
            posY = value.y;
        }
    }

    public bool player;

    public MovementSettings movementSettings;
    public BattleSettings battleSettings;

    [Serializable]
    public struct MovementSettings
    {



        public MovementSettings(IPathFollower<ITile> pathFollower)
        {

        }

    }
    [Serializable]
    public struct BattleSettings
    {



        public BattleSettings(IBattle battle)
        {

        }

    }

    public MapImplement(string _name, int x, int y, MovementSettings _movementSettings, BattleSettings _battleSettings, bool _player = true)
    {
        name = _name;

        posX = x;
        posY = y;

        player = _player;

        movementSettings = _movementSettings;
        battleSettings = _battleSettings;

    }

    public MapImplement(string name, Vector2Int posInGrid, MovementSettings movementSettings, BattleSettings battleSettings, bool player = true) :
        this(name, posInGrid.x, posInGrid.y, movementSettings, battleSettings, player)
    { }

    public MapImplement(string _name, Vector2Int posInGrid)
    {
        name = _name;
        posX = posInGrid.x;
        posY = posInGrid.y;

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

    public MapTile(float pos, int pallateIndex, MapImplement mapImplement = null)
    {
        this.pallateIndex = pallateIndex;

        topHeight = pos + .5f;
        bottomHeight = pos + .5f;

        walkable = true;

        unit = mapImplement;
    }

    public MapTile(float top, float bottom, int pallateIndex, MapImplement mapImplement = null)
    {
        this.pallateIndex = pallateIndex;

        topHeight = top;
        bottomHeight = bottom;

        walkable = true;

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

    public MapImplement unit;

    public static Tile ConvertTile(MapTile mapTile, int x, int y)
    {
        Tile tile = new Tile(x, y);
        tile.bottomHeight = mapTile.bottomHeight;
        tile.topHeight = mapTile.topHeight;
        tile.spriteIndex = mapTile.pallateIndex;

        return tile;
    }

    /// <summary>
    /// This opperation does not include units
    /// </summary>
    public static explicit operator MapTile(Tile tile) => new MapTile(tile.topHeight, tile.bottomHeight, tile.spriteIndex);
}
