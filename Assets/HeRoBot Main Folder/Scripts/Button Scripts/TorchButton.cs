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
	private bool touched;
	public bool torchPressed;

	[HideInInspector]
	public bool Pressed;

	#endregion

	public void TorchOnOff()
    {
		if(Pressed && !touched)
        {
			torchPressed = !torchPressed;
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