using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isPaused {get; set;}

    private void Update()
    {
        PauseInputCheck();
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;

    }

    private void PauseInputCheck()
    {
        if (InputManager.PauseWasPressed)
        {
            if (!isPaused)
            {
                PauseGame();
            }

            else
            {
                UnpauseGame();
            }
        }
    }
}
