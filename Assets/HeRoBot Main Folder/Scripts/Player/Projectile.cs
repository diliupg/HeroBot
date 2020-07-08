using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    public float bulletSpeed;
    [SerializeField] private float bulletOffTime;
    bool hitEnemy;

    public void OnObjectSpawn() // this is called immediatly after the object is spawned from the ObjectPooler
    {
        hitEnemy = false;
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(TurnOffSprite());
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
        IDamageble hit = other.GetComponent<IDamageble>();
        if( hit != null )
        {
            hit.Damage ( );
            gameObject.SetActive ( false );
        }
    }

    IEnumerator TurnOffSprite()
    {
        yield return new WaitForSeconds(bulletOffTime);

        if(this.gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }
}
