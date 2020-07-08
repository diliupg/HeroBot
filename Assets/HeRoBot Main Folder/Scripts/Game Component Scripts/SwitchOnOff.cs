using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SwitchOnOff : MonoBehaviour
{
    Animator animator;
    private AudioManager audioManager;
    public bool setSwitchOnOff;

    public static event Action<bool> OnOpenOrCloseGate;

    private void Start ( )
    {
        animator = this.GetComponent<Animator> ( );
        audioManager = AudioManager.Instance;
        setSwitchOnOff = false;
    }
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            animator.SetTrigger ( "switch" );
            audioManager.PlaySFX ( audioManager.SwitchOn );
            SetOnOff ( );
        }

    }

    void SetOnOff()
    {
        setSwitchOnOff = !setSwitchOnOff;

        if ( OnOpenOrCloseGate != null )
            OnOpenOrCloseGate ( setSwitchOnOff );
    }
}
