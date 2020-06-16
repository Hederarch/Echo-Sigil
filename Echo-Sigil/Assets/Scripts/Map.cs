using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Texture2D mapTexture;
    public Vector2Int size;
    public GameObject tile;

    public float tileHeight = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(mapTexture);
    }

    public void GenerateMap(Texture2D mapTexture)
    {
        RemoveMap();
        if (mapTexture == null)
        {
            mapTexture = new Texture2D(size.x, size.y);

            for (int x = 0; x < mapTexture.width; x++)
            {
                for (int y = 0; y < mapTexture.height; y++)
                {
                    mapTexture.SetPixel(x, y, Color.white);
                }
            }
        }
        else
        {
            Vector2 mapHalfHeight = new Vector2((size.x * tileHeight) / 2 - (tileHeight / 2), (size.y * tileHeight) / 2 - (tileHeight / 2));
            for (int x = 0; x < mapTexture.width; x++)
            {
                for (int y = 0; y < mapTexture.height; y++)
                {
                    if (mapTexture.GetPixel(x, y).a != 0)
                    {
                        GameObject individualTile = Instantiate(tile, new Vector3(mapHalfHeight.x - (x * tileHeight), mapHalfHeight.y - (y * tileHeight)), Quaternion.identity, transform);
                        individualTile.name = x + "," + y + " tile";
                        individualTile.GetComponent<Tile>().walkable = SetProperties(mapTexture.GetPixel(x, y));
                    }
                }
            }
        }
    }
    /// <summary>
    /// Permanently Destroys current map tiles
    /// </summary>
    public void RemoveMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    bool SetProperties(Color color)
    {
        if(color == Color.white)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnValidate()
    {
        if (mapTexture != null)
        {
            size = new Vector2Int(mapTexture.width, mapTexture.height);
        }
    }
}
