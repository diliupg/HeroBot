using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera NormalVCam;
    public CinemachineVirtualCamera ZoomUpVCam;
    public CinemachineVirtualCamera ZoomDownVCam;

    private int camNormal = 1;
    private int camUp = -1;
    private int camDown = -2;

    private bool centered;
    private bool up;
    private bool down;

    void Start()
    {
        GameManager.RegisterCameraManager ( this );

        NormalVCam.Priority = camNormal;
        ZoomUpVCam.Priority = camUp;
        ZoomDownVCam.Priority = camDown;
        centered = true;

    }

    private void OnEnable ( )
    {
        PlayerController.UpOrDownPressed += CameraLook;
    }

    private void OnDisable ( )
    {
        PlayerController.UpOrDownPressed -= CameraLook;
    }

    void CameraLook ( float value )
    {
        if(value == 0 && !centered )
        {
            camUp = -1;
            camDown = -2;

            up = false;
            centered = true;
            down = false;

            SetCamPriority ( );
        }
        else if(value > 0 && !up )
        {
            camUp = 2;
            camDown = -2;

            up = true;
            down = false;
            centered = false;

            SetCamPriority ( );

        }
        else if ( value < 0 && !down )
        {
            camDown = 2;
            camUp = -1;

            up = false;
            down = true;
            centered = false;

            SetCamPriority ( );
        }
    }

    void SetCamPriority()
    {
        ZoomUpVCam.Priority = camUp;
        ZoomDownVCam.Priority = camDown;
    }

}
