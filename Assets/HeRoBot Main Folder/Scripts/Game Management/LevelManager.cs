using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject deathplosion;
    public GameObject playerExp;
    private GameObject inst;
    public GameObject gameOverScreen;
    public AudioSource coinSound;
    public AudioSource levelMusic;
    public AudioSource gameOverMusic;

    public Image heart1, heart2, heart3, heart4, heart5;
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;


    public int maxHealth;
    public int healthCount;

    public int startingLives;
    public int currentLives;
    public float waitToRespawn;

    public Text livestext;
    public Text cointext;

    private bool _respawn;
    public bool invincible;
    public bool _respawnCoActive;

    private ResetOnRespawn [] objectsToReset;

    public int coinCount;
    private int _coinBonusLifeCount;
    public int bonusLifeThreshold;


    //void Start ( )
    //{
    //    gameOverScreen.SetActive ( false );

    //    playerController = FindObjectOfType<PlayerController> ( );

    //    healthCount = maxHealth;

    //    objectsToReset = FindObjectsOfType<ResetOnRespawn> ( );

    //    if(PlayerPrefs.HasKey("coinCount") )
    //    {
    //        coinCount = PlayerPrefs.GetInt ( "coinCount" );
    //    }

    //    cointext.text = "Coins: " + coinCount.ToString ( "0000" );

    //    if ( PlayerPrefs.HasKey ( "currentLives" ) )
    //    {
    //        currentLives  = PlayerPrefs.GetInt ( "currentLives" );
    //    }
    //    else
    //    {
    //        currentLives = startingLives;
    //    }

    //    livestext.text = "Lives X " + currentLives;
    //}

    //void Update()
    //{
    //    //if(healthCount <= 0 )
    //    //{
    //    //    Respawn ( );
    //    //}

    //    // bonusLifeThreshold is set in the inspector, If our coinBonusLifeCOunt count goes over that, we get an extra life and the coinBonusLifeCount is reduced by the bonusLifeThreshold amount
    //    if ( _coinBonusLifeCount >= bonusLifeThreshold )
    //    {
    //        currentLives += 1;

    //        livestext.text = "Lives X " + currentLives;
    //        _coinBonusLifeCount -= bonusLifeThreshold;
    //    }
    //}

    //public void Respawn()
    //{
    //    Debug.Log ( "level manager Respawn" );
    //    if ( !_respawn )
    //    {
    //        currentLives -= 1;
    //        livestext.text = "Lives X " + currentLives;

    //        if ( currentLives > 0 )
    //        {
    //            _respawn = true;
    //            StartCoroutine ( RespawnPlayer ( ) );
    //        }
    //        else
    //        {
    //            playerController.gameObject.SetActive ( false ); // game OVER
    //            gameOverScreen.SetActive ( true );
    //            levelMusic.Stop ( );
    //            gameOverMusic.Play ( );
    //        }
    //    }
    //}

    //public IEnumerator RespawnPlayer()
    //{
    //    Debug.Log ( "level manager RespawnPlayer" );
    //    _respawnCoActive = true;

    //    playerController.gameObject.SetActive ( false );

    //    //Instantiate ( deathplosion, playerController.transform.position, playerController.transform.rotation );
    //    inst = Instantiate ( playerExp, playerController.transform.position, playerController.transform.rotation );

    //    yield return new WaitForSeconds ( waitToRespawn );

    //    Destroy ( inst, 2f );

    //    _respawnCoActive = false;

    //    healthCount = maxHealth;
    //    UpdateHealthDisplay ( );
    //    _respawn = false;
    //    coinCount = 0;
    //    _coinBonusLifeCount = 0;

    //    cointext.text = "Coins: " + coinCount.ToString ( "0000" );

    //    playerController.transform.position = playerController.respawnPosition;
    //    playerController.gameObject.SetActive ( true );

    //    for( int i = 0; i < objectsToReset.Length; i++ )
    //    {

    //        objectsToReset [ i ].gameObject.SetActive ( true );
    //        objectsToReset [ i ].ResetObject ( );
    //    }
    //}

    //public void AddCoins(int coinsToAdd)
    //{
    //    coinCount += coinsToAdd;
    //    _coinBonusLifeCount += coinsToAdd;

    //    cointext.text = "Coins: " + coinCount.ToString("0000");
    //    coinSound.Play ( );
    //}

    //public void PlayerDamage(int damageAmount)
    //{
    //    Debug.Log ( "level manager PlayerDamage" );
    //    if ( !invincible )
    //    {
    //        healthCount -= damageAmount;
    //        UpdateHealthDisplay ( );
    //        playerController.KnockBack ( );
    //        playerController.hurtSound.Play ( );
    //    }
    //}

    //public void UpdateHealthDisplay()
    //{
    //    switch(healthCount)
    //    {
    //        case 10:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartFull;
    //            heart4.sprite = heartFull;
    //            heart5.sprite = heartFull;
    //            return;
    //        case 9:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartFull;
    //            heart4.sprite = heartFull;
    //            heart5.sprite = heartHalf;
    //            return;
    //        case 8:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartFull;
    //            heart4.sprite = heartFull;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 7:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartFull;
    //            heart4.sprite = heartHalf;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 6:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartFull;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 5:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartHalf;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 4:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartFull;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 3:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartHalf;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 2:
    //            heart1.sprite = heartFull;
    //            heart2.sprite = heartEmpty;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 1:
    //            heart1.sprite = heartHalf;
    //            heart2.sprite = heartEmpty;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        case 0:
    //            heart1.sprite = heartEmpty;
    //            heart2.sprite = heartEmpty;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;
    //        default:
    //            heart1.sprite = heartEmpty;
    //            heart2.sprite = heartEmpty;
    //            heart3.sprite = heartEmpty;
    //            heart4.sprite = heartEmpty;
    //            heart5.sprite = heartEmpty;
    //            return;

    //    }
    //}

    public void AddLIves ( int livesToAdd )
    {
        Debug.Log ( "LivesToAdd" );
        currentLives += livesToAdd;
        livestext.text = "Lives X " + currentLives;
    }

    public void GiveHealth ( int healthToGive )
    {
        Debug.Log ( "level manager GiveHealth" );
        healthCount += healthToGive;

        if ( healthCount > maxHealth )
            healthCount = maxHealth;

        //UpdateHealthDisplay ( );
    }


}
