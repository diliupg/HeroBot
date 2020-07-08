using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private int firstLevel = 1;

    private int continueLevel = 1;
    private GameManager gameManager;
    public Button button;
    //public string [] levelNames;

    //public int startingLives;

    void Start()
    {
        gameManager = GameManager.Instance;

        continueLevel = gameManager.stats.level;

        if( continueLevel == firstLevel)
        {
            button.interactable = false;
        }
        //print ( SceneManager.GetActiveScene ( ).buildIndex );
    }

    public void NewGame()
    {
        SceneManager.LoadScene ( firstLevel );


    }

    public void Continue ( )
    {

        SceneManager.LoadScene ( continueLevel );
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be
        // set to false to end the game

        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit ( );
    }

}
