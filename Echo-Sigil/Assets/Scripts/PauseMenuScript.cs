using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public Animator animatior;
    public Text text;

    void Start()
    {
        TurnManager.playerWinEvent += GameWin;
    }

    void GameWin(bool win)
    {
        Unsubscribe();
        text.text = win ? "You Won!" : "You Lost.";
    }

    void Unsubscribe()
    {
        TurnManager.playerWinEvent -= GameWin;
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
