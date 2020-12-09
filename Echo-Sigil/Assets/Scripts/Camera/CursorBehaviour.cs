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

    private void OnDrawGizmosSelected()
    {
        if(GamplayCamera.instance != null)
        {
            Gizmos.color = Color.blue;
            Vector3 centered = GamplayCamera.instance.cam.ScreenToWorldPoint(new Vector3(GamplayCamera.instance.cam.pixelWidth * .5f, GamplayCamera.instance.cam.pixelHeight * .5f, 0));
            Vector3 to = Cursor.GetCursor(true);
            Gizmos.DrawLine(centered, to);
            Gizmos.color *= .5f;
            to = Cursor.WorldPointToZ0(centered, GamplayCamera.instance.transform.forward);
            Gizmos.DrawLine(centered, to);

            Gizmos.color = Color.green;
            Vector3 moused = GamplayCamera.instance.cam.ScreenToWorldPoint(Input.mousePosition);
            to = Cursor.GetCursor(false);
            Gizmos.DrawLine(moused, to);
            Gizmos.color *= .5f;
            to = Cursor.WorldPointToZ0(moused, GamplayCamera.instance.transform.forward);
            Gizmos.DrawLine(moused, to);
        }
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

    public static void GetCursor()
    {
        if(behaviour == null)
        {
            behaviour = new GameObject("Cursor", typeof(CursorBehaviour)).GetComponent<CursorBehaviour>();
        }
        behaviour.transform.position = GetCursor(locked);
    }

    public static Vector3 GetCursor(bool centerMouse) => GetCursor(out TileMap.TileBehaviour tileBehaviour, centerMouse);

    public static Vector3 GetCursor(out TileMap.TileBehaviour tileBehaviour,bool centerMouse)
    {
        TileMap.Tile tile = null;
        Vector3 from = GamplayCamera.instance.cam.ScreenToWorldPoint(centerMouse ? new Vector3(GamplayCamera.instance.cam.pixelWidth * .5f, GamplayCamera.instance.cam.pixelHeight * .5f, 0) : Input.mousePosition);
        if (Physics.Raycast(from, GamplayCamera.instance.transform.forward, out RaycastHit hit))
        {
            
            if (hit.collider.TryGetComponent(out Unit unit))
            {
                tile = unit.CurTile;
            }
            else if (hit.collider.TryGetComponent(out tileBehaviour))
            {
                tile = tileBehaviour.tile;
            }
            if (tile != null)
            {
                tileBehaviour = null;
                Vector3 pos = tile.PosInWorld;
                return pos += new Vector3(0, 0, .02f);
            }
        }
        Vector3 tilePos = TileMap.MapReader.AlignWorldPosToGrid(WorldPointToZ0(from, GamplayCamera.instance.transform.forward) + new Vector3(0, 0, .02f));
        tile = TileMap.MapReader.GetTile(TileMap.MapReader.WorldToGridSpace(tilePos));
        tileBehaviour = tile != null ? tile.TileBehaviour : null;
        return tilePos;
    }

    public static Vector3 WorldPointToZ0(Vector3 position, Vector3 forward)
    {
        float degree = Vector3.Angle(forward, Vector3.back) * Mathf.Deg2Rad;
        float distance = position.z/ Mathf.Cos(degree);
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
        Cursor.locked = EditorGUILayout.Toggle("Locked",Cursor.locked);
    }
}
#endif
