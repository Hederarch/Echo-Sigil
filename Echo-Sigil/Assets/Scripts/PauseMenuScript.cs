using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject darkness;
    public Text text;
    private bool locked = false;

    void Start()
    {
        TurnManager.playerWinEvent += GameWin;
    }

    void GameWin(bool win)
    {
        locked = true;
        Unsubscribe();
        TogglePause();
        text.text = win ? "You Won!" : "You Lost.";
    }

    void Unsubscribe()
    {
        TurnManager.playerWinEvent -= GameWin;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void TogglePause()
    {
        if (!locked)
        {
            bool paused = pauseMenu.activeInHierarchy;
            pauseMenu.SetActive(!paused);
            darkness.SetActive(!paused);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
