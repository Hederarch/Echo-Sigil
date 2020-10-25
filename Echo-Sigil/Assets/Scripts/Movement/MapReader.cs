using MapEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MapReader
{
    public static Transform tileParent;

    public static bool lodeing = false;

    public static int sizeX;
    public static int sizeY => rowIndexes.Length;
    public static Vector2Int Size => new Vector2Int(sizeX, sizeY);
    public static Vector2 mapHalfSize => new Vector2(sizeX / 2, sizeY / 2);

    private static Tile[] tiles;
    /// <summary>
    /// Cumulative number of tiles in all rows before spesified row
    /// </summary>
    private static int[] rowIndexes;
    /// <summary>
    /// Number of tiles in a given grid position
    /// </summary>
    private static int[] numTile;
    public static List<Unit> implements = new List<Unit>();

    public static Texture2D[] spritePallate;

    public static Map Map => new Map(tiles, rowIndexes, numTile, sizeX, implements.ToArray());

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
        DestroyPhysicalMapTiles();

        tileParent = new GameObject("Tile Parent").transform;
        sizeX = map.sizeX;
        tiles = new Tile[map.sizeX * map.sizeY];
        rowIndexes = new int[map.sizeY];
        numTile = new int[map.sizeX * map.sizeY];
        implements.Clear();

        spritePallate = map.readyForSave ? SaveSystem.LoadPallate(map.modPathIndex, map.quest) : TileTextureManager.GetDebugPallate();

        for (int y = 0; y < map.sizeY; y++)
        {
            for (int x = 0; x < map.sizeX; x++)
            {
                for (int i = 0; i < map[x, y].Length; i++)
                {
                    MapTile mapTile = map[x, y][i];
                    Tile tile = MapTile.ConvertTile(mapTile, x, y);

                    int index = rowIndexes[y];
                    for (int X = 0; X < x; X++)
                    {
                        index += numTile[y * sizeX + X];
                    }
                    index += i;
                    tiles[index] = tile;

                    TileToGameObject(tile);
                    MapImplementToUnit(mapTile.unit);

                    MapGeneratedEvent?.Invoke();
                }
            }
        }
    }

    private static void TileToGameObject(Tile tile)
    {
        if (tile.topHeight >= 0)
        {
            GameObject gameObjectTile = new GameObject(tile.posInGrid.x + "," + tile.posInGrid.y + " tile");
            gameObjectTile.transform.position = tile.PosInWorld;
            gameObjectTile.transform.rotation = Quaternion.identity;
            gameObjectTile.transform.parent = tileParent;
            gameObjectTile.tag = "Tile";

            gameObjectTile.AddComponent<TileBehaviour>().tile = tile;

            gameObjectTile.transform.position += new Vector3(0, 0, tile.topHeight);

            gameObjectTile.AddComponent<BoxCollider2D>().size = new Vector3(1, 1, .2f);


        }
    }

    public static Unit MapImplementToUnit(MapImplement mi)
    {
        if (mi != null)
        {
            GameObject unit = new GameObject(mi.name);
            Vector2 pos = GridToWorldSpace(mi.PosInGrid);
            unit.transform.parent = tileParent;
            Tile tile = GetTile(mi.PosInGrid, 0);
            unit.transform.position = new Vector3(pos.x, pos.y, tile.topHeight - .1f);
            Unit i;
            if (mi.player)
            {
                i = unit.AddComponent<PlayerUnit>();
            }
            else
            {
                i = unit.AddComponent<NPCUnit>();
            }

            i.SetValues(mi.movementSettings);

            i.SetValues(mi.battleSettings);

            GameObject spriteRender = new GameObject("Sprite Render");
            spriteRender.transform.parent = unit.transform;
            spriteRender.transform.localPosition = new Vector3(0, 0, .1f);
            SpriteRenderer spriteRenderer = spriteRender.AddComponent<SpriteRenderer>();
            i.unitSprite = spriteRenderer;
            spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            spriteRender.AddComponent<BoxCollider2D>().size = new Vector3(1, 1, .2f);

            implements.Add(i);

            return i;
        }
        return null;
    }

    public static Vector2 GridToWorldSpace(Vector2Int posInGrid)
    {
        Vector2 realitivePosition = new Vector2(posInGrid.x - mapHalfSize.x, posInGrid.y - mapHalfSize.y);
        return new Vector2(tileParent.transform.position.x, tileParent.transform.position.y) - realitivePosition;
    }

    public static Vector2 GridToWorldSpace(int x, int y) => GridToWorldSpace(new Vector2Int(x, y));

    public static Vector2Int WorldToGridSpace(Vector2 posInWorld)
    {
        Vector2 realitivePosition = posInWorld - new Vector2(tileParent.position.x, tileParent.position.y);
        return new Vector2Int((int)Math.Abs(realitivePosition.x - mapHalfSize.x - .5f), (int)Math.Abs(realitivePosition.y - mapHalfSize.y - .5f));
    }

    public static Vector2Int WorldToGridSpace(float x, float y) => WorldToGridSpace(new Vector2(x, y));

    public static Tile[] GetTiles(int x, int y)
    {
        int startIndex = rowIndexes[y];
        for (int i = 0; i < x; i++)
        {
            startIndex += numTile[y * sizeX + i];
        }

        int length = numTile[y * sizeX + x];
        Tile[] output = new Tile[length];
        for (int i = 0; i < length; i++)
        {
            output[i] = tiles[startIndex + i];
        }

        return output;
    }

    public static Tile GetTile(Vector2Int pos, float height) => GetTile(pos.x, pos.y, height);

    public static Tile GetTile(int x, int y, float nearestHeight)
    {
        Tile[] tiles = GetTiles(x, y);
        Tile output = tiles[0];
        foreach (Tile tile in tiles)
        {
            output = Mathf.Abs(nearestHeight - output.midHeight) < Mathf.Abs(nearestHeight - tile.midHeight) ? output : tile;
        }
        return output;
    }

    public static void DestroyPhysicalMapTiles()
    {
        if (tileParent != null)
        {
            UnityEngine.Object.DestroyImmediate(tileParent.gameObject);
        }
    }

}
