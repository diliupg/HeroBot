using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public Sprite flagClosed;
    public Sprite flagOpen;
    public bool checkpointActive;

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer> ( );
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            _spriteRenderer.sprite = flagOpen;
            checkpointActive = true;
        }
    }
}
