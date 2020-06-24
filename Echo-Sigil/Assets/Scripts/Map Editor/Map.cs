using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map 
{
    //map data
    public Vector2Int size;
    public float[,] heightmap;
    public int[,] spriteIndex;
    public bool[,] walkable;

    public Map(int sizeX, int sizeY) : this(new Vector2Int(sizeX,sizeY)) { }

    public Map(Vector2Int vectorSize)
    {
        size = vectorSize;
        heightmap = new float[size.x, size.y];
        walkable = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                heightmap[x, y] = 0f;
                walkable[x, y] = true;
            }
        }
    }

    public Map(Tile[,] tiles)
    {
        size = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
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
