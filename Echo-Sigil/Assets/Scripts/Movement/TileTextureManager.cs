using System;
using UnityEngine;

public static class TileTextureManager
{
    public static Texture2D[] GetDebugPallate()
    {
        return new Texture2D[1] { GetDebugTexture(64, 64 * 3) };
    }

    private static Texture2D GetDebugTexture(int width, int height)
    {
        Texture2D texture2D = new Texture2D(width, height);
        texture2D = GetTileTextureSection(texture2D, TileTextureSection.Top, true, true);
        texture2D = GetTileTextureSection(texture2D, TileTextureSection.Border, true, true);
        texture2D = GetTileTextureSection(texture2D, TileTextureSection.Edge, true, true);
        texture2D = GetTileTextureSection(texture2D, TileTextureSection.Side, true, true);
        texture2D = GetTileTextureSection(texture2D, TileTextureSection.Extents, true, true);
        return texture2D;
    }

    public static Sprite GetTileSprite(int spriteIndex, TileTextureSection tileTextureSection, Vector2Int direction, Tile tile = null)
    {
        Texture2D texture = GetTileTexture(MapReader.spritePallate[spriteIndex], tileTextureSection, direction, false, tile);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f, texture.width);
    }

    public static Texture2D GetTileTexture(Texture2D texture, TileTextureSection tileTextureSection, Vector2Int direction, bool debug = false, Tile tile = null)
    {
        Texture2D sectionTexture = GetTileTextureSection(debug ? GetDebugTexture(texture.width, texture.height) : texture, tileTextureSection);

        if (tile == null)
        {
            return sectionTexture;
        }

        switch (tileTextureSection)
        {
            case TileTextureSection.Top:
                Texture2D topTexture = texture;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (Mathf.Abs(x) != Mathf.Abs(y))
                        {
                            Vector2Int checkDirection = new Vector2Int(x, y);
                            topTexture = ApplyBorder(topTexture, tile, checkDirection);
                        }
                    }
                }
                return GetTileTextureSection(topTexture, TileTextureSection.Top);
            case TileTextureSection.Side:
                if (direction == Vector2Int.zero)
                {
                    Debug.LogWarning("Default direction passed to GetTileTexture.Side");
                    return sectionTexture;
                }
                Tile neighbor = tile.FindNeighbor(direction);
                int height = Mathf.RoundToInt((neighbor == null ? Mathf.Min(tile.topHeight, tile.sideLength) : Mathf.Min(tile.topHeight - neighbor.topHeight, tile.sideLength)) * texture.width);
                Texture2D sideTexture = new Texture2D(texture.width, height);
                Color[] sideColors = new Color[texture.width * height];

                for (int i = 0; i < sideColors.Length; i++)
                {
                    sideColors[i] = Color.white;
                }

                Array.Copy(texture.GetPixels(), (texture.width + (texture.width / 10)) * texture.width, sideColors, 0, Mathf.Min(texture.width * height, texture.width * texture.width));

                if (height > texture.width)
                {
                    for (int y = 0; y < height - texture.width; y++)
                    {
                        int sideAndTop = (2 * texture.width + (texture.width / 10));
                        int sourceIndex = Mathf.Clamp(sideAndTop + Mathf.RoundToInt(Mathf.Lerp(0, texture.height - sideAndTop, Mathf.InverseLerp(0, height - texture.width, y))), 0, texture.height - 1) * texture.width;
                        Array.Copy(texture.GetPixels(), sourceIndex, sideColors, texture.width * texture.width + y * texture.width, texture.width);
                    }
                }

                sideTexture.SetPixels(sideColors);
                sideTexture.Apply();
                return sideTexture;
        }

        return sectionTexture;
    }

    private static Texture2D ApplyBorder(Texture2D texture, Tile tile, Vector2Int direction)
    {
        Tile neighbor = tile.FindNeighbor(direction);
        if (neighbor == null || neighbor != null && neighbor.topHeight != tile.topHeight)
        {
            Texture2D edge = GetTileTextureSection(texture, TileTextureSection.Edge);
            Texture2D border = GetTileTextureSection(texture, TileTextureSection.Border);

            Tile leftNeighbor = tile.FindNeighbor(new Vector2Int(direction.y, -direction.x));
            Tile rightNeighbor = tile.FindNeighbor(new Vector2Int(-direction.y, direction.x));
            bool leftEdge = leftNeighbor == null || leftNeighbor != null && leftNeighbor.topHeight != tile.topHeight;
            bool rightEdge = rightNeighbor == null || rightNeighbor != null && rightNeighbor.topHeight != tile.topHeight;

            int div10 = texture.width/10;

            return texture;
        }
        else
        {
            return texture;
        }
    }

    private static Texture2D GetTileTextureSection(Texture2D texture, TileTextureSection tileTextureSection, bool debug = false, bool deface = false)
    {
        int width = texture.width;
        switch (tileTextureSection)
        {
            case TileTextureSection.Top:
                return GetTextureSection(texture, debug ? Color.blue : Color.black, 0, 0, width, width, deface);
            case TileTextureSection.Border:
                return GetTextureSection(texture, debug ? Color.green : Color.black, 0, width, width / 2, width + width / 10, deface);
            case TileTextureSection.Edge:
                return GetTextureSection(texture, debug ? Color.black + (Color.green / 2) : Color.black, width / 2, width, width, width + width / 10, deface);
            case TileTextureSection.Side:
                return GetTextureSection(texture, debug ? Color.cyan : Color.black, 0, width + width / 10, width, Mathf.Min(2 * width + width / 10, texture.height - 4), deface);
            case TileTextureSection.Extents:
                return GetTextureSection(texture, debug ? Color.red : Color.black, 0, Mathf.Min(2 * width + width / 10, texture.height - width / 10), width, texture.height, deface);
            case TileTextureSection.Original:
                return texture;
            default:
                Debug.LogWarning("No TileTextureSection selection, returning top");
                return GetTextureSection(texture, debug ? Color.blue : Color.black, 0, 0, width, width, deface);
        }

    }

    private static Texture2D GetTextureSection(Texture2D texture, Color debugColor, int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, bool deface = false)
    {
        int width = texture.width;
        bool debug = debugColor != Color.black;
        if (!debug || deface)
        {
            topLeftX = Mathf.Clamp(topLeftX, 0, texture.width);
            bottomRightX = Mathf.Clamp(bottomRightX, 0, texture.width);

            topLeftY = Mathf.Clamp(topLeftY, 0, texture.height);
            bottomRightY = Mathf.Clamp(bottomRightY, 0, texture.height);
        }

        int deltaX = bottomRightX - topLeftX;
        int deltaY = bottomRightY - topLeftY;

        Color[] outputColors = deface ? texture.GetPixels() : new Color[deltaX * deltaY];
        if (debug)
        {
            if (deface)
            {
                for (int y = 0; y < deltaY; y++)
                {
                    for (int x = 0; x < deltaX; x++)
                    {
                        int index = topLeftY * width + (y * width) + topLeftX + x;
                        Color color = Color.Lerp(debugColor, Color.black, Mathf.InverseLerp(0, deltaY, y));
                        outputColors[index] = debugColor;
                    }
                }
            }
            else
            {
                for (int i = 0; i < deltaX * deltaY; i++)
                {
                    outputColors[i] = debugColor;
                }
            }
        }
        else
        {
            outputColors = texture.GetPixels(topLeftX, topLeftY, deltaX, deltaY);
        }

        Texture2D outputTexture = deface ? texture : new Texture2D(deltaX, deltaY);
        outputTexture.SetPixels(outputColors);
        outputTexture.Apply();

        return outputTexture;
    }

}

public enum TileTextureSection { Original, Top, Border, Edge, Side, Extents }
