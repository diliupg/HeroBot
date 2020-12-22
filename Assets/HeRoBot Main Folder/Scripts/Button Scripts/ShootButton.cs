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

public class ShootButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	#region Public Fields

	[HideInInspector]
	public bool Pressed;
	public bool shootPressed;

	#endregion

	public void FireLazer (  )
	{
		shootPressed = Pressed;
	}

	public void StopFiring()
    {
		shootPressed = Pressed;
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
