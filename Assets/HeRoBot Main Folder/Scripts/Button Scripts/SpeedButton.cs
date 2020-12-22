/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields
	private bool touched;
	public bool speedPressed;

	[HideInInspector]
	public bool Pressed;

	#endregion

	public void SpeedBurst (  )
	{
		if(Pressed && !touched)
        {
			speedPressed = true;
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
