using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapReaderBehavior : MonoBehaviour
{
    public SpritePallate pallate = null;
    public bool addUnit = false;
    public string mapName = "Test";
}

public static class MapReader
{
    public static Transform tileParent;
    public static Map map;
    public static Vector2Int backupMapSize;
    public static Tile[,] tiles;
    public static List<Implement> implements;

    public static Tile[,] GeneratePhysicalMap(SpritePallate pallate = null, Map map = null, bool editor = false)
    {
        DestroyPhysicalMapTiles();
        if (map == null)
        {
            map = new Map(backupMapSize.x, backupMapSize.y);
        }
        MapReader.map = map;
        tileParent = new GameObject("Tile Parent").transform;
        tiles = new Tile[map.sizeX,map.sizeY];
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY /2);
        for (int x = 0; x < map.sizeX; x++)
        {
            for (int y = 0; y < map.sizeY; y++)
            {
                Tile tile = map.SetTileProperties(x, y);

                GameObject gameObjectTile = InstantateTileGameObject(mapHalfHeight, tile);

                gameObjectTile.AddComponent<TileBehaviour>().tile = tile;
                tiles[x, y] = tile;

                gameObjectTile.transform.position += new Vector3(0, 0, tile.height);

                gameObjectTile.AddComponent<BoxCollider>().size = new Vector3(1, 1, .2f);

                gameObjectTile.AddComponent<SpriteRenderer>().sprite = GetSpriteFromIndexAndPallete(tile.spriteIndex,pallate);

            }
        }

        if(map.units != null)
        {
            implements = new List<Implement>();
            foreach(MapImplement mi in map.units)
            {
               implements.Add(MapImplementToImplement(mi));
            }
        }

        if (editor)
        {
            GenerateGrowthTiles();
        }

        return tiles;
    }

    private static void GenerateGrowthTiles()
    {
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY / 2);
    }

    public static Implement MapImplementToImplement(MapImplement mi)
    {
        GameObject unit = new GameObject(mi.name);
        Vector2 pos = GridToWorldSpace(mi.PosInGrid());
        unit.transform.parent = tileParent;
        unit.transform.position = new Vector3(pos.x, pos.y, GetTile(mi.PosInGrid()).height + .2f);
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
        i.unitSprite = spriteRender.AddComponent<SpriteRenderer>();
        spriteRender.AddComponent<BoxCollider>();

        return i;
    }

    public static Sprite GetSpriteFromIndexAndPallete(int spriteIndex, SpritePallate spritePallate)
    {
        if(spritePallate != null && spriteIndex < spritePallate.sprites.Count())
        {
            Sprite sprite = spritePallate.sprites[spriteIndex];
            return sprite;
        } else
        {
            return null;
        }

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
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY / 2);
        Vector2 realitivePosition = new Vector2(posInGrid.x - mapHalfHeight.x, posInGrid.y - mapHalfHeight.y);
        return new Vector2(tileParent.transform.position.x, tileParent.transform.position.y) - realitivePosition;
    }

    public static Vector2 GridToWorldSpace(int x,int y)
    {
        return GridToWorldSpace(new Vector2Int(x, y));
    }

    public static Vector2Int WorldToGridSpace(Vector2 posInGrid)
    {
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2, map.sizeY / 2);
        Vector2 realitivePosition = posInGrid - new Vector2(tileParent.position.x,tileParent.position.y);
        return new Vector2Int(Math.Abs((int)(realitivePosition.x - mapHalfHeight.x - .5f)), Math.Abs((int)(realitivePosition.y - mapHalfHeight.y - .5f))); 
    }

    public static Vector2Int WorldToGridSpace(float x, float y)
    {
        return WorldToGridSpace(new Vector2(x, y));
    }

    public static Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);

    public static Tile GetTile(int x, int y) => x > 0 && x < map.sizeX && y > 0 && y < map.sizeY ? tiles[x, y] : null;

    public static void DestroyPhysicalMapTiles()
    {
        if(tileParent != null)
        {
            UnityEngine.Object.DestroyImmediate(tileParent.gameObject);
        }
    }

    public static void SaveMap(string name)
    {
        SaveSystem.SaveMap(name,new Map(tiles,implements.ToArray()));
    }

    public static void LoadMap(string name, SpritePallate spritePallate)
    {
        Map map = SaveSystem.LoadMap(name, true);
        GeneratePhysicalMap(spritePallate, map);
    }
}
