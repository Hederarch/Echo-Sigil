using System;
using UnityEngine;
using UnityEngine.UI;
using MapEditor;

public class MainMenuScript : MonoBehaviour
{
    public Dropdown mapDropdown;
    public Toggle developerToggle;

    public Transform canvas;
    public GameObject gameplayGUIElements;
    public GameObject mainMenuElements;
    

    public void Start()
    {
        developerToggle.isOn = SaveSystem.developerMode;
        developerToggle.onValueChanged.AddListener(delegate { SaveSystem.developerMode = developerToggle.isOn; });
    }

    public void NewGame()
    {
        StartGame(new Map(5,5));
    }

    public void LoadGame()
    {
        throw new NotImplementedException();
    }

    private void StartGame(Map map)
    {
        mainMenuElements.SetActive(false);
        Instantiate(gameplayGUIElements, canvas);
        TurnManager.Reset();
        MapReader.GenerateVirtualMap(map);
        MapReader.GeneratePhysicalMap(map);
        TurnManager.StartTurn();
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
