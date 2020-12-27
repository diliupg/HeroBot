using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[DefaultExecutionOrder ( -120 )]
public class UIManager : MonoBehaviour
{
    //public Text playerGemCountText;
    //public Image selectionImage;
    //public Text gemCountText;

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if ( instance == null )
            {
                Debug.LogError ( "UI Manager is Null" );
            }

            return instance;
        }
    }

    [HideInInspector]
    Color healthCol;

    PlayerController player;

    public bool FillingLifeForceBar = false;
    public bool criticalHealth = false;
    public bool playerAlive = true;

    public float playerHealth;
    public float fadeTime = 1;

    [HideInInspector]
    public int playerLives = 4;

    public Text livesAmount;

    public Image fillImg;
    public Image blackScreen;
    public GameObject gameOverScreen;
    public GameObject levelComplete;
    public GameObject gameComplete;

    private void Awake ( )
    {
        //If a UIManager exists and this isn't it...
        if ( instance != null && instance != this )
        {
            //...destroy this and exit. There can only be one Game Manager
            Destroy ( gameObject );
            return;
        }

        instance = this;
    }

    void Start ( )
    {
        blackScreen.enabled = true;

        BlackToScreen ( );

        InitValues ( );

        livesAmount.text = ( playerLives ).ToString ( );
    }

    public void InitValues ( ) // set the colour of all Health blocks to green
    {
        GameManager.RegisterUIManager ( this );
        playerHealth = 1f;
        playerLives = 4;
        StartCoroutine ( GradualFill ( ) );
    }

    public void UpdateLives (int newValue )
    {
        playerLives = newValue;
        livesAmount.text = ( playerLives ).ToString ( );
    }

    public void DecreaseHealth ( float value )
    {
        if ( !FillingLifeForceBar )
        {
            playerHealth = value;
            SetColor ( playerHealth );
        }
    }

    public void IncreaseHealth ( float value )
    {
        if ( !FillingLifeForceBar )
        {
            playerHealth = value;
            SetColor ( playerHealth );
        }
    }

    public void SetColor ( float playerHealth )
    {
        healthCol = Color.Lerp ( Color.red, Color.green, playerHealth );

        fillImg.color = healthCol;
        fillImg.fillAmount = playerHealth;
    }

    public void LaunchCoroutine ( )
    {
        StartCoroutine ( GradualFill ( ) );
    }

    public IEnumerator GradualFill ( )
    {
        FillingLifeForceBar = true;
        healthCol = Color.green;
        fillImg.color = healthCol;

        for ( float i = 0 ; i <= 1f ; i += 0.02f )
        {
            yield return new WaitForSeconds ( 0.01f );
            if ( i > 1f )
                i = 1f;

            fillImg.fillAmount = i;
        }

        FillingLifeForceBar = false;
        playerHealth = 1f;
        SetColor ( playerHealth );
    }

    public void ScreenToBlack ( )
    {
        blackScreen.CrossFadeAlpha ( 1f, 1.2f, false );
    }

    public void BlackToScreen ( )
    {
        blackScreen.CrossFadeAlpha ( 0f, fadeTime, false );
    }

    public void GameOverScreenOn()
    {
        gameOverScreen.SetActive ( true );
    }

    public void GameOverScreenOff()
    {
        gameOverScreen.SetActive ( false );
    }

    public void LevelCompleteOn ( )
    {
        levelComplete.SetActive ( true );
    }

    public void LevelCompleteOff ( )
    {
        levelComplete.SetActive ( false );
    }

    public void GameCompleteOn()
    {
        gameComplete.SetActive ( true );
    }

    public void GameCompleteOff()
    {
        gameComplete.SetActive ( false );
    }
}



