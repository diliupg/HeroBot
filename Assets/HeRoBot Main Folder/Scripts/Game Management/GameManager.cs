using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// This script is a Manager that controls the the flow and control of the game. It keeps
// track of player data (orb count, death count, total game time) and interfaces with
// the UI Manager. All game commands are issued through the static methods of this class

[DefaultExecutionOrder ( -110 )]
public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public SoundEffect soundEffects;

    [HideInInspector]
    public PlayerController player;

    private float timer;

    [SerializeField]
    private GameObject playerObj;

    [SerializeField]
    private PlayerHealth playerHealth;

    private UIManager uiManager;
    

    GameObject playerExplosion;

    CameraManager cameraManager;

    AudioManager audioManager;

    public static int scenesInBuild;

    //public static int NewLevelNumber = 1; // this is the level number when we exit or save the game

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if ( instance == null )
            {
                Debug.LogError ( "Game Manager missing" );
            }
            return instance;
        }
    }

    public bool isGameOver = false;
    public bool levelComplete = false;
    public bool playerWon = false;
    public bool isPlayerAlive = true;

    // If this is the first run of the game there will be no save file. So we have to first
    //check if the file exsists and then if not, create a new file and save the defaults to it.
    //If file exsists it means this is NOT the first run. So wen can load the data from the file.

    public bool fileExsists;

    private ResetOnRespawn [] objectsToReset;

    public Stats stats;
   
    public int lives = 4;
    public float health = 1;
    public int wealth = 0;
    public int weapons = 1;
    public int level = 1;

    #region The  Game Events







    #endregion

    private void OnEnable ( )
    {
        // subscribe to the event Action -> response to the event(delegaate)
        LevelExit.PlayerLevelComplete += OnPlayerLevelComplete;
        PlayerHealth.OnDecreaseHealth += DecreaseHealth;
        PlayerHealth.OnIncreaseHealth += IncreaseHealth;
        PlayerHealth.OnIsPlayerAlive += SetIsPlayerAlive;
        PlayerHealth.OnUpdateUI += UpdateUI;
    }

    private void OnDisable ( )
    {
        // unsubscribe to the events
        LevelExit.PlayerLevelComplete -= OnPlayerLevelComplete;
        PlayerHealth.OnDecreaseHealth -= DecreaseHealth;
        PlayerHealth.OnIncreaseHealth -= IncreaseHealth;
        PlayerHealth.OnIsPlayerAlive -= SetIsPlayerAlive;
        PlayerHealth.OnUpdateUI -= UpdateUI;
    }

    public void Awake ( )
    {
        //If a Game Manager exists and this isn't it...
        if ( instance != null && instance != this )
        {
            //...destroy this and exit. There can only be one Game Manager
            Destroy ( gameObject );
            return;
        }

        //Set this as the current game manager
        instance = this;

        //Persit this object between scene reloads
        //DontDestroyOnLoad ( gameObject );

        soundEffects = GetComponent<SoundEffect> ( );

        //playerObj = GameObject.FindGameObjectWithTag ( "Player" );

        LoadGameData ( );

        if ( fileExsists ) // this is not the first run of the game so load the stats
        {
            GetStats ( );
            //print ( "Not the first time" );
        }
        else
        {
            InitStats ( );      // this is the first time the game is being played so initialize the stats and..
            SaveGameData ( );   // ..save the initial data
            //print ( "first run" );
        }
    }

    private void Start ( )
    {
        Application.targetFrameRate = 100;

        uiManager = UIManager.Instance;
        audioManager = AudioManager.Instance;

        uiManager.GameOverScreenOff ( );
        uiManager.LevelCompleteOff ( );
        uiManager.GameCompleteOff ( );

        uiManager.playerLives = lives;
        uiManager.playerHealth = health;
        //print ( health );

        playerHealth.health = health;

        scenesInBuild = SceneManager.sceneCountInBuildSettings;
        //print ( "Number of levels: " + scenesInBuild );
    }

    private void Update ( )
    {
        if ( !isPlayerAlive && isGameOver )
        {
            StartCoroutine ( WaitAndGameOver ( ) );
        }
    }

    public static int CurrentLevel()
    {
        return SceneManager.GetActiveScene ( ).buildIndex;
    }

    public static bool IsGameOver ( )
    {
        //If there is no current Game Manager, return false
        if ( instance == null )
            return false;

        //Return the state of the game
        return instance.isGameOver;
    }

    public static void SetGameOver ( )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;

        //set the flag game is over
        instance.isGameOver = true;
    }

    void SetIsPlayerAlive(bool state)
    {
        isPlayerAlive = state;
    }
    public IEnumerator WaitAndGameOver ( )
    {
        uiManager.ScreenToBlack ( );

        //dataManager.Save ( playerHealth.stats );

        yield return new WaitForSeconds ( 2f );
        uiManager.InitValues ( );
        isGameOver = false;

        uiManager.GameOverScreenOn ( );
    }

    public void OnPlayerLevelComplete()
    {
        if (!levelComplete)
        {
            levelComplete = true;
            StartCoroutine ( ShowLevelCompleteScreen ( ) );
        }

    }

    public static bool LevelComplete ( )
    {
        //If there is no current Game Manager, return false
        if ( instance == null )
            return false;

        //Return the state of the game
        return instance.levelComplete;
    }

    public IEnumerator ShowLevelCompleteScreen ( )
    {
        uiManager.LevelCompleteOn ( );
        audioManager.PlaySFX ( audioManager.portalSound );
        yield return new WaitForSeconds ( 1.5f );
        audioManager.PlaySFX ( audioManager.LevelCompleteMusic );
        yield return new WaitForSeconds ( 3f );

        //playerHealth.completedLevel = false;
        instance.levelComplete = false;

        // get the current scene number
        int currentSceneNo = SceneManager.GetActiveScene ( ).buildIndex;

        // go to the next scene only if current scene is not the last scene. If it is then GAME OVER.
        if ( currentSceneNo < scenesInBuild )
        {
            level += 1;
            SetStats ( );
            SaveGameData ( );

            uiManager.blackScreen.enabled = true;
            SceneManager.LoadScene ( level );

            LoadGameData ( );

            GetStats ( );

            uiManager.LevelCompleteOff ( );
            uiManager.BlackToScreen ( );
        }
        else
        // if player alive and last scene completed then the PLAYER WON. Display the PLAYER WON screen.
        {
            uiManager.LevelCompleteOff ( );
            uiManager.GameCompleteOn ( );
        }
    }

    public void UpdateUI()
    {
        uiManager.UpdateLives ( instance.playerHealth.lives );
    }

    public void ResetUIValues ( )
    {
        uiManager.InitValues ( );
    }

    public void IncreaseHealth(float health)
    {
        uiManager.IncreaseHealth ( health );
    }

    public void DecreaseHealth(float health)
    {
        uiManager.DecreaseHealth ( health );
    }

    public static void RegisterPlayer ( PlayerController player )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;

        //Record the player reference
        instance.player = player;
        instance.playerObj = player.gameObject;
    }

    public static void RegisterPlayerHealth ( PlayerHealth playerHealth )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;

        //Record the playerHealth reference
        instance.playerHealth = playerHealth;
    }

    public static void RegisterPlayerExplosion ( GameObject explosion )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;

        //Record the player Explosion reference
        instance.playerExplosion = explosion;
    }

    public static void RegisterUIManager ( UIManager uimanager )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;

        //Record the uiManager reference
        instance.uiManager = uimanager;
    }

    public static void RegisterLevelExit ( LevelExit levelExit )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;
    }

    public static void RegisterCameraManager ( CameraManager cameraManager )
    {
        //If there is no current Game Manager, exit
        if ( instance == null )
            return;
        //print ( "CameraManager registered" );
    }

    public void SaveGameData ( )
    {
        // create a file or open a file to save to
        FileStream file = new FileStream( Application.persistentDataPath + "/wookie.dat", FileMode.OpenOrCreate);

        try
        {
            // Binary formatter -- allows us to write data to a file
            BinaryFormatter formatter = new BinaryFormatter();
            // serialization method to write to the file
            formatter.Serialize ( file, stats );
        }
        catch ( SerializationException e )
        {
            Debug.LogError ( "There was an issue serialzing this data " + e.Message );
        }
        finally
        {
            file.Close ( );
            //Debug.Log ( "SAVED"+ stats.lives+" "+ stats.level);

        }
    }

    public void LoadGameData ( )
    {
        if ( File.Exists ( Application.persistentDataPath + "/wookie.dat" ) )
        {
            fileExsists = true;
            //Debug.Log ( "FILE EXISTS!" );
            FileStream file = new FileStream( Application.persistentDataPath + "/wookie.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                stats = formatter.Deserialize ( file ) as Stats;
            }
            catch ( SerializationException e )
            {
                Debug.LogError ( "There was an issue Deserialzing this data " + e.Message );
            }
            finally
            {
                file.Close ( );
                //Debug.Log ( "LOADED" );
            }
        }
        else
        {
            fileExsists = false;
            Debug.LogError ( "No save data!" );
        }

    }

    public void InitStats ( ) // first time the game is played
    {
        stats.health = health;
        stats.lives = lives;
        stats.wealth = wealth;
        stats.weapons = weapons;
        stats.level = level;

        //stats.myPos.x = gameObject.transform.position.x;
        //stats.myPos.y = gameObject.transform.position.y;
        //stats.myPos.z = gameObject.transform.position.z;
    }

    public void GetStats ( ) // everytime the game is played except the first time
    {
        health = stats.health;
        lives = stats.lives;
        wealth = stats.wealth;
        weapons = stats.weapons;
        level = stats.level;

        //gameObject.transform.position = stats.myPos.GetPos ( );
    }

    public void SetStats ( ) // when saving
    {
        stats.health = health;
        stats.lives = lives;
        stats.wealth = wealth;
        stats.weapons = weapons;
        stats.level = level;

        //stats.myPos.x = gameObject.transform.position.x;
        //stats.myPos.y = gameObject.transform.position.y;
        //stats.myPos.z = gameObject.transform.position.z;
    }

    public void AddCoins ( int coinsToAdd )
    {
        //coinCount += coinsToAdd;
        //_coinBonusLifeCount += coinsToAdd;

        //cointext.text = "Coins: " + coinCount.ToString ( "0000" );
        //coinSound.Play ( );
    }

}
