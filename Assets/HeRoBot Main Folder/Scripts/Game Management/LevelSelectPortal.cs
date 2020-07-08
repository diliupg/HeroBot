using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectPortal : MonoBehaviour
{
    public string levelToLoad;
    public bool unlocked;

    public Sprite doorBottomOpen;
    public Sprite doorTopOpen;
    public Sprite doorBottomClosed;
    public Sprite doorTopClosed;

    public SpriteRenderer doorTop;
    public SpriteRenderer doorBottom;

    void Start ()
    {
        PlayerPrefs.SetInt ( "Level1", 1 ); // because the first level is open anyway

        if ( PlayerPrefs.GetInt ( levelToLoad ) == 1 )
        {
            unlocked = true;
        }
        else
            unlocked = false;

        if(unlocked)
        {
            doorTop.sprite = doorTopOpen;
            doorBottom.sprite = doorBottomOpen;
        }
        else
        {
            doorTop.sprite = doorTopClosed;
            doorBottom.sprite = doorBottomClosed;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            if(Input.GetButtonDown("Jump") && unlocked)
            {
                SceneManager.LoadScene ( levelToLoad );
            }
        }
    }
}
