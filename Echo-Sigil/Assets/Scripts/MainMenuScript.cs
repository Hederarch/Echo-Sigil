using System;
using UnityEngine;
using UnityEngine.UI;
using MapEditor;
using System.Collections.Generic;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    public Dropdown mapDropdown;
    public Toggle developerToggle;

    public Transform canvas;
    public GameObject gameplayGUIElements;
    public GameObject mainMenuElements;
    public GameObject loadingElements;

    public void Start()
    {
        developerToggle.isOn = SaveSystem.developerMode;
        developerToggle.onValueChanged.AddListener(delegate { SaveSystem.developerMode = developerToggle.isOn; });
    }

    public void NewGame()
    {
        StartCoroutine("StartGame",new Map (5,5));
    }

    public void LoadGame()
    {
        throw new NotImplementedException();
    }

    private IEnumerable StartGame(Map map)
    {
        mainMenuElements.SetActive(false);
        Instantiate(loadingElements, canvas);
        yield return null;
        Instantiate(gameplayGUIElements, canvas);
        MapReader.GenerateVirtualMap(map);
        MapReader.GeneratePhysicalMap(map);
        TurnManager.InitTeamTurnQueue();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        developerToggle.onValueChanged.RemoveAllListeners();
    }
}
