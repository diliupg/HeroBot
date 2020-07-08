using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsablePlatform : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    bool done;
    [SerializeField]
    protected float timeToShake;

    private void Start ( )
    {
        animator = GetComponent<Animator> ( );
        rb = GetComponent<Rigidbody2D> ( );
        rb.bodyType = RigidbodyType2D.Kinematic;
        done = false;
    }
    public void CollapsePlatform ( )
    {
        if ( !done )
        {
            done = true;

            animator.SetBool ( "shake", true );
            StartCoroutine ( ShakeAndFall ( ) );
        }

    }

    IEnumerator ShakeAndFall ( )
    {
        yield return new WaitForSeconds ( timeToShake );
        animator.SetBool ( "shake", false );
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 25;
    }
}
