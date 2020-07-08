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
        CorrectGridSize();
        Tile tile = new Tile(posInGrid);
        MapReader.tiles[posInGrid.x, posInGrid.y] = tile;
        MapReader.GeneratePhysicalMap(MapReader.spritePallate,MapReader.Map);
    }

    private void CorrectGridSize()
    {
        if (posInGrid.x < 0)
        {
            CorrectGridSizeInNegitiveX();
        }
        else if (posInGrid.x >= MapReader.tiles.GetLength(0))
        {
            CorrectGridSizeInPositiveX();
        }
        if (posInGrid.y < 0)
        {
            CorrectGridSizeInNegitiveY();
        }
        else if (posInGrid.y >= MapReader.tiles.GetLength(1))
        {
            CorrectGridSizeInPositiveY();
        }
    }

    private void CorrectGridSizeInPositiveY()
    {
        Tile[,] tiles = new Tile[MapReader.tiles.GetLength(0), posInGrid.y +1];
        for (int x = 0; x < MapReader.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < MapReader.tiles.GetLength(1); y++)
            {
                tiles[x, y] = MapReader.tiles[x, y];
            }
        }
        MapReader.tiles = tiles;
    }

    private void CorrectGridSizeInNegitiveY()
    {
        if (Math.Sign(posInGrid.y) != -1)
        {
            Debug.LogError("CorrectGridSizeInNegitiveY called with positive value. Aborting");
            return;
        }
        //always a negitive value
        Tile[,] tiles = new Tile[MapReader.tiles.GetLength(0), MapReader.tiles.GetLength(1) - posInGrid.y];
        for (int x = 0; x < MapReader.tiles.GetLength(0); x++)
        {
            for (int y = -posInGrid.y; y < MapReader.tiles.GetLength(1); y++)
            {

                tiles[x, y] = MapReader.GetTile(x, y + posInGrid.y);
                if (tiles[x, y] != null)
                {
                    tiles[x, y].PosInGrid = new Vector2Int(x, y);
                }
            }
        }
        MapReader.tiles = tiles;
        posInGrid.y = 0;
    }

    private void CorrectGridSizeInPositiveX()
    {
        Tile[,] tiles = new Tile[posInGrid.x + 1, MapReader.tiles.GetLength(1)];
        for (int x = 0; x < MapReader.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < MapReader.tiles.GetLength(1); y++)
            {
                tiles[x, y] = MapReader.tiles[x, y];
            }
        }
        MapReader.tiles = tiles;
    }

    private void CorrectGridSizeInNegitiveX()
    {
        if (Math.Sign(posInGrid.x) != -1)
        {
            Debug.LogError("CorrectGridSizeInNegitiveX called with positive value. Aborting");
            return;
        }
        //always a negitve value
        Tile[,] tiles = new Tile[MapReader.tiles.GetLength(0) - posInGrid.x, MapReader.tiles.GetLength(1)];
        for (int x = -posInGrid.x; x < MapReader.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < MapReader.tiles.GetLength(1); y++)
            {
                tiles[x, y] = MapReader.GetTile(x + posInGrid.x, y);
                if (tiles[x, y] != null)
                {
                    tiles[x, y].PosInGrid = new Vector2Int(x, y);
                }
            }
        }
        MapReader.tiles = tiles;
        posInGrid.x = 0;
    }
}
