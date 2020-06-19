using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public Animator animatior;
    public Text text;

    void Start()
    {
        TurnManager.GameWinEvent += GameWin;
        TurnManager.GameLoseEvent += GameLose;
    }

    void GameWin()
    {
        Unsubscribe();
        text.text = "You Won!";
    }

    void GameLose()
    {
        Unsubscribe();
        text.text = "You Lost.";
    }

    void Unsubscribe()
    {
        TurnManager.GameWinEvent -= GameWin;
        TurnManager.GameLoseEvent -= GameLose;
        animatior.SetBool("Paused", true);
        animatior.SetBool("UnPauseable", true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            animatior.SetBool("Paused", !animatior.GetBool("Paused"));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Reload()
    {
        SceneManager.LoadScene(0);
    }
}
