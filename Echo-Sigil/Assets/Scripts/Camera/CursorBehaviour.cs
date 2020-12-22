using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class CursorBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SaveSystem.Tile.CursorSprite;
    }

    private void Update()
    {
        Cursor.GetCursor();
    }
}

public static class Cursor
{
    private static CursorBehaviour behaviour;
    public static bool locked = true;
    public static Unit unit;
    public static TileMap.TileBehaviour tileBehaviour;
    public static TileMap.Tile Tile => tileBehaviour.tile;

    public static void GetCursor()
    {
        if (behaviour == null)
        {
            behaviour = new GameObject("Cursor", typeof(CursorBehaviour)).GetComponent<CursorBehaviour>();
        }
        behaviour.transform.position = GetCursor(locked);
    }

    public static Vector3 GetCursor(bool centerMouse)
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
                return tile.PosInWorld + new Vector3(0, 0, .02f);
            }
        }

        Vector3 pos = TileMap.MapReader.AlignWorldPosToGrid(WorldPointToZ0(from, GamplayCamera.instance.transform.forward) + Vector3.forward * .02f);
        tile = TileMap.MapReader.GetTile(TileMap.MapReader.WorldToGridSpace(pos));
        if (tile != null)
        {
            pos = tile.PosInWorld + Vector3.forward * .02f;
            tileBehaviour = tile.TileBehaviour;
        }
        return pos;
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
        EditorGUILayout.BeginHorizontal();
        Cursor.locked = EditorGUILayout.Toggle("Locked", Cursor.locked);
        Cursor.GetCursor(out Unit unit, out TileMap.TileBehaviour tileBehaviour, Cursor.locked);
        if (tileBehaviour != null)
        {
            EditorGUILayout.LabelField((unit != null ? unit.ToString() : "nobody") + " at " + tileBehaviour.tile.posInGrid);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
