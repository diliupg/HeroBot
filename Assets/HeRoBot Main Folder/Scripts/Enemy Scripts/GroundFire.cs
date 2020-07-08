using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFire : MonoBehaviour
{
    GameObject player;
    public float damage;

    void Start ( )
    {
        player = GameObject.FindGameObjectWithTag ( "Player" );
    }

    private void OnTriggerStay2D ( Collider2D other )
    {
        if ( other.gameObject.CompareTag ( "Player" ) )
        {
            IPlayerDamage hit = other.gameObject.GetComponent<IPlayerDamage>();

            if ( hit != null )
            {
                hit.Damage ( damage, false );
            }
        }
    }
}
