using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public SpritePallate pallate;

    Transform selectedTransform;
    List<Transform> selectedTransforms = new List<Transform>();

    private bool locked;

    public float snappingDistance = .1f;

    //public Image selectBox;
    //Vector2 boxSelectStartPos;
    //[Range(0, 1)]
    //public float selectBoxTransparency = .2f;

    public event Action<Transform> SelectedEvent;
    public event Action<Transform[]> MultiSelectedEvent;

    // Update is called once per frame
    void Update()
    {
        if (MapReader.map != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Select();
            }
            //SelectBox();
            TileEdits();
        }
    }

    private void TileEdits()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            if (selectedTransforms.Count > 1)
            {
                ChangeTileHeight(Input.GetAxisRaw("Mouse ScrollWheel"), selectedTransforms.ToArray());
                MultiSelectedEvent?.Invoke(selectedTransforms.ToArray());
            }
            if (selectedTransform != null)
            {
                ChangeTileHeight(selectedTransform.position.z + Input.GetAxisRaw("Mouse ScrollWheel"), selectedTransform);
                SelectedEvent?.Invoke(selectedTransform);
            }
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

    public bool ChangeTileHeight(float desierdHeight, Transform tileTransform, bool snap = false)
    {
        if (tileTransform.TryGetComponent(out TileBehaviour tile))
        {
            Tile selectedTile = tile;
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
            tileTransform.position = new Vector3(tileTransform.position.x, tileTransform.position.y, desierdHeight);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeTileHeight(float delta, Transform[] tileTransform, bool snap = true)
    {
        Dictionary<Transform, Tile> tiles = new Dictionary<Transform, Tile>();
        List<Transform> tileTransforms = new List<Transform>();
        foreach (Transform t in tileTransform)
        {
            if (t.TryGetComponent(out TileBehaviour tile))
            {
                tiles.Add(t, tile);
                tileTransforms.Add(t);
            }
        }
        if (snap)
        {
            Tile closestNonSelected = null;
            Tile closestSelected = null;
            foreach (Transform t in tileTransform)
            {
                //Snap to adjacent tiles
                tiles[t].FindNeighbors(float.PositiveInfinity);
                
                foreach (Tile tile in tiles[t].adjacencyList)
                {
                    if ((closestNonSelected == null && Math.Abs(tiles[t].height + delta - tile.height) < snappingDistance) || (closestNonSelected != null && Math.Abs(tiles[t].height + delta - closestNonSelected.height) > Math.Abs(tiles[t].height + delta - tile.height) && !tile.current))
                    {
                        closestNonSelected = tile;
                        closestSelected = tiles[t];
                    }
                }
            }
            if (closestNonSelected != null)
            {
                delta = closestNonSelected.height - closestSelected.height;
                Debug.DrawLine(closestNonSelected.PosInWorld, closestSelected.PosInWorld);
            }
        }
        foreach (Transform t in tileTransforms)
        {
            tiles[t].height += delta;
            t.position += new Vector3(0, 0, delta);
        }
    }

    public void ChangeTileWalkable(bool walkable, Tile selectedTile)
    {
        selectedTile.walkable = walkable;
    }

    public void ChangeTileHeight(string delta, Tile selectedTile, Transform tileTransform)
    {
        ChangeTileHeight(float.Parse(delta) - selectedTile.height, tileTransform, false);
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
        SelectedEvent?.Invoke(selectedTransform);
    }

    internal void ChangeHealthWill(bool health, int value, Implement implement)
    {
        JRPGBattle j = implement.battle as JRPGBattle;
        if (health)
        {
            j.health = value;
        }
        else
        {
            j.will = value;
        }
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && (!locked && hit.transform != selectedTransform || Input.GetMouseButtonDown(0)))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && selectedTransform.TryGetComponent(out TileBehaviour t))
            {
                if (!selectedTransforms.Contains(hit.transform))
                {
                    selectedTransforms.Add(hit.transform);
                }
                else
                {
                    selectedTransforms.Remove(hit.transform);
                }

                if (selectedTransform != null)
                {
                    selectedTransforms.Add(selectedTransform);
                    selectedTransform = null;
                }
                MultiSelectedEvent?.Invoke(selectedTransforms.ToArray());
            }
            else
            {
                selectedTransform = hit.transform;
                SelectedEvent?.Invoke(selectedTransform);
            }

            if (Input.GetMouseButtonDown(0))
            {
                locked = true;
            }
        }
        else if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)))
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                locked = false;
            }
            if (!locked)
            {
                selectedTransform = null;
                selectedTransforms.Clear();
                SelectedEvent?.Invoke(selectedTransform);
            }
        }
    }

    //this was a box select option, scraped for time
    /*private void SelectBox()
    {
        if (Input.GetMouseButtonDown(0))
        {
            boxSelectStartPos = Input.mousePosition;
            selectBox.enabled = true;
        }
        if (Input.GetMouseButton(0))
        {

            float width = Input.mousePosition.x - boxSelectStartPos.x;
            float height = Input.mousePosition.y - boxSelectStartPos.y;

            selectBox.rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectBox.rectTransform.anchoredPosition = boxSelectStartPos + new Vector2(width / 2, height / 2);
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectBox.enabled = false;

            Vector2 minBox = Camera.main.ScreenToWorldPoint(selectBox.rectTransform.anchoredPosition - (selectBox.rectTransform.sizeDelta / 2));
            Vector2 maxBox = Camera.main.ScreenToWorldPoint(selectBox.rectTransform.anchoredPosition + (selectBox.rectTransform.sizeDelta / 2));

            float xSize = maxBox.x - minBox.x;
            float ySize = maxBox.y - minBox.y;


            Bounds bounds = new Bounds(Camera.main.ScreenToWorldPoint(selectBox.rectTransform.anchoredPosition),new Vector3(xSize,ySize,float.PositiveInfinity));

            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                selectedTransforms.Clear();
            }

            foreach (Transform t in MapReader.tileParent)
            {
                if (t.GetComponent<TileBehaviour>() && bounds.Contains(t.position))
                {
                    selectedTransforms.Add(t);
                }
            }
            
            if(selectedTransforms.Count > 1)
            {
                BoxSelectedEvent?.Invoke(selectedTransforms.ToArray());
                locked = true;
            } 
            else if (selectedTransforms.Count == 1)
            {
                SelectedEvent(selectedTransforms[0]);
                locked = true;
            }
        }
    }*/

}
