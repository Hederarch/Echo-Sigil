using System;
using UnityEngine;

public class MapReaderBehavior : MonoBehaviour
{

}

public static class MapReader
{
    public static Transform tileParent;
    public static Map map;
    public static Vector2Int backupMapSize;
    public static Tile[,] tiles;

    public static Tile[,] GeneratePhysicalMap(Map map)
    {
        DestroyPhysicalMapTiles();
        if (map == null)
        {
            map = new Map(backupMapSize.x, backupMapSize.y);
        }
        tileParent = new GameObject("Tile Parent").transform;
        tiles = new Tile[map.sizeX,map.sizeY];
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 2.5f, map.sizeY / 2.5f);
        for (int x = 0; x < map.sizeX; x++)
        {
            for (int y = 0; y < map.sizeY; y++)
            {
                Tile tile = map.SetTileProperties(x, y);

                GameObject gameObjectTile = new GameObject(tile.PosInGrid.x + "," + tile.PosInGrid.y + " tile");
                gameObjectTile.transform.position = new Vector3(mapHalfHeight.x - x, mapHalfHeight.y - y);
                gameObjectTile.transform.rotation = Quaternion.identity;
                gameObjectTile.transform.parent = tileParent;

                gameObjectTile.AddComponent<TileBehaviour>().tile = tile;
                tiles[x, y] = tile;

                gameObjectTile.AddComponent<BoxCollider>().size = new Vector3(1, 1, .2f);
            }
        }
        return tiles;
    }

    internal static Vector2 GetTilesPhyisicalLocation(Vector2Int posInGrid)
    {
        Vector2 mapHalfHeight = new Vector2(map.sizeX / 1.5f, map.sizeY / 1.5f);
        Vector2 realitivePosition = new Vector2(mapHalfHeight.x - posInGrid.x, mapHalfHeight.y - posInGrid.y);
        return new Vector2(tileParent.transform.position.x, tileParent.transform.position.y) + realitivePosition;
    }

    public static Tile GetTile(Vector2Int pos)
    {
        return GetTile(pos.x, pos.y);
    }

    public static Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public static void DestroyPhysicalMapTiles()
    {
        if(tileParent != null)
        {
            UnityEngine.Object.DestroyImmediate(tileParent.gameObject);
        }
    }

    public static void SaveMap(string name)
    {
        
    }

    public static void LoadMap(string name)
    {

    }
}
