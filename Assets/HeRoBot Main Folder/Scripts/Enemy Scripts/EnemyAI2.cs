using System.Collections;
using System;
using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    [SerializeField] protected GameObject projectile;

    [SerializeField] protected Transform left;
    [SerializeField] protected Transform right;

    [SerializeField] protected float speed;
    [SerializeField] protected float retreatSpeed;
    [SerializeField] protected float retreatDistance;
    [SerializeField] protected float stoppingDistance;
    [SerializeField] protected float playerSightedDistance;
    [SerializeField] protected float startTimeBetweenShots;
    [SerializeField] protected float addForce;
    [SerializeField] protected float jumpDelay;

    [SerializeField] protected int health;
    [SerializeField] protected int damagePlayer;

    [SerializeField] protected bool walkEnable;
    [SerializeField] protected bool jumpEnable;
    [SerializeField] protected bool patrolEnable;

    public static event Action<int> setDamage;
    public int Health
    {
        get;
        set;
    }

    private float directionLR;
    private float xVel;
    private float timeBetweenShots;

    private Vector2 currentTarget; // either left or right

    protected GameObject player;
    protected Transform shootPoint;
    protected Rigidbody2D rb;
    protected PlayerHealth playerHealth;
    protected Animator animator;
    protected AudioManager audioManager;
    protected AudioSource audioSource;
    protected ObjectPooler objectPooler;
    protected BoxCollider2D boxCollider2D;

    protected bool isGrounded;
    protected bool isAlive;
    protected bool playerOnLeft;
    protected bool isPlayerSighted;
    protected bool stay;
    protected bool delayTrigger;

    protected Vector3 scale;

    public virtual void Init ( )
    {
        player = GameObject.FindGameObjectWithTag ( "Player" );

        animator = GetComponentInChildren<Animator> ( );
        rb = GetComponent<Rigidbody2D> ( );
        boxCollider2D = GetComponent<BoxCollider2D> ( );
        playerHealth = player.GetComponent<PlayerHealth> ( );
        audioSource = GetComponent<AudioSource> ( );
        objectPooler = ObjectPooler.Instance;
        audioManager = AudioManager.Instance;

        timeBetweenShots = startTimeBetweenShots;
        scale = transform.localScale;
        directionLR = 1f; //normal is facing left
        shootPoint = gameObject.transform.GetChild ( 0 );

        Health = 8;

        isAlive = true;
    }

    private void OnEnable ( )
    {
        ShootTrigger.fire += LaunchActualProjectile;
    }

    private void OnDisable ( )
    {
        ShootTrigger.fire -= LaunchActualProjectile;
    }

    void Start ( )
    {
        Init ( );
        StartCoroutine ( DelayJumping ( ) );
    }

    public virtual void Update ( )
    {
        if ( isAlive && playerHealth.shield )
        {
            FacePlayer ( );

            IsPlayerSighted ( );

            if ( Vector2.Distance ( transform.position, player.transform.position ) < playerSightedDistance )
                Attack ( );

            Death ( );
        }

        if ( setDamage != null )
        {
            setDamage ( damagePlayer );
        }
    }

    public virtual void FixedUpdate ( )
    {

        if ( isPlayerSighted && isAlive )
        {
            MovementMethod ( );
        }
    }

    public void Damage ( )
    {
        if ( isAlive )
        {
            animator.SetTrigger ( "Hit" );

            audioManager.EnemActivitySound ( audioSource, audioManager.GiantHit );

            Health -= 1;
        }
    }


    void FacePlayer ( )
    {
        float dist = transform.localPosition.x -  player.transform.localPosition.x;

        if ( dist > 0 )
        {
            playerOnLeft = true;
            directionLR = 1;
        }
        else if ( dist < 0 )
        {
            playerOnLeft = false;
            directionLR = -1;
        }

        transform.localScale = new Vector3 ( directionLR * scale.x, 1f * scale.y, 1f * scale.z );
    }

    void IsPlayerSighted ( )
    {
        if ( Vector2.Distance ( transform.position, player.transform.position ) < playerSightedDistance )
        {
            isPlayerSighted = true;
        }
        else
        {
            animator.SetBool ( "Walk", false );
            isPlayerSighted = false;
        }
    }

    void MovementMethod ( )
    {
        if ( walkEnable && isGrounded )
        {
            Walk ( );
        }

        if ( jumpEnable && isGrounded )
        {
            jumpEnable = false;
            Jump ( );
        }
    }
    void Walk ( )
    {
        xVel += addForce * -directionLR;
        if ( xVel > speed )
            xVel = speed;
        else if ( xVel < -speed )
            xVel = -speed;

        if ( Vector2.Distance ( transform.position, player.transform.position ) > stoppingDistance )
        {
            animator.SetBool ( "Walk", true );
            rb.velocity = new Vector2 ( xVel, rb.velocity.y ); // move towards player
        }

        else if ( Vector2.Distance ( transform.position, player.transform.position ) < stoppingDistance && ( Vector2.Distance ( transform.position, player.transform.position ) > retreatDistance ) )
        {
            animator.SetBool ( "Walk", false );
            rb.velocity = new Vector2 ( 0f, rb.velocity.y ); // stay
        }
        else if ( ( Vector2.Distance ( transform.position, player.transform.position ) < retreatDistance ) )
        {
            animator.SetBool ( "Walk", true );
            rb.velocity = new Vector2 ( -xVel, rb.velocity.y ); // move away from player
        }
    }
    public virtual void Jump ( )
    {
        animator.SetBool ( "Walk", false );
        rb.AddForce ( new Vector2 ( 0f, addForce ), ForceMode2D.Impulse );
    }

    void Attack ( )
    {
        if ( timeBetweenShots <= 0 )
        {

            animator.SetTrigger ( "Attack" );



            timeBetweenShots = startTimeBetweenShots;
        }
        else
            timeBetweenShots -= Time.deltaTime;
    }

    void LaunchActualProjectile ( )
    {
        GameObject obj = objectPooler.SpawnFromPool(projectile.name, shootPoint.position);

        if ( playerOnLeft )
            obj.transform.Rotate ( 0f, 180f, 0f );

        audioManager.EnemActivitySound ( audioSource, audioManager.GlobShoot );
    }

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if ( collision.transform.CompareTag ( "Ground" ) || collision.transform.CompareTag ( "Spikes" ) )
        {
            isGrounded = true;
            // now on ground so start the jump delay
            if ( !jumpEnable && !delayTrigger )
            {
                delayTrigger = true;
                StartCoroutine ( DelayJumping ( ) );
            }
        }
    }

    IEnumerator DelayJumping ( )
    {
        yield return new WaitForSeconds ( jumpDelay );

        delayTrigger = false;
        jumpEnable = true;
    }

    private void OnCollisionExit2D ( Collision2D collision )
    {
        if ( collision.transform.CompareTag ( "Ground" ) || collision.transform.CompareTag ( "Spikes" ) )
        {
            isGrounded = false;
        }
    }

    void Death ( )
    {
        if ( Health <= 0 && isAlive )
        {
            isAlive = false;
            StartCoroutine ( DieAndEnd ( ) );
        }
    }

    IEnumerator DieAndEnd ( )
    {
        animator.SetTrigger ( "Die" );
        audioManager.EnemyDeath ( audioManager.GlobDie );
        yield return new WaitForSeconds ( 3f );
        gameObject.SetActive ( false );
    }
}
