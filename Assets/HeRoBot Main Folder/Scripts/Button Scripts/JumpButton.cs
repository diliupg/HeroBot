/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using Lean.Touch;

public class JumpButton : MonoBehaviour
{
	#region Public Fields

	public bool jumpPressed;
	public bool jumpHeld;

	#endregion

	public void SetJump()
    {
		jumpPressed = true;
    }
}
