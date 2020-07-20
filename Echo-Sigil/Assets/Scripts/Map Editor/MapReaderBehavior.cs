using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapReaderBehavior : MonoBehaviour
{
    public bool addUnit = false;
}

public static class MapReader
{
    public static Transform tileParent;
    public static Vector2Int backupMapSize;
    public static Tile[,] tiles;
    public static List<Implement> implements = new List<Implement>();

    public static Sprite[] spritePallate;
    public static Map Map => new Map(tiles, implements.ToArray());

    public static Action<Sprite[]> MapGeneratedEvent;

    public static Tile[,] GeneratePhysicalMap(Sprite[] pallate, Map map = null)
    {
        DestroyPhysicalMapTiles();
        if (map == null)
        {
            map = new Map(backupMapSize.x, backupMapSize.y);
        }
        tileParent = new GameObject("Tile Parent").transform;
        tiles = new Tile[map.sizeX, map.sizeY];
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY / 2);
        spritePallate = pallate;
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

                    gameObjectTile.AddComponent<SpriteRenderer>().sprite = pallate[tile.spriteIndex];
                }
            }
        }

        if (map.units != null)
        {
            implements.Clear();
            foreach (MapImplement mi in map.units)
            {
                implements.Add(MapImplementToImplement(mi));
            }
        }

        MapGeneratedEvent?.Invoke(pallate);
        return tiles;
    }

    public static Implement MapImplementToImplement(MapImplement mi)
    {
        GameObject unit = new GameObject(mi.name);
        Vector2 pos = GridToWorldSpace(mi.PosInGrid());
        unit.transform.parent = tileParent;
        Tile tile = GetTile(mi.PosInGrid());
        unit.transform.position = new Vector3(pos.x, pos.y, tile.height - .1f);
        JRPGBattle j;
        TacticsMove t;
        Implement i;
        if (mi.player)
        {
            i = unit.AddComponent<PlayerImplement>();
            j = unit.AddComponent<PlayerBattle>();
            t = unit.AddComponent<PlayerMove>();
        }
        else
        {
            i = unit.AddComponent<NPCImplement>();
            j = unit.AddComponent<NPCBattle>();
            t = unit.AddComponent<NPCMove>();
        }
        i.battle = j;
        i.move = t;

        t.moveDistance = mi.moveDistance;
        t.moveSpeed = mi.moveSpeed;
        t.jumpHeight = mi.jumpHeight;

        j.health = mi.health;
        j.maxHealth = mi.maxHealth;
        j.will = mi.will;
        j.maxHealth = mi.maxWill;
        j.reach = mi.reach;
        j.abilites = new List<Ability>();
        if (mi.abilities != null)
        {
            foreach (Ability a in mi.abilities)
            {
                j.abilites.Add(a);
            }
        }

        GameObject spriteRender = new GameObject("Sprite Render");
        spriteRender.transform.parent = unit.transform;
        SpriteRenderer spriteRenderer = spriteRender.AddComponent<SpriteRenderer>();
        i.unitSprite = spriteRenderer;
        spriteRenderer.sprite = SaveSystem.LoadPNG(Application.dataPath + "/Implements/" + mi.name + "/Base.png",new Vector2(.5f,0));
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

    public static Vector2 GridToWorldSpace(int x, int y)
    {
        return GridToWorldSpace(new Vector2Int(x, y));
    }

    public static Vector2Int WorldToGridSpace(Vector2 posInGrid)
    {
        Vector2 mapHalfHeight = new Vector2(tiles.GetLength(0) / 2, tiles.GetLength(1) / 2);
        Vector2 realitivePosition = posInGrid - new Vector2(tileParent.position.x, tileParent.position.y);
        return new Vector2Int((int)Math.Abs(realitivePosition.x - mapHalfHeight.x - .5f), (int)Math.Abs(realitivePosition.y - mapHalfHeight.y - .5f));
    }

    public static Vector2Int WorldToGridSpace(float x, float y)
    {
        return WorldToGridSpace(new Vector2(x, y));
    }

    public static Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);

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
        GeneratePhysicalMap(spritePallate, map);
    }
}
