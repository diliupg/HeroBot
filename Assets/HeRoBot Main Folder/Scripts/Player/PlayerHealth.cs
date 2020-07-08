using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

[DefaultExecutionOrder ( -130 )]
public class PlayerHealth : MonoBehaviour, IPlayerDamage
{
    public GameObject playerExplode;
    public GameObject player;
    public Light2D bodyGlow;

    public Anima2D.SpriteMeshInstance[] spriteMeshInstance;

    int spikesTilemap;

    public int startingLivesValue = 4;

    [Header ("Player Stats")]
    public Stats stats;

    public bool isAlive = true;
    public bool shield = true;                 // protection is till the shiled is active
    public bool criticalHealth = false;
    public bool criticalHealthWarning = false;
    //public bool completedLevel = false;
    //public bool endLevel = false;
    public bool exploding= false; // true when health runs out and player explodes

    private bool invincible = false;

    public float knockBackCounter;
    public float knockBackForce;
    public float knockBackTime;
    public float invincibilityTime;
    //[HideInInspector]
    public float health = 1f;
    public int level;
    public int weapons = 1;
    public int wealth;

    private float invincibiltyCounter;
    private float reducingAmount = 0.01f; //.005
    private float increasingAmount = 0.0005f;

    public int lives
    {
        get;
        set;
    }

    public Vector3 respawnPosition;

    #region Events for GameManager, AudioManager, PlayerController
    public static event Action<float> OnDecreaseHealth; // create a static event which has global access
    public static event Action<float> OnIncreaseHealth; // create a static event which has global access
    public static event Action<bool> OnIsPlayerAlive;
    public static event Action OnUpdateUI;
    public static event Action OnPlayerHit;
    public static event Action OnExplodeThePlayer;
    public static event Action OnPlayerCanShoot;
    public static event Action OnSetPlayerRespawnPos;
    #endregion

    void Start ( )
    {
        lives = startingLivesValue;
        spikesTilemap = LayerMask.NameToLayer ( "Spikes" );

        player = gameObject;

        respawnPosition = player.transform.position;

        isAlive = true;
        shield = true;

        GameManager.RegisterPlayerHealth ( this );
        GameManager.RegisterPlayerExplosion ( playerExplode );
    }

    void Update ( )
    {
        ReduceInvincibilityCounter ( );

        ReduceKnockbackCounter ( );

        if ( criticalHealth && !criticalHealthWarning )
        {
            StartCoroutine ( CriticalHealthWarning ( ) );
        }

        if ( !criticalHealth && criticalHealthWarning )
        {
            PlayerNormalColour ( );
            criticalHealthWarning = false;
        }

        SetResetCriticalHealth ( );

        HealthIncrease ( );

        IsPlayerHealthOver ( );
        IsPlayerAlive ( );
    }

    public void KnockBack ( )
    {
        knockBackCounter = knockBackTime; // reset the knockback counter to value
        invincibiltyCounter = invincibilityTime;

        invincible = true;
    }

    void ReduceKnockbackCounter ( )
    {
        if ( knockBackCounter > 0 )
        {
            knockBackCounter -= Time.deltaTime;
        }
    }

    void ReduceInvincibilityCounter ( )
    {
        if ( invincibiltyCounter > 0 )
        {
            invincibiltyCounter -= Time.deltaTime;
        }

        else if ( invincibiltyCounter <= 0 ) // ( invincibiltyCounter <= 0 && !reduceLife )
        {
            invincible = false;
        }
    }



    public void Damage ( float val, bool canKnockback ) // from Idamageble interface
    {
        if ( isAlive && shield )
        {
            if ( OnPlayerHit != null )
                OnPlayerHit ( );

            if ( canKnockback )
                KnockBack ( );

            health -= ( val * reducingAmount );

            if(OnDecreaseHealth != null)
            {
                OnDecreaseHealth ( health );
            }
        }
    }

    private void SetResetCriticalHealth ( )
    {
        if ( health < 0.4f && health > 0 )
            criticalHealth = true;
        if ( health >= 0.4f )
            criticalHealth = false;
    }

    private void HealthIncrease ( )
    {
        if ( isAlive && shield )
        {
            if ( health < 1 )
                health += increasingAmount;

            if ( health >= 1 )
            {
                health = 1;
            }

            if ( OnIncreaseHealth != null )
            {
                OnIncreaseHealth ( health );
            }
        }
    }

    void IsPlayerHealthOver ( )
    {
        if ( health <= 0 && shield )
        {
            health = 0; // health over so
            lives -= 1; // reduce lives by one
            shield = false;
            criticalHealth = false;
            criticalHealthWarning = false;
            exploding = true;

            InitiatePlayerDestruction ( );
        }
    }

    public void IsPlayerAlive ( )
    {
        if ( lives <= 0 && isAlive )
        {
            lives = 0;

            isAlive = false;

            if ( OnIsPlayerAlive != null )
                OnIsPlayerAlive ( isAlive );
        }
    }

    IEnumerator CriticalHealthWarning ( )
    {
        PlayerFlashColor ( 10f, 0f, 0f );
        criticalHealthWarning = true;
        yield return new WaitForSeconds ( .1f );

        PlayerNormalColour ( );
        yield return new WaitForSeconds ( .1f );
        criticalHealthWarning = false;
    }

    void PlayerFlashColor ( float x, float y, float z )
    {
        foreach ( Anima2D.SpriteMeshInstance meshInst in spriteMeshInstance )
        {
            meshInst.color = new Color ( x, y, z );
        }
    }

    public void PlayerNormalColour ( )
    {
        foreach ( Anima2D.SpriteMeshInstance meshInst in spriteMeshInstance )
        {
            meshInst.color = new Color ( 1f, 1f, 1f );
        }
    }

    void InitiatePlayerDestruction()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D> ( );
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.AddForce ( Vector2.zero );

        SetRestHeRoBot ( false ); // turn off the player body

        StartCoroutine ( ExplodePlayerAndRespawn ( ) );
    }

    public IEnumerator ExplodePlayerAndRespawn ( )
    {
        GameObject inst = Instantiate ( playerExplode, player.transform.position, player.transform.rotation );

        if ( OnExplodeThePlayer != null )
            OnExplodeThePlayer ( );

        yield return new WaitForSeconds ( 2f );

        Destroy ( inst );

        if ( !isAlive )
            GameManager.SetGameOver ( );

        if ( lives > 0  )
            ResetParameters ( );

        SetRestHeRoBot ( true ); // turn on the player body

        if ( OnUpdateUI != null )
            OnUpdateUI ( );
    }

    void ResetParameters ( )
    {
        health = 1f;
        shield = true;
        criticalHealth = false;
        criticalHealthWarning = false;
        bodyGlow.enabled = true;
        exploding = false;

        PlayerNormalColour ( );

        if ( OnPlayerCanShoot != null )
            OnPlayerCanShoot ( );

        if ( OnSetPlayerRespawnPos != null )
            OnSetPlayerRespawnPos ( );
    }

    private void SetRestHeRoBot(bool setOrReset) // on and off the meshInstance objects
    {
        foreach ( Transform child in player.transform )
        {
            child.transform.gameObject.SetActive ( setOrReset );
        }
    }

}
