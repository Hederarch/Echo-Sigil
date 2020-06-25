using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public Tile selectedTile;
    Vector2 prevMousePos;

    public event Action<Tile> NewSelectedTile;

    // Update is called once per frame
    void Update()
    {
        if(MapReader.map != null)
        {
            SelectTile();
            ChangeTileHeight();
        }
    }

    private void ChangeTileHeight()
    {
        float delta = prevMousePos.y - Input.mousePosition.y;
        if (Input.GetMouseButton(0) && Math.Abs(delta) > .5f && selectedTile != null)
        {
            selectedTile.height += delta;
            MapReader.map.heightmap[selectedTile.PosInGrid.x, selectedTile.PosInGrid.x] = selectedTile.height;
        }
        prevMousePos = Input.mousePosition;
    }

    void SelectTile()
    {
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            selectedTile = hit.collider.GetComponent<TileBehaviour>().tile;
            NewSelectedTile?.Invoke(selectedTile);
        }
    }
}
