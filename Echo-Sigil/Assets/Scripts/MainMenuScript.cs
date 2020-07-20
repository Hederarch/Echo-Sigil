using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Animator mainMenuAnimator;
    public Transform canvas;
    public GameObject mapEditor;
    public GameObject gameplayGUIElements;
    public void NewGame()
    {
        StartGame();
    }

    public void LoadGame()
    {
        Map map = null;
        throw new NotImplementedException();
        StartGame(map);
    }

    private void StartGame(Map map = null)
    {
        mainMenuAnimator.SetTrigger("Exit");
        Instantiate(gameplayGUIElements, canvas);
        if(map == null)
        {
            throw new NotImplementedException();
        }
        LoadMap(map);
    }

    void LoadMap(Map map)
    {
        MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Directory.GetParent(map.path).FullName), map);
    }

    public void MapEditor()
    {
        mainMenuAnimator.SetTrigger("Exit");
        Instantiate(mapEditor, canvas);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
