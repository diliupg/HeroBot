/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields
	private bool touched;
	public bool pausePressed;

	[HideInInspector]
	public bool Pressed;
	#endregion


	public void PauseGame (  )
	{
		if(Pressed && !touched)
        {
			pausePressed = !pausePressed;
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
