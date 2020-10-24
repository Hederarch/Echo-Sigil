using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileTextureTester : MonoBehaviour
{
    public TileTextureType tileTextureType;
    public RawImage rawImage;
    public Vector2Int textureSize;

    protected void OnValidate()
    {
        Texture2D texture2D = TileTextureManager.GetTileTexture(new Texture2D(textureSize.x, textureSize.y), tileTextureType);
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        rawImage.texture = texture2D;
        rawImage.SetNativeSize();
    }
}
