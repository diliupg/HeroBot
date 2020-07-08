using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public string levelSelect;
    public string mainMenu;

    public GameObject pauseScreen;

    //private PlayerController thePlayer;

    //private LevelManager levelManager;

    private void OnEnable ( )
    {
        PlayerController.OnEscapePressed += PauseOrResume;
    }

    private void OnDisable ( )
    {
        PlayerController.OnEscapePressed -= PauseOrResume;
    }

    void PauseOrResume()
    {
        if ( Time.timeScale == 0 )
        {
            ResumeGame ( );
        }
        else
        {
            PauseGame ( );
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive ( true );
        //thePlayer.canMove = false;
        //levelManager.levelMusic.Pause ( );
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive ( false );
        //thePlayer.canMove = true;
        //levelManager.levelMusic.Play ( );
    }

    public void Restart ( )
    {
        //PlayerPrefs.SetInt ( "coinCount", 0 );
        // PlayerPrefs.SetInt ( "currentLives", levelManager.startingLives );
        //PlayerPrefs.SetInt ( "currentLives", gameManager.player.startingLivesValue );
        //gameManager.ResetUIValues ( );
        //gameManager.ResetPlayerParameters ( );
        //gameManager.gameOver = false;
        //gameManager.isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene ( SceneManager.GetActiveScene ( ).name );
    }

    public void QuitToMainMehu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene ( mainMenu );
    }
}
