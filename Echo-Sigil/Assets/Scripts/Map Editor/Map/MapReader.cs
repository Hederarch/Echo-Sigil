using MapEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MapReader
{
    public static Transform tileParent;
    public static Tile[,] tiles;
    public static List<Unit> implements = new List<Unit>();

    public static ImplementList implementList;
    public static Sprite[] spritePallate;

    public static Map Map => new Map(tiles, implements.ToArray());

    
    public static Action<Sprite[]> MapGeneratedEvent;

    public static Tile[,] GeneratePhysicalMap(Map map = null)
    {
        DestroyPhysicalMapTiles();
        if (map == null)
        {
            map = new Map(1, 1);
        }
        tileParent = new GameObject("Tile Parent").transform;
        tiles = new Tile[map.sizeX, map.sizeY];
        spritePallate = SaveSystem.LoadPallate(Directory.GetParent(map.path).FullName);
        implementList = SaveSystem.LoadImplementList(Directory.GetParent(Directory.GetParent(Directory.GetParent(map.path).FullName).FullName).FullName + "/Implements");

        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY / 2);

        for (int x = 0; x < map.sizeX; x++)
        {
            for (int y = 0; y < map.sizeY; y++)
            {
                Tile tile = map.SetTileProperties(x, y);

                if (tile.height >= 0)
                {
                    GameObject gameObjectTile = InstantateTileGameObject(mapHalfHeight, tile);

                    gameObjectTile.AddComponent<TileBehaviour>().tile = tile;
                    tiles[x, y] = tile;

                    gameObjectTile.transform.position += new Vector3(0, 0, tile.height);

                    gameObjectTile.AddComponent<BoxCollider2D>().size = new Vector3(1, 1, .2f);

                    gameObjectTile.AddComponent<SpriteRenderer>().sprite = tile.spriteIndex < spritePallate.Length ? spritePallate[tile.spriteIndex] : spritePallate[0];
                }
            }
        }

        implements.Clear();
        if (map.units != null)
        {
            foreach (MapImplement mi in map.units)
            {
                MapImplementToImplement(mi);
            }
        }

        MapGeneratedEvent?.Invoke(spritePallate);
        return tiles;
    }

    public static Unit MapImplementToImplement(MapImplement mi)
    {
        GameObject unit = new GameObject(mi.GetName(implementList));
        Vector2 pos = GridToWorldSpace(mi.PosInGrid);
        unit.transform.parent = tileParent;
        Tile tile = GetTile(mi.PosInGrid);
        unit.transform.position = new Vector3(pos.x, pos.y, tile.height - .1f);
        JRPGBattle j;
        TacticsMove t;
        Unit i;
        if (mi.player)
        {
            i = unit.AddComponent<PlayerUnit>();
            j = unit.AddComponent<PlayerBattle>();
            t = unit.AddComponent<PlayerMove>();
        }
        else
        {
            i = unit.AddComponent<NPCUnit>();
            j = unit.AddComponent<NPCBattle>();
            t = unit.AddComponent<NPCMove>();
        }
        i.battle = j;
        i.move = t;

        t.SetValues(mi.movementSettings);

        j.SetValues(mi.battleSettings);

        GameObject spriteRender = new GameObject("Sprite Render");
        spriteRender.transform.parent = unit.transform;
        spriteRender.transform.localPosition = new Vector3(0, 0, .1f);
        SpriteRenderer spriteRenderer = spriteRender.AddComponent<SpriteRenderer>();
        i.unitSprite = spriteRenderer;
        spriteRender.AddComponent<Animator>().runtimeAnimatorController = implementList.Implements[mi.implementListIndex].GetAnimationController(implementList.modPath);
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        spriteRender.AddComponent<BoxCollider2D>().size = new Vector3(1, 1, .2f);

        implements.Add(i);

        return i;
    }

    private static GameObject InstantateTileGameObject(Vector2 mapHalfHeight, Tile tile)
    {
        GameObject gameObjectTile = new GameObject(tile.PosInGrid.x + "," + tile.PosInGrid.y + " tile");
        gameObjectTile.transform.position = new Vector3(mapHalfHeight.x - tile.PosInGrid.x, mapHalfHeight.y - tile.PosInGrid.y);
        gameObjectTile.transform.rotation = Quaternion.identity;
        gameObjectTile.transform.parent = tileParent;
        gameObjectTile.tag = "Tile";
        return gameObjectTile;
    }

    public static Vector2 GridToWorldSpace(Vector2Int posInGrid)
    {
        Vector2 mapHalfHeight = new Vector2(tiles.GetLength(0) / 2, tiles.GetLength(1) / 2);
        Vector2 realitivePosition = new Vector2(posInGrid.x - mapHalfHeight.x, posInGrid.y - mapHalfHeight.y);
        return new Vector2(tileParent.transform.position.x, tileParent.transform.position.y) - realitivePosition;
    }

    public static Vector2 GridToWorldSpace(int x, int y) => GridToWorldSpace(new Vector2Int(x, y));

    public static Vector2Int WorldToGridSpace(Vector2 posInWorld)
    {
        Vector2 mapHalfHeight = new Vector2(tiles.GetLength(0) / 2, tiles.GetLength(1) / 2);
        Vector2 realitivePosition = posInWorld - new Vector2(tileParent.position.x, tileParent.position.y);
        return new Vector2Int((int)Math.Abs(realitivePosition.x - mapHalfHeight.x - .5f), (int)Math.Abs(realitivePosition.y - mapHalfHeight.y - .5f));
    }

    public static Vector2Int WorldToGridSpace(float x, float y) => WorldToGridSpace(new Vector2(x, y));
    /// <summary>
    /// Get a tile in the array of tiles
    /// </summary>
    /// <returns>Null if out of array bounds</returns>
    public static Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);
    /// <summary>
    /// Get a tile in the array of tiles
    /// </summary>
    /// <returns>Null if out of array bounds</returns>
    public static Tile GetTile(int x, int y) => x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1) ? tiles[x, y] : null;

    public static void DestroyPhysicalMapTiles()
    {
        if (tileParent != null)
        {
            UnityEngine.Object.DestroyImmediate(tileParent.gameObject);
        }
    }

    public static void SaveMap(string path, Sprite[] pallate)
    {
        SaveSystem.SaveMap(path, Map);
        SaveSystem.SavePallate(Directory.GetParent(path).FullName, pallate);
    }

    public static void LoadMap(string path, Sprite[] spritePallate = null)
    {
        Map map = SaveSystem.LoadMap(path, true);
        if(spritePallate == null)
        {
            spritePallate = SaveSystem.LoadPallate(Directory.GetParent(path).FullName);
        }
        GeneratePhysicalMap(map);
    }
}
