/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields

		[HideInInspector]
	public bool Pressed;

	public bool shootPressed;

	public bool crouchPressed;
	public bool crouchHeld;

	public bool speedPressed;

    public bool pausePressed;

	public bool torchPressed;

	#endregion

	public void SetCrouch ( )
	{
		if ( Pressed )
		{
			crouchPressed = !crouchPressed;
			crouchHeld = !crouchHeld;
		}
	}

	public void FireLazer ( )
	{
		shootPressed = Pressed;

	}

	public void StopFiring ( )
	{
		shootPressed = Pressed;

	}

	public void SpeedBurst ( )
	{
		if ( Pressed )
		{
			speedPressed = true;
		}
	}

	public void PauseGame ( )
	{
		if ( Pressed )
		{
			pausePressed = !pausePressed;
		}
	}

	public void TorchOnOff ( )
	{
		if ( Pressed )
		{
			torchPressed = !torchPressed;
		}
	}

	public void OnPointerDown ( PointerEventData eventData )
	{
		Pressed = true;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		Pressed = false;
		//touched = false;
	}
}
