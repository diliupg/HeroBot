using System.Collections;
using UnityEngine;

public class MovingSpike : DamagePlayer
{
    [SerializeField] float moveSpeed;
    [SerializeField] float waitTimeMin;
    [SerializeField] float waitTimeMax;

    private bool extend = true;
    private bool runRoutine = false;

    private float waitTime;
    private Vector2 pos;

    private void Start ( )
    {
        GetWaitTime();
        pos = transform.position;
    }
    void Update()
    {

        if (extend)
        {
            StartCoroutine ( MachineLoop() );
            extend = !extend;
        }
    }

    IEnumerator MachineLoop ( )
    {
        yield return new WaitForSeconds ( waitTime );
        StartCoroutine( ExtendRetract (moveSpeed ) ); // UP
        moveSpeed = -moveSpeed;

        yield return new WaitForSeconds ( waitTime );
        StartCoroutine ( ExtendRetract ( moveSpeed ) ); // DOWN
        moveSpeed = -moveSpeed;

        yield return new WaitForSeconds ( waitTime );
        extend = !extend;
        transform.position = pos;
        GetWaitTime ( );
    }

    IEnumerator ExtendRetract(float speed )
    {
        for(float t = 0.18f ; t >= 0 ; t -= Time.deltaTime )
        {
            transform.Translate ( Vector3.up * speed * Time.deltaTime );
            yield return null;
        }
    }

    void GetWaitTime()
    {
        waitTime = Random.Range ( waitTimeMin, waitTimeMax );
    }

}
