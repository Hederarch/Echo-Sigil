using System;
using System.Collections;
using System.Collections.Generic;
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

    public Map(int sizeX, int sizeY, bool addUnit = false) : this(new Vector2Int(sizeX,sizeY),addUnit) { }

    public Map(Vector2Int vectorSize, bool addUnit = false)
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

        if (addUnit)
        {
            units = new List<MapImplement>();
            units.Add(new MapImplement(sizeX/2,sizeY/2));
        }
    }

    public Map(Tile[,] tiles, Implement[] units = null)
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
                heightMap[x, y] = tiles[x,y].height;
                walkableMap[x, y] = tiles[x,y].walkable;
                spriteIndexMap[x, y] = tiles[x, y].spriteIndex;
            }
        }
        if (units != null)
        {
            foreach (Implement i in units)
            {
                TacticsMove t = i.move as TacticsMove;
                JRPGBattle j = i.battle as JRPGBattle;
                this.units.Add(new MapImplement(
                    t.currentTile.PosInGrid,
                    i is PlayerImplement,
                    i.name,
                    t.moveDistance,
                    t.jumpHeight,
                    j.reach,
                    t.moveSpeed,
                    j.health,
                    j.will,
                    j.maxHealth,
                    j.maxWill,
                    j.abilites));
            }
        }
    }

    public Tile SetTileProperties(int x, int y)
    {
        Tile tile = new Tile
        {
            PosInGrid = new Vector2Int(x, y),
            height = heightMap[x, y],
            walkable = walkableMap[x,y],
            spriteIndex = spriteIndexMap[x,y]
        };
        return tile;
    }

}

public struct MapImplement
{
    public string name;

    public Vector2Int posInGrid;
    public bool player;

    //movement
    public int moveDistance;
    public float moveSpeed;
    public float jumpHeight;

    //battle
    public int health;
    public int maxHealth;
    public int will;
    public int maxWill;
    public int reach;
    //should this really be here?
    public Ability[] abilities;

    public MapImplement(int x,
                        int y,
                        bool player = true,
                        string name = "Unit",
                        int moveDistance = 3,
                        float jumpHeight = 2,
                        int reach = 1,
                        float moveSpeed = 4,
                        int health = 3,
                        int will = 5,
                        int maxHealth = 5,
                        int maxWill = 5,
                        List<Ability> abilities = null
                        )
    {
        posInGrid = new Vector2Int(x, y);

        this.player = player;
        this.name = name;
        this.moveDistance = moveDistance;
        this.moveSpeed = moveSpeed;
        this.jumpHeight = jumpHeight;
        this.health = health;
        this.maxHealth = maxHealth;
        this.will = will;
        this.maxWill = maxWill;
        this.reach = reach;
        if(abilities != null)
        {
            this.abilities = abilities.ToArray();
        } else
        {
            this.abilities = null;
        }

    }

    public MapImplement(
        Vector2Int posInGrid,
        bool player = true,
        string name = "Unit",
        int moveDistance = 3,
        float jumpHeight = 2,
        int reach = 1,
        float moveSpeed = 4,
        int health = 3,
        int will = 5,
        int maxHealth = 5,
        int maxWill = 5,
        List<Ability> abilities = null) :
        this(posInGrid.x,
             posInGrid.y,
             player,
             name,
             moveDistance,
             jumpHeight,
             reach,
             moveSpeed,
             health,
             will,
             maxHealth,
             maxWill,
             abilities)
    { }
}
