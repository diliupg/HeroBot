using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

[DefaultExecutionOrder ( -118 )]
public class PlayerController : MonoBehaviour //IPlayerDamage
{
    #region The Game Objects
    public bool drawDebugRaycasts = true; //Should the environment checks be visualized

    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public BoxCollider2D bodyCollider;

    [SerializeField] private ParticleSystem gunFlash;
    [SerializeField] private ParticleSystem dustOnLand;
    [SerializeField] private ParticleSystem runDust;

    [SerializeField] private MovementJoystick joystick;
    [SerializeField] private JumpButton jumpBut;
    [SerializeField] private CrouchButton crouchBut;
    [SerializeField] private ShootButton fireBut;
    [SerializeField] private PauseButton pauseBut;
    [SerializeField] private TorchButton torchBut;
    [SerializeField] private SpeedButton speedBut;

    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool lazerspriteFlipped;

    private PlayerHealth playerHealth;
    private PlayerAnimations playerAnimations;
    private PlayerRaycast playerRaycast;

    private InputManager input;          //The current inputs for the player
    private Transform gun;
    private ObjectPooler objectPooler;

    public GameObject stompBox;

    public Light2D torch;


    #endregion

    public LayerMask whatIsGround;
    public Vector3 respawnPosition;

    #region Floats
    [Header("Misalaneous Properties")]
    public float knockBackForce;
    public float groundCheckRadius;
    public float onPlatSpeedModifier;
    public float FireLazerWaitTime;

    private float coyoteTime;

    [Header("Jump Properties")]
    public float jumpForce = 8.5f;          //Initial force of jump
    public float crouchJumpBoost = 2.5f;    //Jump boost when crouching
    public float hangingJumpForce = 15f;    //Force of wall hanging jumo
    public float jumpHoldForce = 1.9f;      //Incremental force when jump is held
    public float jumpHoldDuration = .1f;    //How long the jump key can be held
    public float jumpTime = 0.4f;

    [Header("Movement Properties")]
    public float speed = 6.5f;              //Player speed
    public float crouchSpeedDivisor = 3f;   //Speed reduction when crouching
    public float coyoteDuration = .05f;     //How long the player can jump after falling
    public float maxFallSpeed = -25f;		//Max speed player can fall
    public float dashSpeed = 25f;
    public float dashTimerWait = 8f;
    public float dashTime = 0.3f;
    [HideInInspector]
    public float directionLR;               // add by me. left is -1 right is +1
    [HideInInspector]
    public float directionUD;

    private float lastMoveDirection;

    #endregion

    #region Bools
    [Header ("Status Flags")]
    public bool isOnGround;                  // decides if player is on the ground, spikes, movingPlatforms
    public bool canMove;                    // decides if player is alive
    public bool canDash;
    public bool canShoot;
    public bool isJumping;
    public bool isCrouching;
    public bool isHeadBlocked;
    private bool onMovingPlat;
    public bool playerInWater;
    public bool MoveinWater;
    //public bool invincible;                 // when on player stays invincible till turned off

    private bool torchOnOff;

    [HideInInspector]
    public bool reduceLife;
    #endregion

    #region Ints
    public int playerFacingRignt;            // direction player is facing used in playerRaycast.cs

    //private int damageVal;
    //private int length;
    //private int speedParamID;                // ID of the speed paramter in the Animator
    //private int isGroundedParamID;           // ID of the ground paramter in the Animator
    //private int fallParamID;                 // ID of the fall paramter in the Animator
    //private int isCrouchingParamID;          // ID of the croucn paramter in the Animator
    //private int iSShootingParamID;           // ID of the shoot paramter in the Animator
    //private int isDashingParamID;            // ID of the jump paramter in the Animator
    //private int isJumpingParamID;            // ID of the isJumping paramter in the Animator
    //private int isHangingParamID;            // ID of the grabWall parameter in the Animator

    private Vector3 scale;
    #endregion

    #region Events
    public static event Action OnSpawnPlayer;
    public static event Action OnLaserFire;
    public static event Action<bool> OnWalkSound1;
    public static event Action<bool> OnWalkSound2;
    public static event Action OnJumpSound;
    public static event Action OnPlayerFallOnGround;
    public static event Action OnPlayerFallOnPlatform;
    public static event Action OnEscapePressed;

    public static event Action<float> UpOrDownPressed;
    public static event Action<float> UpOrDownNotPressed;
    #endregion

    private void OnEnable ( )
    {
        PlayerHealth.OnPlayerCanShoot += SetPlayerCanShoot;
        PlayerHealth.OnSetPlayerRespawnPos += SetRespawnPosition;
        InWater.inTheWater += SetPlayerInWaterOrNot;
    }

    private void OnDisable ( )
    {
        PlayerHealth.OnPlayerCanShoot -= SetPlayerCanShoot;
        PlayerHealth.OnSetPlayerRespawnPos -= SetRespawnPosition;
        InWater.inTheWater -= SetPlayerInWaterOrNot;
    }

    void Start ( )
    {
        objectPooler = ObjectPooler.Instance;
        playerHealth = GetComponent<PlayerHealth> ( );
        playerRb = GetComponent<Rigidbody2D> ( );
        playerAnimations = GetComponent<PlayerAnimations> ( );
        playerRaycast = GetComponent<PlayerRaycast> ( );
        bodyCollider = GetComponent<BoxCollider2D> ( );

        //input = GetComponent<InputManager> ( );
        input = GetComponent<InputManager> ( );

        respawnPosition = transform.position;

        lastMoveDirection = 1; // start with the direction as Right

        #region Set bool values
        canMove = true;
        canDash = false;
        isDashing = false;
        lazerspriteFlipped = false;
        canShoot = true;
        torch.enabled = false;
        #endregion

        scale = transform.localScale;

        GameManager.RegisterPlayer ( this );

        if ( OnSpawnPlayer != null )
            OnSpawnPlayer ( );
    }

    void Update ( )
    {
        if ( playerHealth.isAlive || !playerHealth.exploding )
        {
            TorchOnOff ( );
        }

        if ( playerHealth.exploding )
            canMove = false;
        else
            canMove = true;

        if ( pauseBut.pausePressed )
        {
            EscapePressed ( );
            Debug.Log ( "PausePressed" );
        }
           

        directionUD = joystick.directionUD;

        if ( playerInWater )
        {
            UpOrDownPressed ( directionUD );
        }

        if ( UpOrDownPressed != null && !playerInWater )
        {
            UpOrDownPressed ( directionUD );
        }


    }

    void FixedUpdate ( )
    {
        if ( playerHealth.knockBackCounter <= 0 && canMove && playerHealth.isAlive && playerHealth.shield )
        {
            directionLR = joystick.directionLR;

            if ( !playerRaycast.isHanging )
                FlipPlayer ( );

            WaterMovement ( );

            GroundMovement ( );

            AirMovement ( );

            GetGun ( );

            if ( fireBut.shootPressed && !playerRaycast.isHanging && canShoot )
            {
                StartCoroutine ( Fire ( FireLazerWaitTime ) );
            }
        }

        if ( !reduceLife && playerHealth.isAlive && playerRb.bodyType != RigidbodyType2D.Static )
        {
            FlipPlayerKnockbackSide ( );
        }

        StompEm ( );

        if ( directionLR != 0 )
        {
            lastMoveDirection = directionLR;
        }
    }

    void FlipPlayerKnockbackSide ( )
    {
        if ( playerHealth.knockBackCounter > 0 )
        {
            if ( transform.localScale.x > 0 )
            {
                playerRb.velocity = new Vector3 ( -knockBackForce, knockBackForce, 0f );
            }
            else
            {
                playerRb.velocity = new Vector3 ( knockBackForce, knockBackForce, 0f );
            }
        }
    }

    void SetPlayerInWaterOrNot( bool value)
    {
        playerInWater = value;
    }

    void WaterMovement()
    {
        if ( playerInWater )
        {
            MoveinWater = true;

            if ( directionLR != 0 )
            {
                int angle;

                if ( playerFacingRignt == 1 )
                    angle = -90;
                else
                    angle = 90;

                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Lerp ( transform.rotation, q, 4f * Time.deltaTime );

            }
            else
            {
                MoveinWater = false;
                Quaternion q = Quaternion.AngleAxis(0, Vector3.forward);
                transform.rotation = Quaternion.Lerp ( transform.rotation, q, 4f * Time.deltaTime );
            }

            if (directionUD != 0 )
            {
                float yVelocity = ( speed / 2 ) * ( input.directionUD / 0.8f );
                playerRb.velocity = new Vector2 ( playerRb.velocity.x, yVelocity );
            }

        }
        else
        {
            gameObject.transform.rotation = Quaternion.identity;
            MoveinWater = false;
        }


    }

    void GroundMovement ( )
    {
        if ( playerRaycast.isHanging )
            return;

        #region Crouching Stuff
        //Handle crouching input. If holding the crouch button but not crouching, crouch
        if ( crouchBut.crouchHeld && !isCrouching && !isJumping )
            Crouch ( );

        //Otherwise, if not holding crouch but currently crouching, stand up
        else if ( !crouchBut.crouchHeld && isCrouching )
            StandUp ( );

        //Otherwise, if crouching and no longer on the ground, stand up
        else if ( !isOnGround && isCrouching )
            StandUp ( );

        //Calculate the desired velocity based on inputs
        float xVelocity = speed * joystick.directionLR;

        //If the player is crouching, reduce the velocity
        if ( isCrouching )
        {
            xVelocity /= crouchSpeedDivisor;
        }

        #endregion

        #region Dash Stuff
        if ( speedBut.speedPressed && !canDash )
        {
            isDashing = true;
            canDash = true;
        }

        if ( isDashing && isOnGround && canDash )
        {
            canDash = true;
            xVelocity += dashSpeed * lastMoveDirection;
            StartCoroutine ( Dashing ( ) );
        }
        if ( playerHealth.knockBackCounter <= 0 && canMove )
        {
            //Apply the desired velocity
            playerRb.velocity = new Vector2 ( xVelocity, playerRb.velocity.y );
        }
        #endregion

        #region Ground Stuff
        if ( !isOnGround && playerRaycast.isOnGround && !isJumping )
        {
            if ( OnPlayerFallOnGround != null )
                OnPlayerFallOnGround ( );

            isOnGround = true;

            DustOnLand ( );
        }
        else if ( isOnGround && !playerRaycast.isOnGround )
        {
            isOnGround = false;
        }
        #endregion

        #region MovingPlatform Stuff
        if ( !onMovingPlat && playerRaycast.onMovingPlatform )
        {
            if ( OnPlayerFallOnPlatform != null )
                OnPlayerFallOnPlatform ( );

            onMovingPlat = true;
            isOnGround = true;
            transform.parent = playerRaycast.parent;

        }

        else if ( onMovingPlat && !playerRaycast.onMovingPlatform )
        {
            transform.parent = null;
            onMovingPlat = false;
            isOnGround = false;
        }
        #endregion

        //If the player is on the ground, extend the coyote time window
        if ( playerRaycast.isOnGround )
        {
            coyoteTime = Time.time + coyoteDuration;
        }
    }

    void AirMovement ( )
    {
        //If the player is currently hanging...
        if ( playerRaycast.isHanging )
        {
            #region IsHanging Faling
            //If crouch is pressed...
            if ( crouchBut.crouchPressed )
            {
                //...let go...
                playerRaycast.isHanging = false;

                //...set the rigidbody to dynamic and exit
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                return;
            }
            #endregion

            #region isHanging Jumping
            //If jump is pressed...
            if ( jumpBut.jumpPressed )
            {
                //...let go...
                playerRaycast.isHanging = false;

                //isJumping = true; // do we need this line? not in the othjer script

                //...set the rigidbody to dynamic and apply a jump force...
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                playerRb.AddForce ( new Vector2 ( 0f, hangingJumpForce ), ForceMode2D.Impulse );
                //...and exit
                return;
            }
            #endregion
        }

        #region Jump stuff
        //If the jump key is pressed AND the player isn't already jumping AND EITHER
        //the player is on the ground or within the coyote time window...
        if ( jumpBut.jumpPressed && !isJumping && ( (isOnGround || playerInWater) || coyoteTime > Time.time ) )
        {
            transform.rotation = Quaternion.identity; // rotate player to normal in case he is swimming when he jumps

            //...check to see if crouching AND not blocked. If so...
            if ( isCrouching && !playerRaycast.isHeadBlocked )
            {
                //...stand up and apply a crouching jump boost
                StandUp ( );

                playerRb.AddForce ( new Vector2 ( 0f, crouchJumpBoost ), ForceMode2D.Impulse );
            }

            //...The player is no longer on the ground and is jumping...
            isOnGround = false;
            isJumping = true;

            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;

            //...add the jump force to the rigidbody...
            playerRb.AddForce ( new Vector2 ( 0f, jumpForce ), ForceMode2D.Impulse );
        }
        //Otherwise, if currently within the jump time window...
        else if ( isJumping )
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if ( jumpBut.jumpHeld )
            {
                playerRb.AddForce ( new Vector2 ( 0f, jumpHoldForce ), ForceMode2D.Impulse );
            }
            //...and if jump time is past, set isJumping to false
            if ( jumpTime <= Time.time )
            {
                isJumping = false;
                jumpBut.jumpPressed = false;
            }
        }
        #endregion
        //If player is falling to fast, reduce the Y velocity to the max
        if ( playerRb.velocity.y < maxFallSpeed )
        {
            playerRb.velocity = new Vector2 ( playerRb.velocity.x, maxFallSpeed );
        }
        //playerAnimator.SetFloat ( speedParamID, 0f ); // stops walikng in the air
    }

    private IEnumerator Fire ( float waitTime )
    {
        // shoot one bullt
        FireLazer ( );
        // STOP shooting (when false can't shoot)
        canShoot = false;

        // wait for some time (waitTime)
        yield return new WaitForSeconds ( waitTime );
        // turn on shooting
        canShoot = true;
        gunFlash.Stop ( );
    }

    void FireLazer ( )
    {
        GameObject obj = objectPooler.SpawnFromPool("LazerFire1", gun.transform.position);
        // flip bullet according to player direction or last direction
        if ( lastMoveDirection > 0 )
        {
            obj.transform.Rotate ( 0f, 0f, 0f );
        }

        else if ( lastMoveDirection < 0 )
        {
            obj.transform.Rotate ( 0f, 180f, 0f );
        }

        //if ( OnLaserFire != null )
        //    OnLaserFire ( );
        OnLaserFire?.Invoke ( ); // simplified way of writing above

        gunFlash.Play ( );
    }

    void GetGun ( )
    {
        gun = playerAnimations.GetGun ( );
        gunFlash.transform.position = gun.position;
    }

    void Crouch ( )
    {
        isCrouching = true;
    }

    void StandUp ( )
    {
        //If the player's head is blocked, they can't stand so exit
        if ( playerRaycast.isHeadBlocked )
            return;

        //The player isn't crouching
        isCrouching = false; ;
    }

    private IEnumerator Dashing ( )
    {
        yield return new WaitForSeconds ( dashTime );
        isDashing = false;
        canDash = true;
        yield return new WaitForSeconds ( dashTimerWait );
        canDash = false;
        speedBut.speedPressed = false;
    }

    void StompEm ( )
    {
        if ( playerRb.velocity.y < 0 && !playerInWater && !isOnGround  )
        {
            stompBox.SetActive ( true ); // stomp box is active only when the player is falling down
        }
        else
        {
            stompBox.SetActive ( false );
        }
    }

    private void FlipPlayer ( )
    {
        if ( directionLR != 0 )

        {
            if ( directionLR < 0 )
            {
                playerFacingRignt = -1;
                directionLR = -1;
            }


            else if ( directionLR > 0 )
            {
                playerFacingRignt = 1;
                directionLR = 1;
            }

            transform.localScale = new Vector3 ( directionLR * scale.x, 1f * scale.y, 1f * scale.z );
        }
    }

    /* for  the Walksound functions pass the onMovingPlatform bool. The audio manager will decide the sample to use depending on the state of the bool.
     */
    void WalkSound1 ( )
    {
        if ( isOnGround )
        {
            if ( OnWalkSound1 != null )
                OnWalkSound1 ( onMovingPlat );
        }
    }

    void WalkSound2 ( )
    {
        if ( isOnGround )
        {
            if ( OnWalkSound2 != null )
                OnWalkSound2 ( onMovingPlat );
        }
    }

    void JumpUp ( )
    {
        if ( OnJumpSound != null )
            OnJumpSound ( );
    }

    void EscapePressed()
    {
        if(pauseBut.pausePressed)
        {
            Debug.Log ( "EscapePressed" );
            pauseBut.pausePressed = false;
            if ( OnEscapePressed != null )
                OnEscapePressed ( );
        }
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.CompareTag ( "CheckPoint" ) )
        {
            respawnPosition = collision.transform.position;
        }
    }

    void DustOnLand ( )
    {
        dustOnLand.Play ( );
    }

    void RunDust ( ) // called from animation
    {
        if(isOnGround)
            runDust.Play ( );
    }

    void TorchOnOff ( )
    {
        if ( torchBut.torchPressed )
        {
            torch.enabled = true;
        }
        else
            torch.enabled = false;
    }

    void SetPlayerCanShoot()
    {
        canShoot = true;
    }

    void SetRespawnPosition()
    {
        this.transform.position = respawnPosition;
    }

}

