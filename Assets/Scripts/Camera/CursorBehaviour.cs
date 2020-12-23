using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class CursorBehaviour : MonoBehaviour
{
    private static CursorBehaviour behaviour;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SaveSystem.Tile.CursorSprite;
    }

    private void Update()
    {
        GetCursor();
        Vector3 desieredFoucus = GamplayCamera.instance.desieredFoucus = Cursor.posInWorld;
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");
        float yAdd = (Cursor.posInGrid.y + y);
        float xAdd = (Cursor.posInGrid.x + x);
        desieredFoucus.y = desieredFoucus.y + (0 <= yAdd && yAdd < TileMap.MapReader.sizeY ? y : 0);
        desieredFoucus.x = desieredFoucus.x + (0 <= xAdd && xAdd < TileMap.MapReader.sizeX ? x : 0);
        GamplayCamera.instance.desieredFoucus = desieredFoucus;
    }

    public static void GetCursor()
    {
        if (behaviour == null)
        {
            behaviour = new GameObject("Cursor", typeof(CursorBehaviour)).GetComponent<CursorBehaviour>();
        }
        Cursor.GetCursor(Cursor.locked);
        behaviour.transform.position = Cursor.posInWorld;
    }
}

public static class Cursor
{
    public static bool locked = true;
    public static Unit unit;
    public static TileMap.TileBehaviour tileBehaviour;
    public static TileMap.Tile Tile => tileBehaviour.tile;
    public static TileMap.TilePos posInGrid;
    public static Vector3 posInWorld => posInGrid + (Vector3.forward * .02f);
    public static Action GotCursorEvent;

    public static void GetCursor(bool centerMouse)
    {
        TileMap.Tile tile = null;
        unit = null;
        tileBehaviour = null;
        Vector3 from = GamplayCamera.instance.cam.ScreenToWorldPoint(centerMouse ? new Vector3(GamplayCamera.instance.cam.pixelWidth * .5f, GamplayCamera.instance.cam.pixelHeight * .5f, 0) : Input.mousePosition);
        if (Physics.Raycast(from, GamplayCamera.instance.transform.forward, out RaycastHit hit))
        {
            if (hit.collider.transform.parent.TryGetComponent(out unit))
            {
                tile = unit.CurTile;
                tileBehaviour = tile.TileBehaviour;
            }
            else if (hit.collider.TryGetComponent(out tileBehaviour))
            {
                tile = tileBehaviour.tile;
                foreach (Collider collider in Physics.OverlapBox(tile.PosInWorld, Vector3.one * .1f))
                {
                    if (collider.transform.parent.TryGetComponent(out unit))
                    {
                        break;
                    }
                }
            }
            if (tile != null)
            {
                posInGrid = tile.posInGrid;
            }
        }

        TileMap.TilePos pos = TileMap.MapReader.WorldToGridSpace(WorldPointToZ0(from, GamplayCamera.instance.transform.forward));
        tile = TileMap.MapReader.GetTile(pos);
        if (tile != null)
        {
            posInGrid = tile.posInGrid;
            tileBehaviour = tile.TileBehaviour;
        }
        else
        {
            posInGrid = pos;
        }
        GotCursorEvent?.Invoke();
    }

    public static Vector3 WorldPointToZ0(Vector3 position, Vector3 forward)
    {
        float degree = Vector3.Angle(forward, Vector3.back) * Mathf.Deg2Rad;
        float distance = position.z / Mathf.Cos(degree);
        return position + forward.normalized * distance;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CursorBehaviour))]
public class CursorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Cursor.locked = EditorGUILayout.Toggle("Locked", Cursor.locked);
        if (Cursor.tileBehaviour != null)
        {
            EditorGUILayout.LabelField((Cursor.unit != null ? Cursor.unit.ToString() : "nobody") + " at " + Cursor.Tile.posInGrid);
        }
        if (GUILayout.Button("GetCursor()"))
        {
            Cursor.GetCursor(Cursor.locked);
        }
    }
}
#endif
