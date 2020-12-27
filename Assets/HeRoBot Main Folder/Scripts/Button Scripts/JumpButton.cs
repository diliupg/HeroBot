/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields

	[HideInInspector]
	public bool Pressed;

	public bool jumpPressed;
	public bool jumpHeld;

	#endregion

	public void SetJump()
    {
		if( Pressed )
        {
			jumpPressed = true;
		}		
    }

	public void OnPointerDown ( PointerEventData eventData )
	{
		Pressed = true;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		Pressed = false;

	}
}
