using System;
using UnityEngine;
using Pathfinding;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;

namespace TileMap
{
    [Serializable]
    public struct Map : IEnumerable<MapTilePair>, IEnumerable<MapTile>
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

        public IEnumerator<MapTilePair> GetEnumerator()
        {
            return new MapTilePairEnum(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<MapTile> IEnumerable<MapTile>.GetEnumerator()
        {
            return new MapTilePairEnum(this);
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
                if (numTile.Length / 2 == i)
                {
                    mapTiles[i].unit = new MapImplement("Test", new TilePos(sizeX / 2, sizeY / 2, 1));
                    mapTiles[i].unit.player = true;
                }
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
                MapTile mapTile = GetMapTile(unit.posInGrid.x, unit.posInGrid.y, unit.posInGrid.z);
                mapTile.unit = (MapImplement)unit;
            }
        }

        public Map(string name, string quest, int modPathIndex, Tile[] tiles, int[] numTile, int sizeX, int sizeY, Unit[] units) : this(tiles, numTile, sizeX, sizeY, units)
        {
            this.name = name;
            this.quest = quest;
            this.modPathIndex = modPathIndex;
            readyForSave = name != "" && quest != "";

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
            Tile tile = new Tile(x, y, mapTile.topHeight, mapTile.pallateIndex, mapTile.walkable, mapTile.weight);
            tile.bottomHeight = mapTile.bottomHeight;
            tile.spriteIndex = mapTile.pallateIndex;

            return tile;
        }

        /// <summary>
        /// This opperation does not include units
        /// </summary>
        public static explicit operator MapTile(Tile tile) => new MapTile(tile.topHeight, tile.bottomHeight, tile.spriteIndex, tile.walkable, tile.weight);
    }

    public struct MapTilePair
    {
        public MapTile mapTile;
        public TilePos tilePos;
        public int index;

        public MapTilePair(MapTile mapTile, TilePos tilePos, int index)
        {
            this.mapTile = mapTile;
            this.tilePos = tilePos;
            this.index = index;
        }

        public override string ToString()
        {
            return tilePos + " at " + index;
        }
    }

    class MapTilePairEnum : IEnumerator<MapTilePair>, IEnumerator<MapTile>
    {
        Map map;

        public MapTilePairEnum(Map map)
        {
            this.map = map;
        }

        public MapTilePair Current => new MapTilePair(map.mapTiles[tileIndex], new TilePos(x, y, map.mapTiles[tileIndex].topHeight), tileIndex);

        object IEnumerator.Current => Current;

        MapTile IEnumerator<MapTile>.Current => map.mapTiles[tileIndex];

        int numIndex;
        int tileIndex = -1;
        int i = -1;

        int x;
        int y;

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            i++;
            tileIndex++;
            if (i >= map.numTile[numIndex])
            {
                x++;
                if (x >= map.sizeX)
                {
                    y++;
                    if (y >= map.sizeY)
                    {
                        return false;
                    }
                    x = 0;
                }
                numIndex++;
                i = 0;
            }
            return tileIndex < map.mapTiles.Length;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    } 
}