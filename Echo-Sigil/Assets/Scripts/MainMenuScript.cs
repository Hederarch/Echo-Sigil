using System;
using UnityEngine;
using UnityEngine.UI;
using TileMap;
using SaveSystem;

public class MainMenuScript : MonoBehaviour
{
    public Dropdown mapDropdown;
    public Toggle developerToggle;

    public Transform canvas;
    public GameObject gameplayGUIElements;
    public GameObject mainMenuElements;
    

    public void Start()
    {
        developerToggle.isOn = Mod.developerMode;
        developerToggle.onValueChanged.AddListener(delegate { Mod.developerMode = developerToggle.isOn; });
    }

    public void NewGame()
    {
        StartGame(new TileMap.Map(5,5));
    }

    public void LoadGame()
    {
        throw new NotImplementedException();
    }

    private void StartGame(TileMap.Map map)
    {
        mainMenuElements.SetActive(false);
        Instantiate(gameplayGUIElements, canvas);
        TurnManager.Reset();
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
