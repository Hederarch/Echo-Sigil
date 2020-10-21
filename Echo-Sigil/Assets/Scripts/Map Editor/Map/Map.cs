using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapEditor
{
    [Serializable]
    public class Map
    {
        public string name;
        public string quest;
        [NonSerialized]
        public int modPathIndex;
        public string modPath { get => SaveSystem.GetModPaths()[modPathIndex]; }

        //map data
        public int sizeX;
        public int sizeY;
        public float[,] heightMap;
        public bool[,] walkableMap;
        public int[,] spriteIndexMap;
        public List<MapImplement> units;

        public Map(int sizeX, int sizeY, string path = null) : this(new Vector2Int(sizeX, sizeY), path) { }

        public Map(Vector2Int vectorSize, string path = null)
        {
            sizeX = vectorSize.x;
            sizeY = vectorSize.y;
            heightMap = new float[sizeX, sizeY];
            walkableMap = new bool[sizeX, sizeY];
            spriteIndexMap = new int[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    heightMap[x, y] = 0f;
                    walkableMap[x, y] = true;
                    spriteIndexMap[x, y] = 0;
                }
            }
        }

        public Map(Tile[,] tiles, Unit[] units = null)
        {
            sizeX = tiles.GetLength(0);
            sizeY = tiles.GetLength(1);
            heightMap = new float[sizeX, sizeY];
            walkableMap = new bool[sizeX, sizeY];
            spriteIndexMap = new int[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (tiles[x, y] != null)
                    {
                        heightMap[x, y] = tiles[x, y].height;
                        walkableMap[x, y] = tiles[x, y].walkable;
                        spriteIndexMap[x, y] = tiles[x, y].spriteIndex;
                    }
                    else
                    {
                        heightMap[x, y] = -1;
                        walkableMap[x, y] = true;
                        spriteIndexMap[x, y] = 0;
                    }
                }
            }
            if (units != null && units.Length > 0)
            {
                this.units = new List<MapImplement>();
                foreach (Unit i in units)
                {
                    TacticsMove t = i.move as TacticsMove;
                    JRPGBattle j = i.battle as JRPGBattle;
                    MapImplement mapImplement = new MapImplement(i.name, t.currentTile.PosInGrid, t, j, i is PlayerUnit);
                    this.units.Add(mapImplement);
                }
            }
        }

        public Tile SetTileProperties(int x, int y)
        {
            Tile tile = new Tile(x, y)
            {
                height = heightMap[x, y],
                walkable = walkableMap[x, y],
                spriteIndex = spriteIndexMap[x, y]
            };
            return tile;
        }

    }
    [Serializable]
    public struct MapImplement
    {
        public string name;
        public Implement GetImplement(int modPathIndex)
        {
            return SaveSystem.LoadImplement(modPathIndex, name);
        }
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
            public int distance;
            public float speed;
            public float jumpHeight;

            public MovementSettings(int distance, float speed, float jumpHeight)
            {
                this.distance = distance;
                this.speed = speed;
                this.jumpHeight = jumpHeight;
            }

            public MovementSettings(TacticsMove tacticsMove)
            {
                distance = tacticsMove.moveDistance;
                speed = tacticsMove.moveSpeed;
                jumpHeight = tacticsMove.jumpHeight;
            }

            public static implicit operator MovementSettings(TacticsMove tacticsMove) => new MovementSettings(tacticsMove);

        }
        [Serializable]
        public struct BattleSettings
        {
            public int health;
            public int maxHealth;
            public int will;
            public int maxWill;
            public int reach;

            public Ability[] abilities;
            public int[] abilityAnimationIndex;

            public BattleSettings(int maxHealth, int maxWill, int reach, Ability[] abilities = null, int[] indexes = null)
            {
                health = maxHealth;
                this.maxHealth = maxHealth;
                will = maxWill;
                this.maxWill = maxWill;
                this.reach = reach;
                this.abilities = abilities;
                abilityAnimationIndex = indexes;
            }

            public BattleSettings(int health, int maxHealth, int will, int maxWill, int reach, Ability[] abilities = null, int[] indexes = null)
            {
                this.health = health;
                this.maxHealth = maxHealth;
                this.will = will;
                this.maxWill = maxWill;
                this.reach = reach;
                this.abilities = abilities;
                abilityAnimationIndex = indexes;
            }

            public BattleSettings(JRPGBattle jRPGBattle)
            {
                health = jRPGBattle.health;
                maxHealth = jRPGBattle.maxHealth;
                will = jRPGBattle.will;
                maxWill = jRPGBattle.maxWill;
                reach = jRPGBattle.reach;

                abilities = jRPGBattle.abilites != null ? jRPGBattle.abilites.Keys.ToArray() : null;
                abilityAnimationIndex = jRPGBattle.abilites != null ? new int[jRPGBattle.abilites.Count] : null;

            }

            public static implicit operator BattleSettings(JRPGBattle jRPGBattle) => new BattleSettings(jRPGBattle);
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

            movementSettings = new MovementSettings(5, 1, 2);
            battleSettings = new BattleSettings(5, 5, 1);
        }
    }

}