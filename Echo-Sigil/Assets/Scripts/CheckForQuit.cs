using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckForQuit : MonoBehaviour
{
    public Text bugText;
    // Update is called once per frame
    void Update()
    {
        if(Camera.main.GetComponent<TacticsCamera>().desieredAngle == (float)Math.PI)
        {
            bugText.text = "Known bug! only happens at this angle, dont know why. Rotate the camera to 'fix'";
        } else
        {
            bugText.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
