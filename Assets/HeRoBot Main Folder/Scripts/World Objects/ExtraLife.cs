using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public int livesToGive;

    private LevelManager _levelManager;
    void Start()
    {
        _levelManager = FindObjectOfType<LevelManager> ( );
    }

    // Update is called once per frame
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            _levelManager.AddLIves ( livesToGive );
            gameObject.SetActive ( false );// deactivate this object so we can't get more from it
        }
    }
}
