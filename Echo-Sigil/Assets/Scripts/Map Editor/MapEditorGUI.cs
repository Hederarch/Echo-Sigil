using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorGUI : MonoBehaviour
{
    public MapEditor editor;

    public Image SelectedImage;

    public GameObject tileGUI;
    public Text tileName;
    public Toggle walkable;
    public InputField height;

    public GameObject unitGUI;
    public InputField unitName;

    //File
    public InputField mapName;
    public InputField x;
    public InputField y;

    // Start is called before the first frame update
    void Start()
    {
        editor.NewSelected += UpdatePanel;
    }

    void UpdatePanel(Transform selected)
    {
        if (selected != null)
        {
            SelectedImage.color = Color.white;
            SelectedImage.sprite = selected.GetComponent<SpriteRenderer>().sprite;
            if (selected.GetComponent<TileBehaviour>() != null)
            {
                Tile tile = selected.GetComponent<TileBehaviour>().tile;
                tileGUI.SetActive(true);
                unitGUI.SetActive(false);
                tileName.text = selected.name;
                walkable.isOn = tile.walkable;
                height.text = tile.height.ToString();
            }
            else if (selected.parent.GetComponent<Implement>() != null)
            {
                tileGUI.SetActive(false);
                unitGUI.SetActive(true);
            }
        } 
        else
        {
            tileGUI.SetActive(false);
            unitGUI.SetActive(false);
            SelectedImage.color = Color.clear;
        }
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
        MapReader.GeneratePhysicalMap(editor.pallate, new Map(int.Parse(x.text), int.Parse(y.text)));
    }

}
