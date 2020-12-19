/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementJoystick : MonoBehaviour
{
    #region Public Fields

    public GameObject joyS;
    public GameObject joySBorder;
    [SerializeField] private Text butStatus;
    [SerializeField] private Text boolStatus;

    public Vector2 joySVec2;
    public Vector2 joySTouchPos;
    public Vector2 joySStartPos;
    public float joySRadius;

    [HideInInspector] public float directionLR;      //Float that stores horizontal input
    [HideInInspector] public float directionUD;    // float that stores the vertical input

    private bool jumpButPressed;
    private bool torchButPressed;

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
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joySVec2 = ( dragPos - joySTouchPos ).normalized;

        float joystickDist = Vector2.Distance ( dragPos, joySTouchPos );

        if(joystickDist < joySRadius)
        {
            joyS.transform.position = joySTouchPos + joySVec2 * joystickDist;
        }
        else
        {
            joyS.transform.position = joySTouchPos + joySVec2 * joySRadius;
        }

        if ( joySVec2.x < 0 )
            directionLR = -1;
        else if ( joySVec2.x > 0 )
            directionLR = 1;

        if ( joySVec2.y < 0 )
            directionUD = -1;
        else if ( joySVec2.y > 0 )
            directionUD = 1;

        butStatus.text = ( "joys. vec " + joySVec2 + " player Move X dir " + directionLR + " player Move Y dir " + directionUD );
    }

    public void PointerUP()
    {
        joySVec2 = Vector2.zero;
        joyS.transform.position = joySStartPos;
        joySBorder.transform.position = joySStartPos;

        directionLR = directionUD = 0;

        butStatus.text = ( "finger Up "+ gameObject.name );
    }
}