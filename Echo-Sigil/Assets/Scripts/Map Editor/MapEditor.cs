using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public SpritePallate pallate;

    Transform selectedTransform;

    private bool locked;

    public float snappingDistance = .1f;

    public event Action<Transform> NewSelected;

    // Update is called once per frame
    void Update()
    {
        if (MapReader.map != null)
        {
            Select();
        }
    }

    public void ChangeIsPlayer(bool player, Implement selectedImplement)
    {
        if (selectedImplement != null)
        {

            Transform transform = selectedImplement.transform;
            SpriteRenderer sprite = selectedImplement.unitSprite;

            TacticsMove t = selectedImplement.move as TacticsMove;
            int moveDistance = t.moveDistance;
            float moveSpeed = t.moveSpeed;
            float jumpHeight = t.jumpHeight;
            Destroy(t);

            JRPGBattle j = selectedImplement.battle as JRPGBattle;
            int maxHeath = j.maxHealth;
            int health = j.health;
            int maxWill = j.maxWill;
            int will = j.will;
            int reach = j.reach;
            Destroy(j);

            Destroy(selectedImplement);

            if (player)
            {
                selectedImplement = transform.gameObject.AddComponent<PlayerImplement>();
                selectedImplement.move = transform.gameObject.AddComponent<PlayerMove>();
                selectedImplement.battle = transform.gameObject.AddComponent<PlayerBattle>();
            }
            else
            {
                selectedImplement = transform.gameObject.AddComponent<NPCImplement>();
                selectedImplement.move = transform.gameObject.AddComponent<NPCMove>();
                selectedImplement.battle = transform.gameObject.AddComponent<NPCBattle>();
            }

            selectedImplement.unitSprite = sprite;

            t = selectedImplement.move as TacticsMove;
            t.moveDistance = moveDistance;
            t.moveSpeed = moveSpeed;
            t.jumpHeight = jumpHeight;

            j = selectedImplement.battle as JRPGBattle;
            j.maxHealth = maxHeath;
            j.health = health;
            j.maxWill = maxWill;
            j.will = will;
            j.reach = reach;

        }
    }

    public void ChangeTileHeight(float desierdHeight, Tile selectedTile, Transform tileTransform, bool snap = true)
    {
        if (snap)
        {
            //Snap to adjacent tiles
            selectedTile.FindNeighbors(float.PositiveInfinity);
            Tile closest = null;
            foreach (Tile t in selectedTile.adjacencyList)
            {
                if ((closest == null && Math.Abs(desierdHeight - t.height) < snappingDistance) || (closest != null && Math.Abs(desierdHeight - closest.height) > Math.Abs(desierdHeight - t.height)))
                {
                    closest = t;
                }
            }
            if (closest != null)
            {
                desierdHeight = closest.height;
            }
        }
        selectedTile.height = desierdHeight;
        selectedTransform.position = new Vector3(selectedTransform.position.x, selectedTransform.position.y, desierdHeight);
        NewSelected?.Invoke(selectedTransform);
    }

    public void ChangeTileWalkable(bool walkable, Tile selectedTile)
    {
        selectedTile.walkable = walkable;
    }

    public void ChangeTileHeight(string delta, Tile selectedTile, Transform tileTransform)
    {
        ChangeTileHeight(float.Parse(delta) - selectedTile.height, selectedTile, tileTransform, false);
    }

    public void ChangeUnitPos(Vector3 point, Implement selectedImplement)
    {
        ChangeUnitPos(MapReader.WorldToGridSpace(point), selectedImplement);
    }

    public void ChangeUnitPos(Vector2 point, Implement selectedImplement)
    {
        ChangeUnitPos(MapReader.WorldToGridSpace(point), selectedImplement);
    }

    public void ChangeUnitPos(int x, int y, Implement selectedImplement)
    {
        ChangeUnitPos(new Vector2Int(x, y), selectedImplement);
    }

    public void ChangeUnitPos(Vector2Int pointOnGrid, Implement selectedImplement)
    {
        Vector2 worldSpace = MapReader.GridToWorldSpace(pointOnGrid);
        Tile tile = MapReader.GetTile(pointOnGrid);
        selectedImplement.transform.position = new Vector3(worldSpace.x, worldSpace.y, tile.height + .1f);
        NewSelected?.Invoke(selectedTransform);
    }

    public void ChangeNumVariable(string value, string variable, Implement selectedImplement)
    {
        TacticsMove t = selectedImplement.move as TacticsMove;
        JRPGBattle j = selectedImplement.battle as JRPGBattle;
        switch (variable)
        {
            case "Move Distance":
                t.moveDistance = int.Parse(value);
                break;
            case "Move Speed":
                t.moveSpeed = float.Parse(value);
                break;
            case "Jump Height":
                t.jumpHeight = float.Parse(value);
                break;
            case "Max Health":
                j.maxHealth = int.Parse(value);
                break;
            case "Max Will":
                j.maxWill = int.Parse(value);
                break;
            case "Reach":
                j.reach = int.Parse(value);
                break;
        }
    }

    void Select()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && ((!locked && hit.transform != selectedTransform) || Input.GetMouseButtonDown(0)))
            {
                selectedTransform = hit.transform;
                NewSelected?.Invoke(selectedTransform);

                if (Input.GetMouseButtonDown(0))
                {
                    locked = true;
                }
            }
            else if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    locked = false;
                }
                if (!locked)
                {
                    selectedTransform = null;
                    NewSelected?.Invoke(selectedTransform);
                }
            }
        }

    }

}
