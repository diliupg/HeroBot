using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float fadeTime;

    public Image blackScreen;

    void Start()
    {
        //blackScreen = GetComponent<Image> ( );
        blackScreen.enabled = true;
        ScreenToBlack ( );
    }

    // Update is called once per frame
    void Update()
    {
        ScreenToBlack ( );

    }

    void ScreenToBlack()
    {
        blackScreen.CrossFadeAlpha ( 1f, fadeTime, false );
    }

    void BlackToScreen()
    {
        print ( "ok" );
        blackScreen.CrossFadeAlpha ( 0f, fadeTime, false );
    }
}
