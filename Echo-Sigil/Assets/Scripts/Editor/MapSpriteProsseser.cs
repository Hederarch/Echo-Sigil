using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapSpriteProsseser : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        string lowerCaseAssetPath = assetPath.ToLower();
        bool isInSpritesDirectory = lowerCaseAssetPath.IndexOf("/maps/") != -1;

        if (isInSpritesDirectory)
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.isReadable = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
