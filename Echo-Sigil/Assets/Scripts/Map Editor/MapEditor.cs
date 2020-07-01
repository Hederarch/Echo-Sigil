using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public SpritePallate pallate;

    Tile selectedTile;
    Implement selectedImplement;
    Transform selectedTransform;
    
    Vector2 prevMousePos;
    private bool locked;

    public float snappingDistance = .1f;

    public event Action<Transform> NewSelected;

    // Update is called once per frame
    void Update()
    {
        if(MapReader.map != null)
        {
            Select();
            TileEdits();
            UnitEdits();
            prevMousePos = Input.mousePosition;
        }
    }

    private void UnitEdits()
    {
        if (Input.GetMouseButton(1) && selectedImplement != null)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
            ChangeUnitPos(hit.point);
        }
    }

    private void TileEdits()
    {
        if (Input.GetMouseButton(1) && selectedTile != null)
        {
            ChangeTileHeight((Input.mousePosition.y - prevMousePos.y) * .1f);
        }
    }

    public bool ChangeTileHeight(float delta , bool snap = true)
    {
        if (selectedTile == null)
        {
            return false;
        }
        else
        {
            float desierdHeight = selectedTile.height + delta;

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
            return true;
        }

    }

    public void ChangeTileWalkable(bool walkable)
    {
        selectedTile.walkable = walkable;
    }
    
    public void ChangeTileHeight(string delta)
    {
        ChangeTileHeight(float.Parse(delta) - selectedTile.height,false);
    }

    public bool ChangeUnitPos(Vector3 point)
    {
        return ChangeUnitPos(MapReader.WorldToGridSpace(point));
    }

    public bool ChangeUnitPos(Vector2 point)
    {
        return ChangeUnitPos(MapReader.WorldToGridSpace(point));
    }

    public bool ChangeUnitPos(int x, int y)
    {
        return ChangeUnitPos(new Vector2Int(x,y));
    }

    public bool ChangeUnitPos(Vector2Int pointOnGrid)
    {
        if(selectedImplement == null)
        {
            return false;
        } 
        else
        {
            Vector2 worldSpace = MapReader.GridToWorldSpace(pointOnGrid);
            Tile tile = MapReader.GetTile(pointOnGrid);
            selectedImplement.transform.position = new Vector3(worldSpace.x, worldSpace.y, tile.height + .1f);
            NewSelected?.Invoke(selectedTransform);
            return true;
        }
    }

    void Select()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && !locked && hit.transform != selectedTransform)
            {
                selectedTransform = hit.transform;
                NewSelected?.Invoke(selectedTransform);

                //tiles
                if (selectedTransform.GetComponent<TileBehaviour>() != null)
                {
                    selectedTile = selectedTransform.GetComponent<TileBehaviour>().tile;
   
                } else
                {
                    selectedTile = null;
                }

                //Units
                if (selectedTransform.GetComponentInParent<Implement>() != null)
                {
                    selectedImplement = selectedTransform.GetComponentInParent<Implement>();
                } else
                {
                    selectedImplement = null;
                }
            } 
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)) && Input.GetMouseButtonDown(0))
            {
                locked = true;
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
                    selectedTile = null;
                    NewSelected?.Invoke(selectedTransform);
                }
            }
            prevMousePos = Input.mousePosition;
        }
        
    }

}
