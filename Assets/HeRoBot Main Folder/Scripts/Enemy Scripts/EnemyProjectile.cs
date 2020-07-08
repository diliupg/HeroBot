using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPooledObject
{
    public float bulletSpeed;
    public int playerDamage;
    public float offTime;

    private Animator anim;
    private bool hitEnemy;
    private string playEndAnim = "End";
    private bool endOk;
    private bool hitPlayer;

    public void OnObjectSpawn()
    {
        hitEnemy = false;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        anim = GetComponentInChildren<Animator> ( );
        StartCoroutine (TurnOffSprite(offTime));
        foreach ( AnimatorControllerParameter param in anim.parameters )
        {
            if ( param.name == playEndAnim )
                endOk = true;
        }
    }

    private void OnEnable ( )
    {
        EnemyAI.setDamage += SetPlayerDamageValue;
    }

    private void OnDisable ( )
    {
        EnemyAI.setDamage -= SetPlayerDamageValue;
    }

    private void Update()
    {
        if ( !hitEnemy )
        {
            transform.Translate ( Vector3.right * bulletSpeed * Time.deltaTime );
        }
    }

    private void OnTriggerEnter2D( Collider2D other )
    {
        if ( other.CompareTag ( "Player" ) )
        {
            IPlayerDamage hit = other.GetComponent<IPlayerDamage>();
            if ( hit != null && !hitPlayer )
            {
                hit.Damage ( playerDamage, true );
                hitPlayer = true;
            }

            if (endOk)
            {
                bulletSpeed = 0;
                TurnOffSprite ( 2f );
                anim.SetBool ( playEndAnim, true );
            }
            else
                this.gameObject.SetActive ( false );
        }
    }

    IEnumerator TurnOffSprite( float time) // called in start. after the wait time has lapsed the line(s) below executed
    {
        yield return new WaitForSeconds(time);

        if(this.gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }

    void SetPlayerDamageValue( int val)
    {
        playerDamage = val;
    }
}
