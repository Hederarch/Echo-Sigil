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
    public Text unitName;
    public Toggle player;
    public InputField unitPosX;
    public InputField unitPosY;
    public InputField moveDistance;
    public InputField moveSpeed;
    public InputField jumpHeight;
    public InputField maxHealth;
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
        height.onEndEdit.AddListener(delegate { editor.ChangeTileHeight(float.Parse(height.text), selectedTile, selected, false); });
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
        height.onEndEdit.RemoveAllListeners();
    }

    private void SubscribeUnit(Implement implement)
    {
        player.onValueChanged.AddListener(delegate { editor.ChangeIsPlayer(player.isOn, implement);  });
        unitPosX.onEndEdit.AddListener(delegate { editor.ChangeUnitPos(int.Parse(unitPosX.text), int.Parse(unitPosY.text), implement); });
        unitPosY.onEndEdit.AddListener(delegate { editor.ChangeUnitPos(int.Parse(unitPosX.text), int.Parse(unitPosY.text), implement); });
        moveDistance.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(moveDistance.text, "Move Distance", implement); });
        moveSpeed.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(moveSpeed.text, "Move Speed", implement); });
        jumpHeight.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(jumpHeight.text, "Jump Height", implement); });
        maxHealth.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(maxHealth.text, "Max Health", implement); });
        health.onValueChanged.AddListener(delegate { editor.ChangeHealthWill(true, (int)health.value, implement); });
        maxWill.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(maxWill.text, "Max Will", implement); });
        will.onValueChanged.AddListener(delegate { editor.ChangeHealthWill(false, (int)will.value, implement); });
        reach.onEndEdit.AddListener(delegate { editor.ChangeNumVariable(reach.text, "Reach", implement); });
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

        maxHealth.text = jRPGBattle.maxHealth.ToString();
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
        player.onValueChanged.RemoveAllListeners();
        unitPosX.onEndEdit.RemoveAllListeners();
        unitPosY.onEndEdit.RemoveAllListeners();
        moveDistance.onEndEdit.RemoveAllListeners();
        moveSpeed.onEndEdit.RemoveAllListeners();
        jumpHeight.onEndEdit.RemoveAllListeners();
        maxHealth.onEndEdit.RemoveAllListeners();
        health.onValueChanged.RemoveAllListeners();
        maxWill.onEndEdit.RemoveAllListeners();
        will.onValueChanged.RemoveAllListeners();
        reach.onEndEdit.RemoveAllListeners();
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
