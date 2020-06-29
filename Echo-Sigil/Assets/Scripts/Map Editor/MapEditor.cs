using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public Tile selectedTile;
    public Transform selectedTransform;
    Vector2 prevMousePos;

    public event Action<Tile,Transform> NewSelectedTile;

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
        float delta = (prevMousePos.y - Input.mousePosition.y) * .1f;
        if (Input.GetMouseButton(0) && selectedTile != null)
        {
            selectedTile.height -= delta;
            selectedTransform.position -= new Vector3(0, 0, delta);
        }
        prevMousePos = Input.mousePosition;
    }

    void SelectTile()
    {
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            selectedTransform = hit.collider.transform;
            selectedTile = selectedTransform.GetComponent<TileBehaviour>().tile;
            prevMousePos = Input.mousePosition;
            NewSelectedTile?.Invoke(selectedTile,selectedTransform);
        }
    }
}
