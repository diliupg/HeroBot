/*
==========================
Copyright (c) Diliupg 2020
   www.soft.diliupg.com
==========================
*/

using System;
using UnityEngine;

public class KeyboardController : ButtonManager
{
    #region Public Fields

    int horizontal;
    int vertical;
    #endregion


    public static event Action<int> OnPlayerHorizontal;
    public static event Action OnDashPressed;
    public static event Action OnFirePressed;
    public static event Action OnFireReleased;
    public static event Action OnJumpPressed;
    public static event Action OnPausePressed;
    public static event Action OnCrouchPressed;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

    void Update()
    {
        horizontal = ( int ) Input.GetAxisRaw ( "Horizontal" );
        vertical = ( int ) Input.GetAxisRaw ( "Vertical" );
        
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp ( KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp ( KeyCode.RightArrow ) )
        {
            if ( OnPlayerHorizontal != null )
                OnPlayerHorizontal ( 0 );
        }
        if(horizontal != 0)
        {
            if ( OnPlayerHorizontal != null )
                OnPlayerHorizontal ( horizontal );
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if ( OnDashPressed != null )
                OnDashPressed ( );
        }
        //else if(Input.GetKeyUp(KeyCode.Z))
        //{
        //    if ( OnDashPressed != null )
        //        OnDashPressed ( false );
        //}
        if ( Input.GetKey ( KeyCode.C ) || Input.GetKey ( KeyCode.LeftShift ) )
        {
            if ( OnFirePressed != null )
                OnFirePressed (  );
        }
        if ( Input.GetKeyUp ( KeyCode.C ) || Input.GetKeyUp ( KeyCode.LeftShift ) )
        {
            if ( OnFireReleased != null )
                OnFireReleased (  );
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if ( OnJumpPressed != null )
                OnJumpPressed ( );
        }
        if ( Input.GetKeyDown ( KeyCode.LeftControl ) || Input.GetKeyDown ( KeyCode.DownArrow ) )
        {
            if ( OnCrouchPressed != null )
                OnCrouchPressed ( );
        }
        if ( Input.GetKeyDown ( KeyCode.Escape ) )
        {
            if ( OnPausePressed != null )
                OnPausePressed ( );
        }

    }

#endif
}
