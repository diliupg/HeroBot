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

	private bool touched;
	public bool crouchPressed;
	public bool crouchHeld;

	#endregion

	public void SetCrouch()
    {
		if(Pressed && !touched)
        {
			crouchPressed = !crouchPressed;
			crouchHeld = !crouchHeld;
			touched = true;
		}
	}

	public void OnPointerDown ( PointerEventData eventData )
	{
		Pressed = true;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		Pressed = false;
		touched = false;
	}
}
