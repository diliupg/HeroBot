using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float followAhead;
    public float smoothing;
    public bool followTarget;

    private Vector3 _targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        followTarget = true;
    }

    // Update is called once per frame
    void LateUpdate ( )
    {
        if ( followTarget )
        {
            _targetPosition = new Vector3 ( target.transform.position.x, transform.position.y, transform.position.z );

            if ( target.transform.localScale.x > 0f )
            {
                _targetPosition = new Vector3 ( _targetPosition.x + followAhead, _targetPosition.y, _targetPosition.z );
            }
            else
            {
                _targetPosition = new Vector3 ( _targetPosition.x - followAhead, _targetPosition.y, _targetPosition.z );
            }

            //transform.position = _targetPosition;
            transform.position = Vector3.Lerp ( transform.position, _targetPosition, smoothing * Time.deltaTime );
        }
    }
}
