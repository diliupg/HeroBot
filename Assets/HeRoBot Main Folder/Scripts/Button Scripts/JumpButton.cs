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
	private bool touched;
	public bool jumpPressed;
	public bool jumpHeld;

	#endregion

	public void SetJump()
    {
		if(Pressed && !touched)
        {
			jumpPressed = true;
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
