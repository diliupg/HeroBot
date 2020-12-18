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

    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickVector;
    public Vector2 joystickTouchPos;
    public Vector2 joystickOriginalPos;
    public float joystickRadius;

    #endregion
	
    void Start()
    {
        joystickOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform> ( ).sizeDelta.y / 8;
    }

    public void PointerDown()
    {
        joystick.transform.position = Input.mousePosition;
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVector = ( dragPos - joystickTouchPos ).normalized;
        Debug.Log ( joystickVector );
        float joystickDist = Vector2.Distance ( dragPos, joystickTouchPos );

        if(joystickDist < joystickRadius)
        {
            joystick.transform.position = joystickTouchPos + joystickVector * joystickDist;
        }
        else
        {
            joystick.transform.position = joystickTouchPos + joystickVector * joystickRadius;
        }
    }

    public void PointerUP()
    {
        joystickVector = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
        joystickBG.transform.position = joystickOriginalPos;
    }
}


















