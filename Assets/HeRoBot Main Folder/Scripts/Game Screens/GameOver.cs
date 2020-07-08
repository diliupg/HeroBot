using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public string mainMenu;

    //private LevelManager levelManager;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager> ( );
    }


    public void Restart()
    {
        //PlayerPrefs.SetInt ( "coinCount", 0 );
        // PlayerPrefs.SetInt ( "currentLives", levelManager.startingLives );
        //PlayerPrefs.SetInt ( "currentLives", gameManager.player.startingLivesValue );
        //gameManager.ResetUIValues ( );
        //gameManager.ResetPlayerParameters ( );
        //gameManager.gameOver = false;
        //gameManager.isGameOver = false;
        SceneManager.LoadScene ( SceneManager.GetActiveScene ( ).name );
    }

    public void QutiToMainMenu()
    {
        SceneManager.LoadScene ( mainMenu );
    }


}
