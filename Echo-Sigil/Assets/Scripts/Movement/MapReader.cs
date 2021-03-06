﻿using MapEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MapReader
{
    public static Transform tileParent;

    public static bool lodeing = false;

    public static int sizeX;
    public static int sizeY;
    public static Vector2Int Size => new Vector2Int(sizeX, sizeY);
    public static Vector2 mapHalfSize => new Vector2(sizeX / 2, sizeY / 2);

    private static Tile[] tiles;
    /// <summary>
    /// Number of tiles in a given grid position
    /// </summary>
    private static int[] numTile;
    public static List<Unit> implements = new List<Unit>();

    public static Texture2D[] spritePallate;

    public static Map Map => new Map(tiles, numTile, sizeX, sizeY, implements.ToArray());

    public static int numTiles
    {
        get
        {
            int size = 0;
            foreach (int i in numTile)
            {
                size += i;
            }
            return size;
        }
    }

    public static Action MapGeneratedEvent;

    public static void GeneratePhysicalMap(Map map)
    {
        ResetTileParent();

        spritePallate = map.readyForSave ? SaveSystem.LoadPallate(map.modPathIndex, map.quest) : TileTextureManager.GetDebugPallate();

        foreach (Tile tile in tiles)
        {
            if (tile!= null && tile.topHeight > 0)
            {
                TileBehaviour.GetTileBehaviour(tile, tileParent);
            }
        }
        TileBehaviour.ClearCachedTiles();
        MapGeneratedEvent?.Invoke();
    }

    public static void GenerateVirtualMap(Map map)
    {
        ResetTileParent();

        sizeX = map.sizeX;
        sizeY = map.sizeY;
        tiles = new Tile[map.sizeX * map.sizeY];
        numTile = map.numTile;
        implements.Clear();

        foreach (MapTilePair mapTilePair in map)
        {
            tiles[mapTilePair.index] = MapTile.ConvertTile(mapTilePair.mapTile, mapTilePair.tilePos.x, mapTilePair.tilePos.y);
            Unit.GetUnit(mapTilePair.mapTile.unit);
        }
    }

    private static void ResetTileParent()
    {
        if (tileParent == null)
        {
            tileParent = new GameObject("Tile Parent").transform;
        }
        else
        {
            foreach (Transform transform in tileParent)
            {
                if (transform.TryGetComponent(out TileBehaviour t) && t.cached)
                {
                    t.CacheTile();
                }
            }
        }
    }

    public static Vector3 GridToWorldSpace(TilePos posInGrid)
    {
        Vector3 realitivePosition = new Vector3(posInGrid.x - mapHalfSize.x, posInGrid.y - mapHalfSize.y, 0);
        return new Vector3(tileParent.transform.position.x, tileParent.transform.position.y, posInGrid.z) - realitivePosition;
    }

    public static Vector3 GridToWorldSpace(int x, int y, float z) => GridToWorldSpace(new TilePos(x, y, z));

    public static TilePos WorldToGridSpace(Vector3 posInWorld)
    {
        Vector3 realitivePosition = posInWorld - tileParent.position;
        return new TilePos((int)Math.Abs(realitivePosition.x - mapHalfSize.x - .5f), (int)Math.Abs(realitivePosition.y - mapHalfSize.y - .5f), realitivePosition.z);
    }

    public static TilePos WorldToGridSpace(float x, float y, float z) => WorldToGridSpace(new Vector3(x, y, z));

    public static Tile[] GetTiles(int x, int y)
    {
        if (x >= sizeX || y >= sizeY || x < 0 || y < 0)
        {
            return new Tile[0];
        }
        int startIndex = 0;
        for (int i = 0; i < y * sizeX + x; i++)
        {
            startIndex += numTile[i];
        }

        int length = numTile[y * sizeX + x];
        Tile[] output = new Tile[length];
        for (int i = 0; i < length; i++)
        {
            output[i] = tiles[startIndex + i];
        }

        return output;
    }

    public static Tile GetTile(TilePos pos) => GetTile(pos.x, pos.y, pos.z);

    public static Tile GetTile(int x, int y, float nearestHeight)
    {
        Tile[] tiles = GetTiles(x, y);
        if (tiles.Length > 0)
        {
            Tile output = tiles[0];
            foreach (Tile tile in tiles)
            {
                output = Mathf.Abs(nearestHeight - output.topHeight) < Mathf.Abs(nearestHeight - tile.topHeight) ? output : tile;
            }
            return output;
        }
        else
        {
            return null;
        }
    }

}
