using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EditorTile : MonoBehaviour
{
    internal Vector2Int posInGrid;

    public void AddTileToMapReader()
    {
        posInGrid = CorrectGridSize(posInGrid);
        Tile tile = new Tile(posInGrid);
        MapReader.tiles[posInGrid.x, posInGrid.y] = tile;
        MapReader.GeneratePhysicalMap(MapReader.spritePallate, MapReader.Map);
    }
    /// <summary>
    /// Changes the size of a tile array to allow a position to exist
    /// </summary>
    /// <param name="posInGrid">Tile that should have a place to be</param>
    /// <returns>position in grid, corrected for negitive values</returns>
    public static Vector2Int CorrectGridSize(Vector2Int posInGrid)
    {
        if (posInGrid.x < 0)
        {
            MapReader.tiles = CorrectGridSizeInNegitiveX(posInGrid, MapReader.tiles);
            posInGrid.x = 0;
        }
        else if (posInGrid.x >= MapReader.tiles.GetLength(0))
        {
            MapReader.tiles = CorrectGridSizeInPositiveX(posInGrid, MapReader.tiles);
        }
        if (posInGrid.y < 0)
        {
            MapReader.tiles = CorrectGridSizeInNegitiveY(posInGrid, MapReader.tiles);
            posInGrid.y = 0;
        }
        else if (posInGrid.y >= MapReader.tiles.GetLength(1))
        {
            MapReader.tiles = CorrectGridSizeInPositiveY(posInGrid, MapReader.tiles);
        }
        return posInGrid;
    }

    private static Tile[,] CorrectGridSizeInPositiveY(Vector2Int posInGrid, Tile[,] startTiles)
    {
        Tile[,] tiles = new Tile[startTiles.GetLength(0), posInGrid.y + 1];
        for (int x = 0; x < startTiles.GetLength(0); x++)
        {
            for (int y = 0; y < startTiles.GetLength(1); y++)
            {
                tiles[x, y] = startTiles[x, y];
            }
        }
        return tiles;
    }

    private static Tile[,] CorrectGridSizeInNegitiveY(Vector2Int posInGrid, Tile[,] startTiles)
    {
        if (Math.Sign(posInGrid.y) != -1)
        {
            Debug.LogError("CorrectGridSizeInNegitiveY called with positive value. Aborting");
            return startTiles;
        }
        //always a negitive value
        Tile[,] tiles = new Tile[startTiles.GetLength(0), startTiles.GetLength(1) - posInGrid.y];
        for (int x = 0; x < startTiles.GetLength(0); x++)
        {
            for (int y = -posInGrid.y; y <= startTiles.GetLength(1); y++)
            {

                tiles[x, y] = GetTile(x, y + posInGrid.y, startTiles);
                if (tiles[x, y] != null)
                {
                    tiles[x, y].PosInGrid = new Vector2Int(x, y);
                }
            }
        }
        return tiles;
    }

    private static Tile[,] CorrectGridSizeInPositiveX(Vector2Int posInGrid, Tile[,] startTiles)
    {
        Tile[,] tiles = new Tile[posInGrid.x + 1, startTiles.GetLength(1)];
        for (int x = 0; x < startTiles.GetLength(0); x++)
        {
            for (int y = 0; y < startTiles.GetLength(1); y++)
            {
                tiles[x, y] = startTiles[x, y];
            }
        }
        return tiles;
    }

    private static Tile[,] CorrectGridSizeInNegitiveX(Vector2Int posInGrid, Tile[,] startTiles)
    {
        if (Math.Sign(posInGrid.x) != -1)
        {
            Debug.LogError("CorrectGridSizeInNegitiveX called with positive value. Aborting");
            return startTiles;
        }
        //always a negitve value
        Tile[,] tiles = new Tile[startTiles.GetLength(0) - posInGrid.x, startTiles.GetLength(1)];
        for (int x = -posInGrid.x; x <= MapReader.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < startTiles.GetLength(1); y++)
            {
                tiles[x, y] = GetTile(x + posInGrid.x, y, startTiles);
                if (tiles[x, y] != null)
                {
                    tiles[x, y].PosInGrid = new Vector2Int(x, y);
                }
            }
        }
        return tiles;
    }

    private static Tile GetTile(int x, int y, Tile[,] tiles) => x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1) ? tiles[x, y] : null;

}