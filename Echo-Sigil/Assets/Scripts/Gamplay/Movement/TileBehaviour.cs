using System;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Tile tile;
    private void Update()
    {
        GetComponent<SpriteRenderer>().color = tile.CheckColor();
    }

    public static implicit operator Tile(TileBehaviour t) => t.tile;

}

public class Tile
{
    public Vector2Int PosInGrid;
    public Vector2 PosInWorld { get => MapReader.GridToWorldSpace(PosInGrid); }
    public float topHeight;
    public float bottomHeight;
    public float midHeight => (topHeight + bottomHeight) / 2f;
    public float sideLength
    {
        get => topHeight - bottomHeight;
        set
        {
            float mid = midHeight;
            topHeight = mid + (value / 2f);
            bottomHeight = mid - (value / 2f);
        }
    }

    public int spriteIndex;

    public bool walkable = true;
    public bool current;
    public bool target;
    public bool selectable;

    //BFS stuff
    public bool visited;
    public Tile parent;
    public int distance;

    //A* stuff
    public float f => g + h;
    public float g;
    public float h;

    public Tile(int x, int y) : this(new Vector2Int(x, y)) { }

    internal static Sprite[] GetDebugPallate()
    {
        throw new NotImplementedException();
    }

    public Tile(Vector2Int posInGrid)
    {
        PosInGrid = posInGrid;
    }

    public Color CheckColor()
    {
        Color output = Color.white;
        if (selectable)
        {
            output = Color.green;
        }
        if (target)
        {
            output = Color.red;
        }
        if (current)
        {
            output = Color.blue;
        }
        if (!walkable)
        {
            output -= Color.white / 2f;
            output.a = 1;
        }
        return output;
    }

    /// <summary>
    /// Reset tile and then add neighbors to adjaceny list
    /// </summary>
    /// <param name="jumpHeight">Distance up and down before tiles stop being neighbors</param>
    /// <param name="target">Tile discounted for direction check.</param>
    public Tile[] FindNeighbors(float jumpHeight, Tile target = null)
    {
        return new Tile[4] {
        FindNeighbor(Vector2Int.up, jumpHeight, target),
        FindNeighbor(Vector2Int.down, jumpHeight, target),
        FindNeighbor(Vector2Int.left, jumpHeight, target),
        FindNeighbor(Vector2Int.right, jumpHeight, target)
        };
    }

    public Tile FindNeighbor(Vector2Int direction, float jumpHeight, Tile target)
    {
        throw new NotImplementedException();
    }

    public bool DirectionCheck()
    {
        bool output = true;
        //TODO
        /*if(Physics.Raycast(transform.position,Vector3.back,out RaycastHit hit))
        {
            output = false;
        }*/
        return output;
    }

    public void ResetTile()
    {
        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;

        g = 0;
        h = 0;
    }

    public static Sprite GetTileSprite(int spriteIndex, TileTextureType tileTextureType, Tile tile = null)
    {
        Texture2D texture = GetTileTexture(MapReader.spritePallate[spriteIndex].texture, tileTextureType, tile);
        return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f, texture.width);
    }

    public static Texture2D GetTileTexture(Texture2D texture, TileTextureType tileTextureType, Tile tile = null)
    {
        if (tile == null)
        {
            bool inContext = false;
            switch (tileTextureType)
            {
                case TileTextureType.DebugContext:
                    tileTextureType = TileTextureType.Debug;
                    inContext = true;
                    break;
                case TileTextureType.ExtentsContext:
                    tileTextureType = TileTextureType.Extents;
                    inContext = true;
                    break;
                case TileTextureType.SideExtenetsContext:
                    tileTextureType = TileTextureType.SideExtenets;
                    inContext = true;
                    break;
                case TileTextureType.TopBorederedContext:
                    tileTextureType = TileTextureType.TopBordered;
                    inContext = true;
                    break;
                case TileTextureType.TopEdgedContext:
                    tileTextureType = TileTextureType.TopEdged;
                    inContext = true;
                    break;
                case TileTextureType.DebugTopContext:
                    tileTextureType = TileTextureType.DebugTop;
                    inContext = true;
                    break;
                case TileTextureType.DebugSideContext:
                    tileTextureType = TileTextureType.DebugSide;
                    inContext = true;
                    break;
            }
            if (inContext)
            {
                Debug.LogWarning("No context given. Switching to non-contextual");
            }
        }
        switch (tileTextureType)
        {
            case TileTextureType.Original:
                return texture;
            case TileTextureType.Top:
                return GetTileTextureSection(texture, TileTextureSection.Top);
            case TileTextureType.Border:
                return GetTileTextureSection(texture, TileTextureSection.Border);
            case TileTextureType.Edge:
                return GetTileTextureSection(texture, TileTextureSection.Edge);
            case TileTextureType.Side:
                return GetTileTextureSection(texture, TileTextureSection.Side);
            case TileTextureType.Extents:
                return GetTileTextureSection(texture, TileTextureSection.Extents);
            case TileTextureType.TopBordered:
                throw new NotImplementedException();
            case TileTextureType.TopEdged:
                throw new NotImplementedException();
            case TileTextureType.SideExtenets:
                throw new NotImplementedException();
            case TileTextureType.Debug:
                Texture2D texture2D = new Texture2D(texture.width, texture.height);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Top, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Border, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Edge, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Side, true, true);
                texture2D = GetTileTextureSection(texture2D, TileTextureSection.Extents, true, true);
                return texture2D;
            case TileTextureType.DebugTop:
                return GetTileTextureSection(texture, TileTextureSection.Top, true);
            case TileTextureType.DebugSide:
                Texture2D debugSideTexture2D = GetTileTexture(texture, TileTextureType.Debug);
                int index = texture.width + texture.width / 10;
                Color[] debugSideColors = debugSideTexture2D.GetPixels(0, index, debugSideTexture2D.width, debugSideTexture2D.height - index);
                debugSideTexture2D = new Texture2D(debugSideTexture2D.width, debugSideTexture2D.height - index);
                debugSideTexture2D.SetPixels(debugSideColors);
                debugSideTexture2D.Apply();
                return debugSideTexture2D;
            case TileTextureType.DebugContext:
                throw new NotImplementedException();
            case TileTextureType.DebugTopContext:
                throw new NotImplementedException();
            case TileTextureType.DebugSideContext:
                throw new NotImplementedException();
            case TileTextureType.ExtentsContext:
                throw new NotImplementedException();
            case TileTextureType.TopEdgedContext:
                throw new NotImplementedException();
            case TileTextureType.SideExtenetsContext:
                throw new NotImplementedException();
            case TileTextureType.TopBorederedContext:
                throw new NotImplementedException();
            default:
                Debug.LogWarning("No TileTextureType selected. Returning top");
                return GetTileTextureSection(texture, TileTextureSection.Top);
        }
    }

    public static Texture2D GetTileTextureSection(Texture2D texture, TileTextureSection tileTextureSection, bool debug = false, bool deface = false)
    {
        int width = texture.width;
        switch (tileTextureSection)
        {
            case TileTextureSection.Top:
                return GetTextureSection(texture, debug ? Color.blue : Color.black, 0, 0, width, width, deface);
            case TileTextureSection.Border:
                return GetTextureSection(texture, debug ? Color.green : Color.black, 0, width, width / 2, width + width / 10, deface);
            case TileTextureSection.Edge:
                return GetTextureSection(texture, debug ? Color.yellow : Color.black, width / 2, width, width, width + width / 10, deface);
            case TileTextureSection.Side:
                return GetTextureSection(texture, debug ? Color.cyan : Color.black, 0, width + width / 10, width, Mathf.Min(2 * width + width / 10, texture.height - 4), deface);
            case TileTextureSection.Extents:
                return GetTextureSection(texture, debug ? Color.red : Color.black, 0, Mathf.Min(2 * width + width / 10, texture.height - width / 10), width, texture.height, deface);
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

public enum TileTextureType { Top, Border, TopBordered, TopBorederedContext, Edge, TopEdged, TopEdgedContext, Side, Extents, ExtentsContext, SideExtenets, SideExtenetsContext, Original, Debug, DebugContext, DebugTop, DebugTopContext, DebugSide, DebugSideContext }
public enum TileTextureSection { Top, Border, Edge, Side, Extents }