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
        Tile[,] tiles = new Tile[map.size.x,map.size.y];
        Vector2 mapHalfHeight = new Vector2(backupMapSize.x / 1.5f, backupMapSize.y / 1.5f);
        tileParent = UnityEngine.Object.Instantiate(new GameObject("Tile Parent")).transform;
        for (int x = 0; x < map.size.x; x++)
        {
            for (int y = 0; y < map.size.y; y++)
            {
                Tile tile = map.SetTileProperties(x, y);
                GameObject gameObjectTile = UnityEngine.Object.Instantiate(new GameObject(tile.PosInGrid.x + "," + tile.PosInGrid.y + " tile"), new Vector3(mapHalfHeight.x - x, mapHalfHeight.y - y), Quaternion.identity, tileParent);
                tile.PosInWorld = new Vector2(mapHalfHeight.x - x, mapHalfHeight.y - y);
                gameObjectTile.AddComponent<TileBehaviour>().tile = tile;

            }
        }
        MapReader.tiles = tiles;
        return tiles;
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
            Object.DestroyImmediate(tileParent);
        }
    }

    public static void SaveMap(string name)
    {
        
    }

    public static void LoadMap(string name)
    {

    }
}
