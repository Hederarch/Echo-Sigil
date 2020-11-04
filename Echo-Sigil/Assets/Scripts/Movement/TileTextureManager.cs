using System;
using UnityEngine;

public static class TileTextureManager
{
    public const float fogDist = .5f;
    public static Color GetFogColor(Color originalColor, float distToZ0)
    {
        if (distToZ0 < fogDist)
        {
            return Color.Lerp(originalColor, Color.black, Mathf.InverseLerp(fogDist, 0, distToZ0));
        }
        return originalColor;
    }

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
        Texture2D texture = GetTileTexture(MapReader.spritePallate[spriteIndex], tileTextureSection, false, tile);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f, texture.width);
    }

    public static Texture2D GetTileTexture(Texture2D texture, TileTextureSection tileTextureSection, bool debug = false, Tile tile = null)
    {
        Texture2D sectionTexture = GetTileTextureSection(debug ? GetDebugTexture(texture.width, texture.height) : texture, tileTextureSection);

        if (tile == null)
        {
            return sectionTexture;
        }

        switch (tileTextureSection)
        {
            case TileTextureSection.Top:
                Texture2D topTexture = new Texture2D(texture.width, texture.height);
                topTexture.SetPixels(texture.GetPixels());
                topTexture.Apply();
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
                topTexture = GetTileTextureSection(topTexture, TileTextureSection.Top);
                Color[] colors = topTexture.GetPixels();
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = GetFogColor(colors[i], tile.topHeight);
                }
                topTexture.SetPixels(colors);
                topTexture.Apply();
                return topTexture;
            case TileTextureSection.Side:
                return GetTileSide(texture, Mathf.RoundToInt(Mathf.Min(tile.topHeight, tile.sideLength) * texture.width), tile.bottomHeight);

        }

        return sectionTexture;
    }

    private static Texture2D ApplyBorder(Texture2D texture, Tile tile, Vector2Int direction)
    {
        if ((direction.x == 0 || direction.y == 0) && (direction.x != direction.y))
        {
            if (direction.x == 0)
            {
            }
            Tile neighbor = tile.FindNeighbor(direction);
            if (neighbor == null || neighbor != null && neighbor.topHeight != tile.topHeight)
            {
                Texture2D edge = GetTileTextureSection(texture, TileTextureSection.Edge);
                Texture2D border = GetTileTextureSection(texture, TileTextureSection.Border);

                Tile leftNeighbor = tile.FindNeighbor(new Vector2Int(direction.y, -direction.x));
                Tile rightNeighbor = tile.FindNeighbor(new Vector2Int(-direction.y, direction.x));
                bool leftEdge = leftNeighbor == null || leftNeighbor != null && leftNeighbor.topHeight != tile.topHeight;
                bool rightEdge = rightNeighbor == null || rightNeighbor != null && rightNeighbor.topHeight != tile.topHeight;

                int width = texture.width;
                int div10 = texture.width / 10;
                int div2 = texture.width / 2;

                Color[] completeEdgeColors = new Color[texture.width * div10];
                Color[] rightArray = rightEdge ? edge.GetPixels() : border.GetPixels();
                Color[] leftArray = leftEdge ? edge.GetPixels() : border.GetPixels();
                for (int i = 0; i < div10; i++)
                {
                    Array.Copy(leftArray, div2 * i, completeEdgeColors, texture.width * i, div2);
                    for (int g = 0; g < div2; g++)
                    {
                        int index = Mathf.Clamp((div2 * i) + div2 - g + 1, 0, rightArray.Length - 1);
                        int output = Mathf.Clamp((texture.width * i) + div2 + g, 0, completeEdgeColors.Length - 1);
                        completeEdgeColors[output] = rightArray[index];
                    }
                }

                Color[] outputColors = texture.GetPixels();
                int y = 0;
                for (int i = 0; i < div10 * width; i++)
                {
                    int index;
                    if (direction.y == 1)
                    {
                        index = i;
                    }
                    else if (direction.y == -1)
                    {
                        index = width * width - i;
                    }
                    else if (direction.x == 1)
                    {
                        index = (i / width) + (y * width);
                        y++;
                        if (y >= 64)
                        {
                            y = 0;
                        }
                    }
                    else
                    {
                        index = width - (i / width) + (y * width);
                        y++;
                        if (y >= 64)
                        {
                            y = 0;
                        }
                    }
                    index = Mathf.Clamp(index, 0, width * width);
                    Color color = completeEdgeColors[i];
                    outputColors[index] = color != Color.clear ? color : outputColors[index];
                }

                Texture2D outputTexture = texture;
                outputTexture.name = "Bordered Texture";
                outputTexture.SetPixels(outputColors);
                outputTexture.Apply();

                return outputTexture;
            }
        }
        return texture;

    }

    public static Sprite GetTileSide(int pallateIndex, float heightInWorldUnits, float distToZ0InWorldUnits)
    {
        Texture2D texture = GetTileSide(MapReader.spritePallate[pallateIndex], heightInWorldUnits, distToZ0InWorldUnits);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        Sprite sprite = texture.height > 0 ? Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f, texture.width) : null;
        if (sprite != null)
        {
            sprite.name = texture.name;
        }
        return sprite;
    }
    private static Texture2D GetTileSide(Texture2D texture, float heightInWorldUnits, float distToZ0InWorldUnits)
    {
        return GetTileSide(texture, (int)(heightInWorldUnits * texture.width), distToZ0InWorldUnits);
    }
    private static Texture2D GetTileSide(Texture2D texture, int heightInPixels, float distToZ0)
    {
        int width = texture.width;
        int div10 = width / 10;

        Texture2D sideTexture = new Texture2D(width, heightInPixels);
        if (heightInPixels > 0)
        {
            Color[] sideColors = new Color[width * heightInPixels];

            for (int i = 0; i < sideColors.Length; i++)
            {
                sideColors[i] = Color.white;
            }


            Array.Copy(texture.GetPixels(), (width + div10) * width, sideColors, 0, width * Mathf.Min(heightInPixels, width));

            if (heightInPixels > width)
            {
                for (int y = 0; y < heightInPixels - width; y++)
                {
                    int sideAndTop = (2 * width + div10);
                    int sourceIndex = Mathf.Clamp(sideAndTop + Mathf.RoundToInt(Mathf.Lerp(0, texture.height - sideAndTop, Mathf.InverseLerp(0, heightInPixels - width, y))), 0, texture.height - 1) * width;
                    Array.Copy(texture.GetPixels(), sourceIndex, sideColors, width * width + y * width, width);
                }
            }

            if (distToZ0 < fogDist)
            {
                int fogPixels = Mathf.RoundToInt(Mathf.Min(fogDist * width, sideColors.Length / width));
                for (int y = 0; y < fogPixels; y++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        int index = (sideColors.Length - 1) - ((y * width) + i);
                        sideColors[index] = GetFogColor(sideColors[index], (float)y / (float)width);
                    }
                }
            }

            sideTexture.SetPixels(sideColors);
            sideTexture.Apply();
        }
        return sideTexture;
    }

    private static Texture2D GetTileTextureSection(Texture2D texture, TileTextureSection tileTextureSection, bool debug = false, bool deface = false)
    {
        int width = texture.width;
        int div10 = width / 10;
        switch (tileTextureSection)
        {
            case TileTextureSection.Top:
                Texture2D topTexture = GetTextureSection(texture,
                                         debug ? Color.blue : Color.black,
                                         0,
                                         0,
                                         width,
                                         width,
                                         deface);
                topTexture.name = "Top Texture";
                return topTexture;
            case TileTextureSection.Border:
                Texture2D borderTexture = GetTextureSection(texture,
                                         debug ? Color.green : Color.black,
                                         0,
                                         width,
                                         width / 2,
                                         width + div10,
                                         deface);
                borderTexture.name = "Border Texture";
                return borderTexture;
            case TileTextureSection.Edge:
                Texture2D edgeTexture = GetTextureSection(texture,
                                         debug ? Color.black + (Color.green / 2) : Color.black,
                                         width / 2,
                                         width,
                                         width,
                                         width + div10,
                                         deface);
                edgeTexture.name = "Edge Texture";
                /*if (edgeTexture.width > 0 && edgeTexture.height > 0)
                {
                    Color[] edgeColors = edgeTexture.GetPixels();
                    for (int x = 0; x < div10; x++)
                    {
                        for (int y = 0; y < div10; y++)
                        {
                            int index = x + (y * edgeTexture.width);
                            if (deface)
                            {
                                index += width * width;
                            }
                            edgeColors[index] = x <= y ? Color.clear : edgeColors[index];
                        }
                    }
                    edgeTexture.SetPixels(edgeColors);
                    edgeTexture.Apply();
                }*/
                return edgeTexture;
            case TileTextureSection.Side:
                Texture2D sideTexture = GetTextureSection(texture,
                                         debug ? Color.cyan : Color.black,
                                         0,
                                         width + div10,
                                         width,
                                         Mathf.Min((2 * width) + div10, texture.height - 4),
                                         deface);
                sideTexture.name = "Side Texture";
                return sideTexture;
            case TileTextureSection.Extents:
                Texture2D extentsTexture = GetTextureSection(texture,
                                         debug ? Color.red : Color.black,
                                         0,
                                         Mathf.Min((2 * width) + div10, texture.height - div10),
                                         width,
                                         texture.height,
                                         deface);
                extentsTexture.name = "Extents Texture";
                return extentsTexture;
            case TileTextureSection.Original:
                return texture;
            default:
                Debug.LogWarning("No TileTextureSection selection, returning original");
                return texture;
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
        if (deltaX > 0 && deltaY > 0)
        {
            outputTexture.SetPixels(outputColors);
            outputTexture.Apply();
        }

        return outputTexture;
    }

}

public enum TileTextureSection { Original, Top, Border, Edge, Side, Extents }
