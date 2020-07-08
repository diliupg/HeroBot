using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthToGive;

    private LevelManager _levelManager;
    void Start ( )
    {
        _levelManager = FindObjectOfType<LevelManager> ( );
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.CompareTag ( "Player" ) )
        {
            _levelManager.GiveHealth ( healthToGive );
            gameObject.SetActive ( false ); // deactivate this object so we can't get more from it
        }
    }
}
