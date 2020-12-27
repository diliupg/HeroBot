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

	public bool speedPressed;

	[HideInInspector]
	public bool Pressed;

	#endregion

	public void SpeedBurst (  )
	{
		if( Pressed )
        {
			speedPressed = true;
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
