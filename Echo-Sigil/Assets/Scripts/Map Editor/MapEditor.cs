using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public SpritePallate pallate;

    Tile selectedTile;
    Transform selectedTransform;

    Tile prevTile;
    
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

    }

    private void TileEdits()
    {
        if (Input.GetMouseButton(0))
        {
            ChangeTileHeight((Input.mousePosition.y - prevMousePos.y) * .1f);
        }
    }

    public bool ChangeTileHeight(float delta)
    {
        float desierdHeight = 0;
        if (selectedTile != null)
        {
            desierdHeight = selectedTile.height + delta;

            //Snap to adjacent tiles
            selectedTile.FindNeighbors(float.PositiveInfinity);
            foreach (Tile t in selectedTile.adjacencyList)
            {
                if (Math.Abs(desierdHeight - t.height) < snappingDistance)
                {
                    desierdHeight = t.height;
                    break;
                }
            }
            selectedTile.height = desierdHeight;
            selectedTransform.position = new Vector3(selectedTransform.position.x, selectedTransform.position.y, desierdHeight);
            return true;
        } 
        else
        {
            return false;
        }

    }

    public void ChangeTileWalkable(bool walkable)
    {
        selectedTile.walkable = walkable;
    }
    
    public void ChangeTileHeight(string delta)
    {
        ChangeTileHeight(float.Parse(delta) - selectedTile.height);
    }

    void Select()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) )
        {
            if ((!locked && hit.transform != selectedTransform) || Input.GetMouseButtonDown(0))
            {
                selectedTransform = hit.transform;

                if(prevTile != null)
                {
                    prevTile.current = false;
                    prevTile = null;
                }
                if (selectedTransform.GetComponent<TileBehaviour>() != null)
                {
                    selectedTile = selectedTransform.GetComponent<TileBehaviour>().tile;
                    selectedTile.current = true;
                    prevTile = selectedTile;
                }
                prevMousePos = Input.mousePosition;
                NewSelected?.Invoke(selectedTransform);
            }
            if (Input.GetMouseButtonDown(0))
            {
                locked = true;
            }
        } 
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                locked = false;
                selectedTransform = null;
                selectedTile = null;
                if (prevTile != null)
                {
                    prevTile.current = false;
                }
                prevTile = null;
            }
        }
        NewSelected?.Invoke(selectedTransform);
    }


}
