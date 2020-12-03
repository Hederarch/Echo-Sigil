using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace TileMap
{
    [RequireComponent(typeof(RawImage))]
    public class TileTextureManager : MonoBehaviour
    {
        public bool border = true;
        [HideInInspector]
        public bool leftEdge = false;
        [HideInInspector]
        public bool rightEdge = false;
        public void OnValidate()
        {
            RawImage rawImage = GetComponent<RawImage>();
            Texture2D texture = GetDebugTexture();
            if (border)
            {
                Texture2D texture2d = new Texture2D(texture.width, texture.width / 10);
                texture2d.SetPixels(GetCompleteEdgeColors(texture, leftEdge, rightEdge));
                texture2d.Apply();
                texture2d.name = "Border Texture";
                texture2d.filterMode = FilterMode.Point;
                texture2d.wrapMode = TextureWrapMode.Clamp;
                texture = texture2d;
            }
            rawImage.texture = texture;
            rawImage.SetNativeSize();
        }

        public static float fogDist = .5f;
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
            return new Texture2D[1] { GetDebugTexture() };
        }

        private static Texture2D GetDebugTexture()
        {
            Texture2D texture2D = SaveSystem.Tile.GetDefaultTiileTexture();

            if (texture2D == null)
            {
                texture2D = new Texture2D(64, 64 * 3);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Top, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Border, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Edge, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Side, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Extents, true, true);
            }

            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.filterMode = FilterMode.Point;
            texture2D.name = "Debug Tile Texture";
            return texture2D;
        }

        public static Sprite GetTileSprite(int spriteIndex, TileTextureSection tileTextureSection, Tile tile = null)
        {
            Texture2D texture = GetTileTexture(MapReader.spritePallate[spriteIndex], tileTextureSection, false, tile);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f, texture.width);
        }

        public static Texture2D GetTileTexture(Texture2D texture, TileTextureSection tileTextureSection, bool debug = false, Tile tile = null)
        {
            Texture2D sectionTexture = GetTileTextureSection(debug ? GetDebugTexture() : texture, tileTextureSection);

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

                    Tile leftNeighbor = tile.FindNeighbor(new Vector2Int(direction.y, -direction.x));
                    Tile rightNeighbor = tile.FindNeighbor(new Vector2Int(-direction.y, direction.x));
                    bool leftEdge = leftNeighbor == null || leftNeighbor != null && leftNeighbor.topHeight != tile.topHeight;
                    bool rightEdge = rightNeighbor == null || rightNeighbor != null && rightNeighbor.topHeight != tile.topHeight;

                    int width = texture.width;
                    int div10 = texture.width / 10;
                    int div2 = texture.width / 2;

                    Color[] completeEdgeColors = GetCompleteEdgeColors(texture, leftEdge, rightEdge);

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
                            index = width * width - i - 1;
                        }
                        else if (direction.x == 1)
                        {
                            index = (i / width) + ((width - y - 1) * width);
                            y++;
                            if (y >= width)
                            {
                                y = 0;
                            }
                        }
                        else
                        {
                            index = width - 1 - (i / width) + (y * width);
                            y++;
                            if (y >= 64)
                            {
                                y = 0;
                            }
                        }
                        index = Mathf.Clamp(index, 0, outputColors.Length - 1);
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

        private static Color[] GetCompleteEdgeColors(Texture2D texture, bool leftEdge, bool rightEdge)
        {
            Texture2D edge = GetTileTextureSection(texture, TileTextureSection.Edge);
            Texture2D border = GetTileTextureSection(texture, TileTextureSection.Border);

            int width = texture.width;
            int div10 = width / 10;
            int div2 = width / 2;

            Color[] completeEdgeColors = new Color[width * div10];
            Color[] rightArray = rightEdge ? edge.GetPixels() : border.GetPixels();
            Color[] leftArray = leftEdge ? edge.GetPixels() : border.GetPixels();
            for (int y = 0; y < div10; y++)
            {
                Array.Copy(leftArray, div2 * y, completeEdgeColors, width * y, div2);
                for (int x = 0; x < div2; x++)
                {
                    int sourceIndex = (div2 * y) + div2 - x - 1;
                    int destinationIndex = (width * y) + div2 + x;
                    sourceIndex = Mathf.Clamp(sourceIndex, 0, rightArray.Length - 1);
                    destinationIndex = Mathf.Clamp(destinationIndex, 0, completeEdgeColors.Length - 1);
                    completeEdgeColors[destinationIndex] = rightArray[sourceIndex];
                }
            }

            return completeEdgeColors;
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

        private static Texture2D GetTileTextureSection(Texture2D texture, TileTextureSection tileTextureSection, bool deface = false, bool debug = false)
        {
            if (debug)
            {
                Texture2D texture2D = SaveSystem.Tile.GetDefaultTiileTexture();
                if (texture2D != null)
                {
                    texture = texture2D;
                    debug = false;
                }
            }
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
                    WedgeEdge(edgeTexture, deface, width, div10);
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

        private static void WedgeEdge(Texture2D edgeTexture, bool deface = false, int width = 0, int div10 = 0)
        {
            if (edgeTexture.width > 0 && edgeTexture.height > 0)
            {
                if (!deface)
                {
                    width = edgeTexture.width;
                    div10 = edgeTexture.height;
                }
                Color[] edgeColors = edgeTexture.GetPixels();
                for (int x = 0; x < div10; x++)
                {
                    for (int y = 0; y < div10; y++)
                    {
                        int index = x + (y * edgeTexture.width);
                        if (deface)
                        {
                            index += (width * width) + (width / 2);
                            if (index >= edgeColors.Length)
                            {
                                continue;
                            }
                        }
                        edgeColors[index] = x < y ? Color.clear : edgeColors[index];
                    }
                }
                edgeTexture.SetPixels(edgeColors);
                edgeTexture.Apply();
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
                            Color color = Color.Lerp(debugColor, Color.black, Mathf.InverseLerp(0, deltaY - 1, y));
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

#if UNITY_EDITOR
    [CustomEditor(typeof(TileTextureManager))]
    public class TileTextureBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TileTextureManager tileTextureManager = (TileTextureManager)target;
            if (tileTextureManager.border)
            {
                EditorGUI.BeginChangeCheck();
                tileTextureManager.leftEdge = EditorGUILayout.Toggle("Left Edge", tileTextureManager.leftEdge);
                tileTextureManager.rightEdge = EditorGUILayout.Toggle("Right Edge", tileTextureManager.rightEdge);
                if (EditorGUI.EndChangeCheck())
                {
                    tileTextureManager.OnValidate();
                }

            }
            TileTextureManager.fogDist = EditorGUILayout.FloatField("Fog Distance ", TileTextureManager.fogDist);
            if (GUILayout.Button("Export Template"))
            {
                RawImage rawImage = tileTextureManager.GetComponent<RawImage>();
                Texture2D texture = (Texture2D)rawImage.texture;
                SaveSystem.PNG.SavePNG(EditorUtility.SaveFilePanel("Save as...", Application.persistentDataPath, "TileTextureTemplate", "png"), texture);
            }
        }
    }
#endif 
}