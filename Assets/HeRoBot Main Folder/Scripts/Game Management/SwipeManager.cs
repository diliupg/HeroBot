using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public static bool tap; // swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging = false;
    public static Vector2 startTouch, swipeDelta;
    public float swipeThreshold;

    private void Update ( )
    {
        tap  = false;

        #region Standalone Inputs

#if ( UNITY_EDITOR )

        if ( Input.GetMouseButtonDown ( 0 ) )
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if ( Input.GetMouseButtonUp ( 0 ) )
        {
            isDraging = false;
            Reset ( );
        }
#endif

        #endregion

        #region Mobile Input

        if ( Input.touches.Length > 0 )
        {
            if ( Input.touches [ 0 ].phase == TouchPhase.Began )
            {
                tap = true;
                isDraging = true;
                startTouch = Input.touches [ 0 ].position;
            }
            else if ( Input.touches [ 0 ].phase == TouchPhase.Ended || Input.touches [ 0 ].phase == TouchPhase.Canceled )
            {
                isDraging = false;
                Reset ( );
            }
        }
        #endregion
    }

    private void Reset ( )
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
