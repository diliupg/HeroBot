using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompOnEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    private float _enemyLife;
    private Rigidbody2D _playerRigidbody;

    public GameObject EnemyExplosion;
    public float bounceFloat;
    void Start()
    {
        _playerRigidbody = GetComponentInParent<Rigidbody2D> ( );
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive ( false );

            _playerRigidbody.velocity = new Vector3 ( _playerRigidbody.velocity.x, bounceFloat, 0 );
            Instantiate ( EnemyExplosion, collision.transform.position, collision.transform.rotation );
        }

        if ( collision.CompareTag ( "Boss" ) )
        {
            _playerRigidbody.velocity = new Vector3 ( _playerRigidbody.velocity.x, bounceFloat, 0 );
            collision.transform.parent.GetComponent<Boss> ( ).takeDamage = true;
        }
    }
}
