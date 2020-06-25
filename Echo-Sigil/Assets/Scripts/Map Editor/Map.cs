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
    public float[,] heightmap;
    public int[,] spriteIndex;
    public bool[,] walkable;

    public Map(int sizeX, int sizeY) : this(new Vector2Int(sizeX,sizeY)) { }

    public Map(Vector2Int vectorSize)
    {
        sizeX = vectorSize.x;
        sizeY = vectorSize.y;
        heightmap = new float[sizeX, sizeY];
        walkable = new bool[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                heightmap[x, y] = 0f;
                walkable[x, y] = true;
            }
        }
    }

    public Map(Tile[,] tiles)
    {
        sizeX = tiles.GetLength(0); 
        sizeY = tiles.GetLength(1);
        heightmap = new float[tiles.GetLength(0), tiles.GetLength(1)];

        for (int x = 0; x < heightmap.GetLength(0); x++)
        {
            for (int y = 0; y < heightmap.GetLength(1); y++)
            {
                heightmap[x, y] = tiles[x,y].height;

            }
        }
    }
    public Tile SetTileProperties(int x, int y)
    {
        Tile tile = new Tile
        {
            PosInGrid = new Vector2Int(x, y),
            height = heightmap[x, y]
        };
        return tile;
    }

}
