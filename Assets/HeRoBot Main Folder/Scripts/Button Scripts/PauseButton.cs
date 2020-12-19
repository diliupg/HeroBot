/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using Lean.Touch;

public class PauseButton : MonoBehaviour
{
	#region Public Fields

	public bool pausePressed;

	#endregion


	public void PauseGame (  )
	{
		pausePressed = true;
	}

}
