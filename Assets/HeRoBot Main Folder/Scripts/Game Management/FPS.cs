using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/
public class FPS : MonoBehaviour
{
    public Text gameFPS;  //UI element.
    //public Text gameTime;

    private float fpsCounter = 0;
    private float currentFpsTime = 0;
    private float fpsShowPeriod = 1;

    // Update is called once per frame
    void Update ( )
    {
        currentFpsTime = currentFpsTime + Time.deltaTime;
        fpsCounter = fpsCounter + 1;
        if ( currentFpsTime > fpsShowPeriod )
        {
            gameFPS.text = fpsCounter.ToString ( );
            currentFpsTime = 0;
            fpsCounter = 0;
        }

        //gameTime.text = Mathf.Floor ( Time.time ).ToString ( );
    }
}

