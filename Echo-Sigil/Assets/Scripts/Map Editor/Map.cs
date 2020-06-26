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
    public int spritePallate;
    public float[,] heightMap;
    public bool[,] walkableMap;
    public int[,] spriteIndexMap;

    public Map(int sizeX, int sizeY) : this(new Vector2Int(sizeX,sizeY)) { }

    public Map(Vector2Int vectorSize)
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

    public Map(Tile[,] tiles)
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
