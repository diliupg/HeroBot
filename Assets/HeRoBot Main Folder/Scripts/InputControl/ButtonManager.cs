/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using UnityEngine;
//using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
	#region Public Fields

	public bool Pressed;

	public bool shootPressed;

	public bool crouchPressed;
	public bool crouchHeld;

	public bool speedPressed;

    public bool pausePressed;

	public bool torchPressed;


	public bool jumpPressed;
	public bool jumpHeld;

    #endregion

    private void OnEnable(  )
    {
		KeyboardController.OnFirePressed += FireLazer;
		KeyboardController.OnFireReleased += StopFiring;
		KeyboardController.OnDashPressed += SpeedBurst;
		KeyboardController.OnJumpPressed += SetJump;
		KeyboardController.OnCrouchPressed += SetCrouch;
		KeyboardController.OnPausePressed += PauseGame;
	}
    private void OnDisable ( )
    {
		KeyboardController.OnFirePressed -= FireLazer;
		KeyboardController.OnFireReleased -= StopFiring;
		KeyboardController.OnDashPressed -= SpeedBurst;
		KeyboardController.OnJumpPressed -= SetJump;
		KeyboardController.OnCrouchPressed -= SetCrouch;
		KeyboardController.OnPausePressed -= PauseGame;
	}

    public void SetCrouch ( )
	{
		crouchPressed = !crouchPressed;
		crouchHeld = !crouchHeld;
	}

	public void FireLazer ( )
	{
		shootPressed = true;

	}

	public void StopFiring ( )
	{
		shootPressed = false;

	}

	public void SpeedBurst ( )
	{
		speedPressed = true;
	}

	public void PauseGame ( )
	{
		pausePressed = !pausePressed;
	}

	public void TorchOnOff ( )
	{
		torchPressed = !torchPressed;
	}

    public void SetJump ( )
	{
		jumpPressed = true;
	}

	//public void OnPointerDown ( PointerEventData eventData )
	//{
	//	Pressed = true;
	//	Debug.Log ( "from ButtonScript: pressed" );
	//}

	//public void OnPointerUp ( PointerEventData eventData )
	//{
	//	Pressed = false;
	//	Debug.Log ( "from ButtonScript: released" );
	//}
}
