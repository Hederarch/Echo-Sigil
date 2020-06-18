using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public Animator animatior;
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
}
