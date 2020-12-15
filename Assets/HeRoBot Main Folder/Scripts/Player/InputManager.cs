using UnityEngine;

[DefaultExecutionOrder ( -104)]
public class InputManager : MonoBehaviour
{
    public bool testTouchControlsInEditor = false;  //Should touch controls be tested?
    public float verticalDPadThreshold = .5f;       //Threshold touch pad inputs
    public Thumbstick thumbstick;                   //Reference to Thumbstick
    public TouchButton jumpButton;					//Reference to jump TouchButton

    [HideInInspector] public float directionLR;      //Float that stores horizontal input
    [HideInInspector] public float directionUD;    // float that stores the vertical input

    [HideInInspector] public bool jumpHeld;         //Bool that stores jump pressed
    [HideInInspector] public bool jumpPressed;      //Bool that stores jump held
    [HideInInspector] public bool crouchHeld;       //Bool that stores crouch pressed
    [HideInInspector] public bool crouchPressed;    //Bool that stores crouch held
    [HideInInspector] public bool shootPressed;    //Bool that stores shoot pressed
    [HideInInspector] public bool dashPressed;    //Bool that stores dash pressed
    [HideInInspector] public bool torchPressed;    //Bool that stores torch pressed
    [HideInInspector] public bool pausedPressed;    //Bool that stores escape pressed

    bool dPadCrouchPrev;                            //Previous values of touch Thumbstick
    bool readyToClear;  							//Bool used to keep input in sync

    GameManager gameManager;
    private void Start ( )
    {
        gameManager = GameManager.Instance;
    }
    void Update ( )
    {
        ClearInput ( );

        //If the Game Manager says the game is over, exit
        if ( GameManager.IsGameOver ( ) || GameManager.LevelComplete () )
            return;

        //Process keyboard, mouse, gamepad (etc) inputs
        ProcessInputs ( );

        //MobileInputs ( );

        directionLR = Mathf.Clamp ( directionLR, -1f, 1f );

        directionUD = Mathf.Clamp ( directionUD, -1f, 1f );
    }

    private void FixedUpdate ( )
    {
        //In FixedUpdate() we set a flag that lets inputs to be cleared out during the
        //next Update(). This ensures that all code gets to use the current inputs
        readyToClear = true;
    }

    void ClearInput ( )
    {
        //If we're not ready to clear input, exit
        if ( !readyToClear )
            return;

        //Reset all inputs
        directionLR = 0f;
        directionUD = 0f;

        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;
        shootPressed = false;
        dashPressed = false;
        readyToClear = false;
        torchPressed = false;
        pausedPressed = false;

    }


    //void ProcessTouchInputs ( )
    //{
    //    //If this isn't a mobile platform AND we aren't testing in editor, exit
    //    if ( !Application.isMobilePlatform && !testTouchControlsInEditor )
    //        return;

    //    //Record inputs from screen thumbstick
    //    Vector2 thumbstickInput = thumbstick.GetDirection ( );

    //    //Accumulate horizontal input
    //    directionLR += thumbstickInput.x;

    //    //Accumulate jump button input
    //    jumpPressed = jumpPressed || jumpButton.GetButtonDown ( );
    //    jumpHeld = jumpHeld || jumpButton.GetButton ( );

    //    //Using thumbstick, accumulate crouch input
    //    bool dPadCrouch = thumbstickInput.y <= -verticalDPadThreshold;
    //    crouchPressed = crouchPressed || ( dPadCrouch && !dPadCrouchPrev );
    //    crouchHeld = crouchHeld || dPadCrouch;

    //    //Record whether or not playing is crouching this frame (used for determining
    //    //if button is pressed for first time or held
    //    dPadCrouchPrev = dPadCrouch;
    //}

    void MobileInputs ( )
    {
        var fingers = Lean.Touch.LeanTouch.GetFingers ( true, false );
        
        var delta = Lean.Touch.LeanGesture.GetScaledDelta ( fingers );

        if ( delta != Vector2.zero )
        {
            // Horizontal
            if ( Mathf.Abs ( delta.x ) > Mathf.Abs ( delta.y ) )
            {
                // Right
                if ( delta.x > 0.0f )
                {
                    Debug.Log ( "RIGHT" );
                    directionLR = 1;
                }
                // Left
                else
                {
                    Debug.Log ( "LEFT" );
                    directionLR = -1;
                }
            }
            // Vertical
            else
            {
                // Up
                if ( delta.y > 0.0f )
                {
                    Debug.Log ( "UP" );
                    directionUD = 1;
                }
                // Down
                else
                {
                    Debug.Log ( "DOWN" );
                    directionUD = -1;
                }
            }
        }
    }

    void ProcessInputs ( )
    {
        //Accumulate horizontal axis input
        directionLR += Input.GetAxis ( "Horizontal" );

        //Accumulate vertical axis input
        directionUD += Input.GetAxis ( "Vertical" );

        //Accumulate button inputs
        jumpPressed = jumpPressed || Input.GetButtonDown ( "Jump" );
        jumpHeld = jumpHeld || Input.GetButton ( "Jump" );

        crouchPressed = crouchPressed || Input.GetButtonDown ( "Crouch" );
        crouchHeld = crouchHeld || Input.GetButton ( "Crouch" );

        shootPressed = Input.GetButton ( ( "Fire1" ) );

        dashPressed = Input.GetButtonDown ( "Dash" );

        torchPressed = Input.GetButtonDown ( "Torch" );

        pausedPressed = Input.GetButtonDown ( "Pause" );
    }

    public void Right()
    {
        Debug.Log ( "right" );
        directionLR = 1;
    }
    public void Left ( )
    {
        Debug.Log ( "left" );
        directionLR = -1;
    }
}
