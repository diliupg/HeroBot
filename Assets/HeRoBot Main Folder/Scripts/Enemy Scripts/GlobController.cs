using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobController : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public float moveSpeed;
    public bool movingRight;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D> ( );
    }

    void FixedUpdate()
    {
        if ( movingRight && transform.position.x > rightPoint.position.x )
        {
            movingRight = false;
        }
        else if ( !movingRight && transform.position.x < leftPoint.position.x )
        {
            movingRight = true;
        }

        if ( movingRight )
            _rb.velocity = new Vector3 ( moveSpeed, _rb.velocity.y, 0 );
        else
            _rb.velocity = new Vector3 ( -moveSpeed, _rb.velocity.y, 0 );
    }
}
