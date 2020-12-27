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

	public bool pausePressed;

	[HideInInspector]
	public bool Pressed;
	#endregion


	public void PauseGame (  )
	{
		if( Pressed )
        {
			pausePressed = !pausePressed;
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
