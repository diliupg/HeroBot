using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // needed to use event Action
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public static event Action PlayerLevelComplete; // create a static event which has global access

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.CompareTag ( "Player" ) )
        {
            if ( PlayerLevelComplete != null )
                PlayerLevelComplete ( ); // Invoke or launch the event
        }
    }
}
