using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private UnityAction someListener;

    private void Awake ( )
    {
        //someListener = new UnityAction ( SomeFunction  );
    }
    private void OnEnable ( )
    {
        EventManager.StartListening ( "Test", SomeFunction );
    }

    private void OnDisable ( )
    {
        EventManager.StopListening ( "Test", SomeFunction );
    }

    void SomeFunction()
    {
        Debug.Log ( "Some function was called" );
    }
}
