/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using Lean.Touch;

public class CrouchButton : MonoBehaviour
{
	#region Public Fields

	public bool crouchPressed;
	public bool crouchHeld;

	#endregion

	public void SetCrouch()
    {
		crouchPressed = !crouchPressed;
		crouchHeld = !crouchHeld;
	}

}
