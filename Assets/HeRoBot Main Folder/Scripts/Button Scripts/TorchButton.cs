/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class TorchButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields

	public bool torchPressed;

	[HideInInspector]
	public bool Pressed;

	#endregion

	public void TorchOnOff()
    {
		if( Pressed )
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
	}
}