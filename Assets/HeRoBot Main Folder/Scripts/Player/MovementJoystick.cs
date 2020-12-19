/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    #region Public Fields

    public GameObject joyS;
    public GameObject joySBorder;
    public Vector2 joySVec2;
    public Vector2 joySTouchPos;
    public Vector2 joySStartPos;
    public float joySRadius;

    #endregion
	
    void Start()
    {
        joySStartPos = joySBorder.transform.position;
        joySRadius = joySBorder.GetComponent<RectTransform> ( ).sizeDelta.y / 8;
    }

    public void PointerDown()
    {
        joyS.transform.position = Input.mousePosition;
        joySBorder.transform.position = Input.mousePosition;
        joySTouchPos = Input.mousePosition;
        Debug.Log ( "finger Down" );
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joySVec2 = ( dragPos - joySTouchPos ).normalized;
        //Debug.Log ( "joystick vector "+joystickVector );
        float joystickDist = Vector2.Distance ( dragPos, joySTouchPos );

        if(joystickDist < joySRadius)
        {
            joyS.transform.position = joySTouchPos + joySVec2 * joystickDist;
        }
        else
        {
            joyS.transform.position = joySTouchPos + joySVec2 * joySRadius;
        }
        Debug.Log ( "joystick trans. Pos "+joyS.transform.position );
    }

    public void PointerUP()
    {
        joySVec2 = Vector2.zero;
        joyS.transform.position = joySStartPos;
        joySBorder.transform.position = joySStartPos;
        Debug.Log ( "finger Up" );
    }
}