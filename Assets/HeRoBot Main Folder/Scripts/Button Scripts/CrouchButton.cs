/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class CrouchButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields

	[HideInInspector]
	public bool Pressed;

	public bool crouchPressed;
	public bool crouchHeld;

	#endregion

	public void SetCrouch()
    {
		if(Pressed )
        {
			crouchPressed = !crouchPressed;
			crouchHeld = !crouchHeld;
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
