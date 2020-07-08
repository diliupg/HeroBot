using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;

[DefaultExecutionOrder ( -102 )]
public class PlayerRaycast : MonoBehaviour
{

    public bool drawDebugRaycasts = true;	//Should the environment checks be visualized
    public bool isOnGround;
    public bool onMovingPlatform;
    public bool isHeadBlocked;
    public bool isHanging;
    [HideInInspector]
    public Transform parent;

    private float playerHeight;

    public float footOffset = .4f;			//X Offset of feet raycast
    public float groundDistance = .2f;		//Distance player is considered to be on the ground
    public float headClearance = .5f;
    public float eyeHeight = 1.5f;
    public float grabDistance = .4f;		//The reach distance for wall grabs
    public float reachOffset = .7f;			//X offset for wall grabbing

    public int direction;

    public PlayerController playerController;

    public LayerMask whatIsGround;
    public BoxCollider2D bodyCollider;
    public Rigidbody2D rigidBody;

    const float smallAmount = .05f;			//A small amount used for hanging position
    public TagTypes tagTypes;
    private bool onCollapsiblePlat;

    private void Start ( )
    {
        playerHeight = bodyCollider.size.y;
        isHanging = false;

    }
    void FixedUpdate ( )
    {
        PhysicsCheck ( );
        direction = playerController.playerFacingRignt;
    }

    void PhysicsCheck ( )
    {
        //Start by assuming player isn't on the ground, head isn't blocked and is not on a moving platform
        isOnGround = false;
        isHeadBlocked = false;
        onMovingPlatform = false;

        //Cast rays for the left and right foot and center
        RaycastHit2D centerCheck = Raycast(new Vector2(0f, -1f), Vector2.down, groundDistance);
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, -1f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, -1f), Vector2.down, groundDistance);

        //If either ray hit the ground, the player is on the ground
        if ( leftCheck && leftCheck.transform.gameObject.layer == LayerMask.NameToLayer ( "Ground" ) || rightCheck && rightCheck.transform.gameObject.layer == LayerMask.NameToLayer ( "Ground" ) )
            isOnGround = true;

        if ( leftCheck && leftCheck.transform.gameObject.CompareTag ( "MovingPlatform" ) || rightCheck && rightCheck.transform.gameObject.CompareTag ( "MovingPlatform" ) )
        {
            onMovingPlatform = true;
            if ( leftCheck )
                parent = leftCheck.transform;
            if ( rightCheck )
                parent = rightCheck.transform;
        }

        if ( centerCheck && centerCheck.transform.gameObject.CompareTag ( "CollapsiblePlatform" ) && !onCollapsiblePlat )
        {
            onCollapsiblePlat = true;
            centerCheck.transform.gameObject.GetComponent<Animator> ( ).SetBool ( "shake", true );

            centerCheck.transform.gameObject.GetComponent<CollapsablePlatform> ( ).CollapsePlatform ( );
        }
        else
            onCollapsiblePlat = false;


        //Cast the ray upwards to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, (bodyCollider.size.y / 2f) + bodyCollider.offset.y), Vector2.up, headClearance);

        //If that ray hits, the player's head is blocked
        if ( headCheck )
            isHeadBlocked = true;

        //Determine the direction of the wall grab attempt
        Vector2 grabDir = new Vector2(direction, 0f);

        //Cast three rays to look for a wall grab
        // upper horizontal ray
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight * .42f), grabDir, grabDistance);
        // middle downward ray
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight * .42f), Vector2.down, grabDistance);
        // lower horizontal ray
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight * .35f), grabDir, grabDistance);

        //If the player is off the ground AND is not hanging AND is falling AND
        //found a ledge AND found a wall AND the grab is NOT blocked...
        if ( !isOnGround && !isHanging && rigidBody.velocity.y < 0f &&
            ledgeCheck && wallCheck && !blockedCheck && !playerController.playerInWater )
        {
            //...we have a ledge grab. Record the current position...
            Vector3 pos = transform.position;
            //...move the distance to the wall (minus a small amount)...
            pos.x += ( wallCheck.distance - smallAmount ) * direction;
            //...move the player down to grab onto the ledge...
            pos.y -= ledgeCheck.distance;
            //...apply this position to the platform...
            transform.position = pos;
            //...set the rigidbody to static...
            rigidBody.bodyType = RigidbodyType2D.Static;
            //...finally, set isHanging to true

            isHanging = true;
        }
    }



    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast ( Vector2 offset, Vector2 rayDirection, float length )
    {
        //Call the overloaded Raycast() method using the ground layermask and return
        //the results
        return Raycast ( offset, rayDirection, length, whatIsGround );
    }

    RaycastHit2D Raycast ( Vector2 offset, Vector2 rayDirection, float length, LayerMask mask )
    {
        //Record the player's position
        Vector2 pos = transform.position;

        //Send out the desired raycasr and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //if (hit && hit.transform.gameObject.layer == LayerMask.NameToLayer ( "Ground" ) )
        //    Debug.Log ( "GROUNDED" );

        //if ( hit && hit.transform.gameObject.layer == LayerMask.NameToLayer ( "MovingPlatform" ) )
        //    Debug.Log ( "MOVINGPLATFORM" );

        //If we want to show debug raycasts in the scene...
        if ( drawDebugRaycasts )
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay ( pos + offset, rayDirection * length, color );
        }

        //Return the results of the raycast
        return hit;
    }


}