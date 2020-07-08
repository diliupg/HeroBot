using System.Collections;
using System;
using UnityEngine;

public class InWater : MonoBehaviour
{
    private PlayerAnimations playerAnims;
    private bool inWater;

    public static event Action<bool> inTheWater;

    void Start()
    {
        playerAnims = GetComponent<PlayerAnimations> ( );
        inWater = false;
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.gameObject.CompareTag ( "Water" ) )
        {
            inWater = true;

            if ( inTheWater != null )
            {
                inTheWater ( inWater ); // if there is a subscriber, broadcast the event
            }
        }
    }

    private void OnTriggerStay2D ( Collider2D collision )
    {
        if ( collision.gameObject.CompareTag ( "Water" ) )
        {
            inWater = true;

            if ( inTheWater != null )
            {
                inTheWater ( inWater ); // if there is a subscriber, broadcast the event
            }
        }
    }

    private void OnTriggerExit2D ( Collider2D collision )
    {
        if ( collision.gameObject.CompareTag ( "Water" ) )
        {
            inWater = false;

            if ( inTheWater != null )
            {
                inTheWater ( inWater ); // if there is a subscriber, broadcast the event
            }
        }
    }
}
