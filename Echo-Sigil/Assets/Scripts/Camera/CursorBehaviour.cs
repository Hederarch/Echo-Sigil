using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CursorBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer floatingSpriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        floatingSpriteRenderer = new GameObject("floating cursor", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SaveSystem.Tile.GetCursorTexture();
        floatingSpriteRenderer.sprite = SaveSystem.Tile.GetFloatingCursorTexture();
    }
}

public static class Cursor
{
    private static CursorBehaviour behaviour;
    internal static CursorBehaviour GetCursor()
    {
        if(behaviour == null)
        {
            behaviour = new GameObject("Cursor", typeof(CursorBehaviour)).GetComponent<CursorBehaviour>();
        }
        if(Physics.Raycast(GamplayCamera.instance.transform.position,GamplayCamera.instance.transform.forward,out RaycastHit hit))
        {
            TileMap.TileBehaviour tileBehaviour = null;
            if (hit.collider.TryGetComponent(out Unit unit))
            {
                tileBehaviour = unit.GetCurTile();
            }
            else if(hit.collider.TryGetComponent(out tileBehaviour) || tileBehaviour != null)
            {
                behaviour.transform.position = tileBehaviour.tile.PosInWorld;
            }
        }
        else
        {
            behaviour.transform.position = TileMap.MapReader.AlignWorldPosToGrid(WorldPointToZ0(GamplayCamera.instance.transform.position, GamplayCamera.instance.transform.forward));
        }
        return behaviour;
    }

    private static Vector3 WorldPointToZ0(Vector3 position, Vector3 forward)
    {
        float distance = Mathf.Cos(Vector3.Angle(forward,Vector3.down)*Mathf.Deg2Rad) * position.z;
        return position + forward.normalized * distance;
    }
}
