using mapEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Map
{
    //map data
    public int sizeX;
    public int sizeY;
    public float[,] heightMap;
    public bool[,] walkableMap;
    public int[,] spriteIndexMap;
    public List<MapImplement> units;
    internal string path = null;

    public Map(int sizeX, int sizeY, string path = null) : this(new Vector2Int(sizeX, sizeY), path) { }

    public Map(Vector2Int vectorSize, string path = null)
    {
        sizeX = vectorSize.x;
        sizeY = vectorSize.y;
        heightMap = new float[sizeX, sizeY];
        walkableMap = new bool[sizeX, sizeY];
        spriteIndexMap = new int[sizeX, sizeY];

        this.path = path == null ? Application.dataPath + "/Quests/Tests/Default.headrap" : path;

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

    public Map(Tile[,] tiles, Unit[] units = null, string path = null)
    {
        sizeX = tiles.GetLength(0);
        sizeY = tiles.GetLength(1);
        heightMap = new float[sizeX, sizeY];
        walkableMap = new bool[sizeX, sizeY];
        spriteIndexMap = new int[sizeX, sizeY];

        this.path = path == null ? Application.dataPath + "/Quests/Tests/Default.headrap" : path;

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
                MapImplement mapImplement = new MapImplement(t.currentTile.PosInGrid, t, j, i is PlayerUnit, i.implementListIndex);
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
    public int implementListIndex;
    public string GetName(ImplementList implementList) => implementList.implements[implementListIndex].name;

    public int posX;
    public int posY;
    public Vector2Int PosInGrid { get => new Vector2Int(posX, posY); set => SetPos(value); }
    private void SetPos(Vector2Int value)
    {
        posX = value.x;
        posY = value.y;
    }

    public bool player;

    public MovementSettings movementSettings;
    public BattleSettings battleSettings;

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

    public MapImplement(int x, int y, MovementSettings movementSettings, BattleSettings battleSettings, bool player = true, int index = 0)
    {
        posX = x;
        posY = y;

        this.player = player;
        implementListIndex = index;

        this.movementSettings = movementSettings;
        this.battleSettings = battleSettings;

    }

    public MapImplement(Vector2Int posInGrid, MovementSettings movementSettings, BattleSettings battleSettings, bool player = true, int index = 0) :
        this(posInGrid.x, posInGrid.y, movementSettings, battleSettings, player, index)
    { }

    public MapImplement(Vector2Int posInGrid)
    {
        posX = posInGrid.x;
        posY = posInGrid.y;

        player = false;
        implementListIndex = 0;

        movementSettings = new MovementSettings(5, 1, 2);
        battleSettings = new BattleSettings(5, 5, 1);
    }
}
