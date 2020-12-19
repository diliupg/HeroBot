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
using Lean.Touch;

public class ShootButton : MonoBehaviour
{
	#region Public Fields

	public bool shootPressed;

	#endregion

	public void FireLazer (  )
	{
		shootPressed = true;
	}

	public void StopFiring()
    {
		shootPressed = false;
    }

}
