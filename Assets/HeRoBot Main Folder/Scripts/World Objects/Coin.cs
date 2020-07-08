using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    private GameManager gameManager;
    public int coinValue;


    void Start()
    {
        gameManager = FindObjectOfType<GameManager> ( );
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            gameManager.AddCoins ( coinValue );
            //Destroy ( gameObject );
            gameObject.SetActive ( false );
        }
    }
}
