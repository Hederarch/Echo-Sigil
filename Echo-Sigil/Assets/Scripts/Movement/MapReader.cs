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
        DestroyPhysicalMapTiles();

        tileParent = new GameObject("Tile Parent").transform;
        sizeX = map.sizeX;
        tiles = new Tile[map.sizeX * map.sizeY];
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

                    int index = 0;
                    for (int a = 0; a < y * map.sizeX + x; a++)
                    {
                        index += map.numTile[a];
                    }
                    index += i;
                    tiles[index] = tile;

                    MapImplementToUnit(mapTile.unit);

                    MapGeneratedEvent?.Invoke();
                }
            }
        }

        foreach (Tile tile in tiles)
        {
            TileToTileBehavior(tile);
        }
    }

    private static void TileToTileBehavior(Tile tile)
    {
        if (tile.topHeight >= 0)
        {
            GameObject gameObjectTile = new GameObject(tile.posInGrid.x + "," + tile.posInGrid.y + " tile");
            gameObjectTile.transform.position = tile.PosInWorld;
            gameObjectTile.transform.rotation = Quaternion.identity;
            gameObjectTile.transform.parent = tileParent;
            gameObjectTile.tag = "Tile";

            TileBehaviour tileBehaviour = gameObjectTile.AddComponent<TileBehaviour>();
            tileBehaviour.tile = tile;

            GameObject topSprite = new GameObject(tile.posInGrid.x + "," + tile.posInGrid.y + " top sprite");
            topSprite.transform.parent = gameObjectTile.transform;
            topSprite.transform.localPosition = Vector3.forward * (tile.sideLength / 2f);
            SpriteRenderer spriteRenderer = topSprite.AddComponent<SpriteRenderer>();
            tileBehaviour.spriteRenderer = spriteRenderer;
            spriteRenderer.sprite = TileTextureManager.GetTileSprite(tile.spriteIndex, TileTextureSection.Top, Vector2Int.zero, tile);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (Mathf.Abs(x) != Mathf.Abs(y))
                    {
                        Vector2Int direction = new Vector2Int(x, y);
                        Tile neighbor = tile.FindNeighbor(direction);
                        if (neighbor == null || neighbor != null && neighbor.topHeight < tile.topHeight && tile.topHeight > 0)
                        {
                            GameObject sideSprite = new GameObject(tile.posInGrid.x + "," + tile.posInGrid.y + " side sprite (" + x + "," + y + ")");
                            sideSprite.transform.parent = gameObjectTile.transform;
                            sideSprite.transform.localPosition = new Vector3(x / 2f, y / 2f, 0);
                            sideSprite.transform.localRotation = Quaternion.LookRotation(new Vector3(x, y, 0), -Vector3.forward);
                            sideSprite.AddComponent<SpriteRenderer>().sprite = TileTextureManager.GetTileSide(tile.spriteIndex, direction, tile);
                        }
                    }
                }
            }



            gameObjectTile.transform.position += Vector3.forward * tile.midHeight;

            gameObjectTile.AddComponent<BoxCollider2D>().size = new Vector3(1, 1, tile.sideLength);


        }
    }

    public static Unit MapImplementToUnit(MapImplement mi)
    {
        if (mi != null)
        {
            GameObject unit = new GameObject(mi.name);
            Vector2 pos = GridToWorldSpace(mi.posInGrid);
            unit.transform.parent = tileParent;
            Tile tile = GetTile(mi.posInGrid, 0);
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

    public static Vector3 GridToWorldSpace(TilePos posInGrid)
    {
        Vector2 realitivePosition = new Vector2(posInGrid.x - mapHalfSize.x, posInGrid.y - mapHalfSize.y);
        return new Vector2(tileParent.transform.position.x, tileParent.transform.position.y) - realitivePosition;
    }

    public static Vector3 GridToWorldSpace(int x, int y, float z) => GridToWorldSpace(new TilePos(x, y, z));

    public static TilePos WorldToGridSpace(Vector3 posInWorld)
    {
        Vector3 realitivePosition = posInWorld - tileParent.position;
        return new TilePos((int)Math.Abs(realitivePosition.x - mapHalfSize.x - .5f), (int)Math.Abs(realitivePosition.y - mapHalfSize.y - .5f), realitivePosition.z);
    }

    public static TilePos WorldToGridSpace(float x, float y, float z = 1) => WorldToGridSpace(new Vector3(x, y, z));

    public static Tile[] GetTiles(int x, int y)
    {
        if (x > sizeX || y > sizeY || x < 0 || y < 0)
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

    public static Tile GetTile(Vector2Int pos, float height) => GetTile(pos.x, pos.y, height);

    public static Tile GetTile(int x, int y, float nearestHeight)
    {
        Tile[] tiles = GetTiles(x, y);
        if (tiles.Length > 0)
        {
            Tile output = tiles[0];
            foreach (Tile tile in tiles)
            {
                output = Mathf.Abs(nearestHeight - output.midHeight) < Mathf.Abs(nearestHeight - tile.midHeight) ? output : tile;
            }
            return output;
        }
        else
        {
            return null;
        }
    }

    public static void DestroyPhysicalMapTiles()
    {
        if (tileParent != null)
        {
            UnityEngine.Object.DestroyImmediate(tileParent.gameObject);
        }
    }

}
