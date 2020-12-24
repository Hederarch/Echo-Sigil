using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TileMap;
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
        Vector2 direction = Input.GetAxisRaw("Horizontal") * GamplayCamera.instance.transform.right;
        direction += Input.GetAxisRaw("Vertical") * (Vector2)GamplayCamera.instance.transform.up;
        Vector2Int directionInt = new Vector2Int(Mathf.RoundToInt(direction.x),Mathf.RoundToInt(direction.y));
        if(directionInt != Vector2Int.zero)
        {
            Cursor.MoveCursor(directionInt);
        }
        GamplayCamera.instance.DesieredFoucus = MapReader.ConstrainToMap(Cursor.posInWorld);
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.locked = !Cursor.locked;
        }
    }

    public static void GetCursor()
    {
        if (behaviour == null)
        {
            behaviour = new GameObject("Cursor", typeof(CursorBehaviour)).GetComponent<CursorBehaviour>();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            behaviour.gameObject.SetActive(true);
        }
        Cursor.GetCursor(Cursor.locked);
        behaviour.transform.position = Cursor.posInWorld + (Vector3.forward * .02f);
    }

    public static void HideCursor()
    {
        behaviour.gameObject.SetActive(false);
    }
}

public static class Cursor
{
    public static bool locked = false;
    public static Unit unit;
    public static TileBehaviour tileBehaviour;
    public static Tile Tile => tileBehaviour.tile;
    public static TilePos posInGrid;
    public static Vector3 posInWorld => MapReader.GridToWorldSpace(posInGrid);
    public static Action GotCursorEvent;
    private static bool moveing;

    public static void GetCursor(bool centerMouse)
    {
        if ((centerMouse && !moveing) || !centerMouse)
        {
            Tile tile = null;
            unit = null;
            tileBehaviour = null;
            Vector3 from = centerMouse ? GamplayCamera.instance.transform.position : GamplayCamera.instance.cam.ScreenToWorldPoint(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(from, GamplayCamera.instance.transform.forward))
            {
                if (!centerMouse && hit.collider.transform.parent.TryGetComponent(out unit))
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
                    break;
                }
            }
            if (tileBehaviour == null)
            {
                TilePos pos = MapReader.WorldToGridSpace(WorldPointToZ0(from, GamplayCamera.instance.transform.forward));
                tile = MapReader.GetTile(pos);
                if (tile != null)
                {
                    posInGrid = tile.posInGrid;
                    tileBehaviour = tile.TileBehaviour;
                }
                else
                {
                    posInGrid = pos;
                }
            }
            GotCursorEvent?.Invoke();
        }
    }

    public static void MoveCursor(Vector2Int direction)
    {
        if (!moveing)
        {
            moveing = true;
            GamplayCamera.instance.finishedPositionChangedEvent += EndMoveing;
            GamplayCamera.instance.fixedCursor = false;
            posInGrid = MapReader.ConstrainToMap(posInGrid + direction);
            Tile tile = MapReader.GetTile(posInGrid);
            
            if(tile != null)
            {
                tileBehaviour = tile.TileBehaviour;
                unit = Tile.Unit;
            }
            else
            {
                tileBehaviour = null;
                unit = null;
            }
            GamplayCamera.instance.DesieredFoucus += new Vector3(direction.x,direction.y);
        }
    }

    private static void EndMoveing()
    {
        moveing = false;
        GamplayCamera.instance.finishedPositionChangedEvent -= EndMoveing;
        GamplayCamera.instance.fixedCursor = true;
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
