using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpenClose : MonoBehaviour
{
    GameObject gateSwitch;
    private AudioManager audioManager;
    private AudioSource audioSource;
    private Vector2 pos;
    float speed = 2f;
    void Start()
    {
        gateSwitch = GameObject.FindGameObjectWithTag ( "Switch" );
        audioManager = AudioManager.Instance;
        audioSource = GetComponent<AudioSource> ( );
        pos = transform.position;
    }

    private void OnEnable ( )
    {
        SwitchOnOff.OnOpenOrCloseGate += OpenOrCloseGate;
    }

    private void OnDisable ( )
    {
        SwitchOnOff.OnOpenOrCloseGate -= OpenOrCloseGate;
    }

    public void OpenOrCloseGate( bool state)
    {
        float temp;
        print ( state );
        if ( !state )
            temp = -speed;
        else
            temp = speed;


        StartCoroutine ( ExtendRetract ( temp ) );
    }

    IEnumerator ExtendRetract (float openSpeed )
    {
        yield return new WaitForSeconds ( 0.8f );
        audioManager.EnemActivitySound ( audioSource, audioManager.GateOpen );
        yield return new WaitForSeconds ( 0.5f );
        for ( float t = 3f ; t >= 0 ; t -= Time.deltaTime )
        {
            transform.Translate ( Vector3.up * openSpeed * Time.deltaTime );
            yield return null;
        }
    }


}
