/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
using Lean.Touch;

public class TorchButton : MonoBehaviour
{
	#region Public Fields

	public bool torchPressed;


	#endregion

	public void TorchOnOff()
    {
		torchPressed = !torchPressed;
	}
}