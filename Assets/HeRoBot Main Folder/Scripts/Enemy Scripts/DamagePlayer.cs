using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour // this script is on the spike grid and spinSaws
{
    [SerializeField] protected int damageAmount;

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if ( collision.gameObject.CompareTag ( "Player" ) )
        {
            IPlayerDamage hit = collision.gameObject.GetComponent<IPlayerDamage>();

            if ( hit != null )
            {
                hit.Damage ( damageAmount, true );
                hit.KnockBack ( );
            }
        }
    }
}
