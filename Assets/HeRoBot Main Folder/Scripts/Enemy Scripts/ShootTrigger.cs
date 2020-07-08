using System.Collections;
using System;
using UnityEngine;

public class ShootTrigger : MonoBehaviour
{
    public static event Action fire;
    void FireTrigger()
    {
        if ( fire != null )
            fire ( );
    }
}
