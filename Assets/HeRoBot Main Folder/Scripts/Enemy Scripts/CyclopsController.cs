using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsController : MonoBehaviour
{
    public float moveSpeed;
    private bool canMove = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D> ( );
    }

    void FixedUpdate()
    {
        if ( canMove )
            rb.velocity = new Vector3 ( -moveSpeed, rb.velocity.y, 0 );
    }

    private void OnBecameVisible() // any camera. In game or scene view
    {
        canMove = true;
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("KillPlane"))
        {
            gameObject.SetActive ( false );
        }
    }

    private void OnEnable ( )
    {
        canMove = false;
    }
}
