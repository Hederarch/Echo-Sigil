using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorGUI : MonoBehaviour
{
    public MapEditor editor;

    public Image SelectedImage;

    //tile
    public GameObject tileGUI;
    public Text tileName;
    public Toggle walkable;
    public InputField height;
    Tile prevTile;

    //unit
    public GameObject unitGUI;
    public InputField unitName;
    public Toggle player;
    public InputField unitPosX;
    public InputField unitPosY;
    public InputField moveDistance;
    public InputField moveSpeed;
    public InputField jumpHeight;
    public InputField maxHeath;
    public Slider health;
    public InputField maxWill;
    public Slider will;
    public InputField reach;

    //File
    public InputField mapName;
    public InputField sizeX;
    public InputField sizeY;

    // Start is called before the first frame update
    void Start()
    {
        editor.NewSelected += UpdatePanel;
        editor.NewSelected += ResetPrevTile;
    }

    private void ResetPrevTile(Transform selected)
    {
        if (prevTile != null)
        {
            prevTile.current = false;
        }
        if (selected != null && selected.GetComponent<TileBehaviour>() != null)
        {
            Tile selTile = selected.GetComponent<TileBehaviour>();
            selTile.current = true;
            prevTile = selTile;
        } 
        else
        {
            prevTile = null;
        }
    }

    void UpdatePanel(Transform selected)
    {
        UnSubscribeTile();
        UnSubscribeUnit();
        if (selected != null)
        {
            SelectedImage.color = Color.white;
            SelectedImage.sprite = selected.GetComponent<SpriteRenderer>().sprite;
            if (selected.GetComponent<TileBehaviour>() != null)
            {
                Tile tile = selected.GetComponent<TileBehaviour>();
                SetTileProperties(selected,tile);
                SubscribeTile(selected,tile);
            }
            else if (selected.parent.GetComponent<Implement>() != null)
            {
                Implement implement = selected.parent.GetComponent<Implement>();
                SetUnitPanelProperties(implement);
                SubscribeUnit(implement);
            }
        } 
        else
        {
            tileGUI.SetActive(false);
            unitGUI.SetActive(false);
            SelectedImage.color = Color.clear;
        }
    }

    private void SubscribeTile(Transform selected, Tile selectedTile)
    {
        walkable.onValueChanged.AddListener(delegate { editor.ChangeTileWalkable(walkable.isOn, selectedTile); });
        height.onValueChanged.AddListener(delegate { editor.ChangeTileHeight(float.Parse(height.text), selectedTile, selected, false); });
    }

    private void SetTileProperties(Transform selected, Tile tile)
    {
        unitGUI.SetActive(false);

        tileName.text = selected.name;
        walkable.isOn = tile.walkable;
        height.text = tile.height.ToString();

        tileGUI.SetActive(true);
    }

    private void UnSubscribeTile()
    {
        walkable.onValueChanged.RemoveAllListeners();
        height.onValueChanged.RemoveAllListeners();
    }

    private void SubscribeUnit(Implement implement)
    {
        
    }

    private void SetUnitPanelProperties(Implement implement)
    {
        tileGUI.SetActive(false);

        TacticsMove tacticsMove = (TacticsMove)implement.move;
        JRPGBattle jRPGBattle = (JRPGBattle)implement.battle;

        unitName.text = implement.name;

        if (implement is PlayerImplement)
        {
            player.isOn = true;
        }
        else
        {
            player.isOn = false;
        }
        Vector2Int unitPos = MapReader.WorldToGridSpace(implement.transform.position);
        if (unitPos != null)
        {
            unitPosX.text = unitPos.x.ToString();
            unitPosY.text = unitPos.y.ToString();
        }
        else
        {
            unitPosX.text = "";
            unitPosY.text = "";
        }

        moveDistance.text = tacticsMove.moveDistance.ToString();
        moveSpeed.text = tacticsMove.moveSpeed.ToString();
        jumpHeight.text = tacticsMove.jumpHeight.ToString();

        maxHeath.text = jRPGBattle.maxHealth.ToString();
        health.maxValue = jRPGBattle.maxHealth;
        health.value = jRPGBattle.health;

        maxWill.text = jRPGBattle.maxWill.ToString();
        will.maxValue = jRPGBattle.maxWill;
        will.value = jRPGBattle.will;

        reach.text = jRPGBattle.reach.ToString();

        unitGUI.SetActive(true);
    }

    private void UnSubscribeUnit()
    {
        
    }

    public void ChangeUnitPos(Implement selectedImplement)
    {
        editor.ChangeUnitPos(int.Parse(unitPosX.text),int.Parse(unitPosY.text),selectedImplement);
    }

    public void SaveMap()
    {
        MapReader.SaveMap(mapName.text);
    }

    public void LoadMap()
    {
        MapReader.LoadMap(mapName.text, editor.pallate);
    }

    public void NewMap()
    {
        MapReader.GeneratePhysicalMap(editor.pallate, new Map(int.Parse(sizeX.text), int.Parse(sizeY.text)));
    }

}
