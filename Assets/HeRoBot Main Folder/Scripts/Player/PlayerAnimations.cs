using UnityEngine;

[DefaultExecutionOrder ( -102 )]
public class PlayerAnimations : MonoBehaviour
{
    private Rigidbody2D playerRb;
    //private InputManager input;          //The current inputs for the player
    private Animator playerAnimator;
    private PlayerController playerController;
    private PlayerRaycast playerRaycast;

    public Transform firePointIdle;
    public Transform firePointRun;
    public Transform firePointWalk;
    public Transform firePointCrouch;
    public Transform firePointCrawl;
    public Transform swimAndShoot;
    public Transform IdleWaterShoot;

    private int speedParamID;                // ID of the speed paramter in the Animator
    private int isGroundedParamID;           // ID of the ground paramter in the Animator
    private int fallParamID;                 // ID of the fall paramter in the Animator
    private int isCrouchingParamID;          // ID of the croucn paramter in the Animator
    private int iSShootingParamID;           // ID of the shoot paramter in the Animator
    private int isDashingParamID;            // ID of the jump paramter in the Animator
    private int isJumpingParamID;            // ID of the isJumping paramter in the Animator
    private int isHangingParamID;            // ID of the grabWall parameter in the Animator
    private int isSwimmingParamID;           // ID for the isSwimming parameter in the Animator
    private int isInWaterParamID;
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D> ( );
        //input = GetComponent<InputManager> ( );
        playerAnimator = GetComponent<Animator> ( );
        playerController = GetComponent<PlayerController> ( );
        playerRaycast = GetComponent<PlayerRaycast> ( );

        #region Animator Patrameters
        speedParamID = Animator.StringToHash ( "Speed" );
        fallParamID = Animator.StringToHash ( "VerticalVelocity" );
        isHangingParamID = Animator.StringToHash ( "IsHanging" );
        isGroundedParamID = Animator.StringToHash ( "IsOnGround" );
        isCrouchingParamID = Animator.StringToHash ( "IsCrouching" );
        isDashingParamID = Animator.StringToHash ( "IsDashing" );
        iSShootingParamID = Animator.StringToHash ( "Shoot" );
        isJumpingParamID = Animator.StringToHash ( "IsJumping" );
        isSwimmingParamID = Animator.StringToHash ( "IsSwimming" );
        isInWaterParamID = Animator.StringToHash ( "InWater" );
        #endregion

        //If any of the needed components don't exist...
        if ( playerController == null || playerRb == null || playerAnimator == null )
        {
            //...log an error and then remove this component
            Debug.LogError ( "A needed component is missing from the player" );
            Destroy ( this );
        }
    }

    void Update()
    {
        playerAnimator.SetFloat ( speedParamID, Mathf.Abs ( playerController.directionLR ) );
        playerAnimator.SetFloat ( fallParamID, playerRb.velocity.y );
        playerAnimator.SetBool ( isHangingParamID, playerRaycast.isHanging );
        playerAnimator.SetBool ( isGroundedParamID, playerRaycast.isOnGround );
        playerAnimator.SetBool ( isCrouchingParamID, playerController.isCrouching );
        playerAnimator.SetBool ( iSShootingParamID, playerController.isShooting );
        playerAnimator.SetBool ( isDashingParamID, playerController.isDashing );
        playerAnimator.SetBool ( isJumpingParamID, playerController.isJumping );
        playerAnimator.SetBool ( isSwimmingParamID, playerController.MoveinWater );
        playerAnimator.SetBool ( isInWaterParamID, playerController.playerInWater );
    }

    public Transform GetGun ( )
    {
        Transform gun = firePointIdle;

        if ( playerAnimator.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "Shooting" ) && !playerController.playerInWater )
            gun = firePointIdle;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 1 ).IsName ( "RunShoot" ) )
            gun = firePointRun;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "WalkShoot" ) && !playerController.playerInWater )
            gun = firePointWalk;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "CrouchIdleShoot" ) )
            gun = firePointCrouch;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 0 ).IsName ( "CrawlShoot" ) )
            gun = firePointCrawl;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 2 ).IsName ( "SwimNShoot" ) )
            gun = swimAndShoot;

        else if ( playerAnimator.GetCurrentAnimatorStateInfo ( 2 ).IsName ( "IdleInWaterShoot" ) )
            gun = IdleWaterShoot;

        return gun;
    }
}
