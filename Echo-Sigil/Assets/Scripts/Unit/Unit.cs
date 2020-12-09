using System;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using TileMap;

/// <summary>
/// Character in the world
/// </summary>
public class Unit : FacesCamera, ITurn
{
    public TilePos posInGrid => MapReader.WorldToGridSpace(transform.position);

    public Tile CurTile => MapReader.GetTile(posInGrid);

    public static Action<Unit> IsTurnEvent;

    public virtual string Tag => gameObject.tag;

    public virtual void BeginTurn()
    {
        IsTurnEvent?.Invoke(this);
    }

    public virtual void EndTurn()
    {
        
    }

    public void SetPos(TilePos tilePos)
    {
        transform.position = MapReader.GridToWorldSpace(tilePos);
    }

    public void SetSprite()
    {
        GameObject spriteRender = new GameObject(name + " Sprite Renderer");
        spriteRender.transform.parent = transform;
        spriteRender.transform.localPosition = new Vector3(0, 0, .1f);
        SpriteRenderer spriteRenderer = spriteRender.AddComponent<SpriteRenderer>();
        unitSprite = spriteRenderer;
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        spriteRender.AddComponent<BoxCollider>().size = new Vector3(1, 1, .2f);
    }

    public void SetValues(MapImplement.MovementSettings movementSettings)
    {

    }

    public void SetValues(MapImplement.BattleSettings battleSettings)
    {

    }

    public static Unit GetUnit(MapImplement mapImplement)
    {
        if (mapImplement != null)
        {
            GameObject unitObject = new GameObject(mapImplement.name);
            unitObject.transform.parent = MapReader.tileParent;

            Unit unit;
            if (mapImplement.player)
            {
                unit = unitObject.AddComponent<PlayerUnit>();
            }
            else
            {
                unit = unitObject.AddComponent<NPCUnit>();
            }

            TurnManager.AddUnit(unit);
            MapReader.implements.Add(unit);

            unit.SetValues(mapImplement.movementSettings);
            unit.SetValues(mapImplement.battleSettings);

            unit.SetSprite();
            unit.SetPos(mapImplement.posInGrid);

            

            return unit;
        }
        return null;
    }

    private void OnDestroy()
    {
        TurnManager.RemoveUnit(this);
    }

    public virtual Color GetTeamColor()
    {
        return Color.black;
    }

    public virtual Texture2D GetTeamTexture()
    {
        return null;
    }
}
