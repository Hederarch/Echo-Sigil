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
    public TilePos PosInGrid
    {
        get
        {
            return MapReader.WorldToGridSpace(transform.position);
        }
        set
        {
            transform.position = MapReader.GridToWorldSpace(value);
        }
    }

    public Tile CurTile => MapReader.GetTile(PosInGrid);

    public static Action<Unit> IsTurnEvent;

    public virtual string Tag => gameObject.tag;

    public Implement implement;

    public virtual void BeginTurn()
    {
        IsTurnEvent?.Invoke(this);
    }

    public virtual void EndTurn()
    {

    }

    public void SetSprite(Sprite sprite)
    {
        GameObject spriteRender = new GameObject(name + " Sprite Renderer");
        spriteRender.transform.parent = transform;
        SpriteRenderer spriteRenderer = spriteRender.AddComponent<SpriteRenderer>();
        unitSprite = spriteRenderer;
        unitSprite.sprite = sprite;
        BoxCollider boxCollider = spriteRender.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(1, 1, .2f);
        //sprites pivot is at the bottom, this is to compensate.
        boxCollider.center = Vector3.up * .5f;
        
    }

    public void SetValues(MapImplement.MovementSettings movementSettings)
    {

    }

    public void SetValues(MapImplement.BattleSettings battleSettings)
    {

    }

    private void SetValues(Implement implement)
    {
        SetSprite(implement.baseSprite);
        name = implement.name;
        this.implement = implement;
    }

    public static Unit GetUnit(MapImplement mapImplement, int modPathIndex)
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

            unit.PosInGrid = mapImplement.posInGrid;

            unit.SetValues(SaveSystem.Unit.GetImplement(mapImplement.name, modPathIndex));

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
