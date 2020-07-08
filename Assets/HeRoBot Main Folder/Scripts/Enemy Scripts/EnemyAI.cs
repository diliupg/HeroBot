using System.Collections;
using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageble
{
    [SerializeField] protected GameObject projectile;

    [SerializeField] protected Transform left;
    [SerializeField] protected Transform right;

    [SerializeField] protected float speed;
    //[SerializeField] protected float retreatSpeed;
    //[SerializeField] protected float retreatDistance;
    [SerializeField] protected float stoppingDistance;
    [SerializeField] protected float playerSightedDistance;
    [SerializeField] protected float startTimeBetweenShots;
    [SerializeField] protected float addForce;
    [SerializeField] protected float jumpDelay;

    [SerializeField] protected int health;
    [SerializeField] protected int damagePlayer;

    [SerializeField] protected bool walkEnabled;
    [SerializeField] protected bool jumpEnabled;
    [SerializeField] protected bool patrolEnabled;

    public static event Action<int> setDamage;
    public int Health
    {
        get;
        set;
    }
    [SerializeField]
    private float directionLR;
    [SerializeField]
    private float timeBetweenShots;
    [SerializeField]
    private Transform moveToTarget; // either left or right

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
    protected bool targetOnLeft;
    protected bool playerInAttackRange;
    protected bool stay;
    protected bool delayTrigger;
    protected bool attacking;
    protected bool moveToBase;
    protected bool canJump;
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
        shootPoint = gameObject.transform.GetChild ( 0 );

        moveToTarget = left;
        directionLR = -1f; //normal is facing left

        Health = 8;

        isAlive = true;

        if(jumpEnabled)
        {
            canJump = true;
        }

        animator.SetBool ( "Idle", true ); // start by setting the Idle animation
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
    }

    public virtual void Update ( )
    {
        if ( isAlive && playerHealth.shield )
        {
            if ( patrolEnabled && !attacking && !moveToBase ) // do the normal patrolling
            {
                animator.SetBool ( "Walk", true );

                Patrol ( );
            }

            IsPlayerSighted ( );

            if ( !attacking && playerInAttackRange ) // attack the player
            {
                attacking = true;
                moveToBase = false;
                FaceTarget ( player.transform );
            }

            else if ( attacking && !playerInAttackRange ) // end attack
            {
                attacking = false;

                if( (transform.position.x < left.position.x || transform.position.x > right.position.x) )
                {
                    moveToBase = true; // enemy is out of patrol range so move to base
                }

                FaceTarget ( left );
            }

            if (moveToBase) // move straight to first base
            {
                Move ( );

                if ( transform.position.x < left.position.x && moveToBase && directionLR == -1 )
                {
                    moveToBase = false;
                }
                else if ( transform.position.x > left.position.x && moveToBase && directionLR == 1 )
                {
                    moveToBase = false;
                }
            }

            if ( attacking )
            {
                Attack ( );
            }

            if (attacking && canJump && timeBetweenShots > 0)
            {
                Jump ( );
            }

            Death ( );
        }

        if ( setDamage != null )
        {
            setDamage ( damagePlayer );
        }
    }

    void Patrol()
    {
        if ( transform.position.x <= left.position.x )
        {
            moveToTarget = right;
            directionLR = 1;
            SetPatrol ( );
        }

        if ( transform.position.x >= right.position.x )
        {
            moveToTarget = left;
            directionLR = -1;
            SetPatrol ( );
        }

        Move ( );
    }

    void Move()
    {
        transform.Translate ( transform.right * directionLR * speed * Time.deltaTime );
    }

    void SetPatrol()
    {
        StartCoroutine ( PausePatrol ( ) );
    }

    IEnumerator PausePatrol()
    {
        animator.SetBool ( "Walk", false );
        patrolEnabled = false;

        yield return new WaitForSeconds ( 2f );

        FacingDirection ( directionLR ); // turn

        animator.SetBool ( "Walk", true );
        patrolEnabled = true;
    }

    void IsPlayerSighted ( )
    {
        if ( Vector2.Distance ( transform.position, player.transform.position ) < playerSightedDistance )
        {
            playerInAttackRange = true;
        }
        else
        {
            playerInAttackRange = false;
        }
    }

    void FaceTarget ( Transform targetTransform )
    {

        if ( targetTransform.position.x < transform.position.x )
        {
            targetOnLeft = true;
            directionLR = -1;
        }
        else if ( targetTransform.position.x > transform.position.x )
        {
            targetOnLeft = false;
            directionLR = 1;
        }

        FacingDirection ( directionLR );
    }

    void FacingDirection ( float val )
    {
        transform.localScale = new Vector2 ( -val * scale.x, 1f * scale.y );
    }

    void Attack ( )
    {
        AttactMovement ( );

        if ( timeBetweenShots <= 0 )
        {
            animator.SetTrigger ( "Attack" );

            timeBetweenShots = startTimeBetweenShots;
        }
        else
            timeBetweenShots -= Time.deltaTime;
    }

    public void AttactMovement ( )
    {
        FaceTarget ( player.transform );

        Vector3 vec;
        if ( Vector2.Distance ( transform.position, player.transform.position ) <= stoppingDistance )
        {
            vec = Vector2.zero;
            animator.SetBool ( "Walk", false );
        }
        else
        {
            vec = transform.right * directionLR * speed * Time.deltaTime;
            animator.SetBool ( "Walk", true );
        }

        transform.Translate ( vec );
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

    public virtual void Jump ( )
    {
        animator.SetBool ( "Walk", false );
        rb.AddForce ( new Vector2 ( (addForce * directionLR ) /2, addForce ), ForceMode2D.Impulse );
        canJump = false;
        StartCoroutine ( DelayJumping ( ) );
    }

    IEnumerator DelayJumping ( )
    {
        yield return new WaitForSeconds ( jumpDelay );
        canJump = true;
    }

    void LaunchActualProjectile ()
    {
        GameObject obj = objectPooler.SpawnFromPool(projectile.name, shootPoint.position);

        if ( targetOnLeft )
            obj.transform.Rotate ( 0f, 180f, 0f );

        audioManager.EnemActivitySound ( audioSource, audioManager.GlobShoot );
    }

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if ( collision.transform.CompareTag ( "Ground" ) || collision.transform.CompareTag ( "Spikes" ) )
        {
            isGrounded = true;
        }
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
