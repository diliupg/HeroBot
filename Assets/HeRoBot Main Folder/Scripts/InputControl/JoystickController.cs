using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Put it on any Image UI Element
/// </summary>
public class JoystickController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    protected RectTransform Background;
    protected bool Pressed;
    protected int PointerId;
    public RectTransform Handle;
    [Range ( 0f, 2f )]
    public float HandleRange = 1f;

    [HideInInspector] public float directionLR;      //Float that stores horizontal input

    [HideInInspector] public float directionUD;    // float that stores the vertical input

    [HideInInspector] public Vector2 InputVector = Vector2.zero;

    //public Vector2 AxisNormalized { get { return InputVector.magnitude > 0.25f ? InputVector.normalized : ( InputVector.magnitude < 0.01f ? Vector2.zero : InputVector * 4f ); } }

    void OnEnable ( )
    {
        KeyboardController.OnPlayerHorizontal += PlayerHorizontalMove;
    }
    void OnDisable ( )
    {
        KeyboardController.OnPlayerHorizontal -= PlayerHorizontalMove;
    }

#if UNITY_ANDROID
    void Start ( )
    {
        //if ( Handle == null )
        //    Handle = transform.GetChild ( 0 ).GetComponent<RectTransform> ( );
        //Background = GetComponent<RectTransform> ( );
        //Background.pivot = new Vector2 ( 0.5f, 0.5f );
        Pressed = false;
    }

    void Update ( )
    {
        if ( Pressed )
        {
            Vector2 direction = ( PointerId >= 0 && PointerId < Input.touches.Length ) ? Input.touches [ PointerId ].position - new Vector2 ( Background.position.x, Background.position.y ) : new Vector2 ( Input.mousePosition.x, Input.mousePosition.y ) - new Vector2 ( Background.position.x, Background.position.y );
            InputVector = ( direction.magnitude > Background.sizeDelta.x / 2f ) ? direction.normalized : direction / ( Background.sizeDelta.x / 2f );
            Handle.anchoredPosition = ( InputVector * Background.sizeDelta.x / 8f ) * HandleRange;

            if ( InputVector.x < 0 )
                directionLR = -1f;
            else if ( InputVector.x > 0 )
                directionLR = 1f;

            if ( InputVector.y < 0 )
                directionUD = -1f;
            else if ( InputVector.y > 0 )
                directionUD = 1f;
        }
    }

    public void OnPointerDown ( PointerEventData eventData )
    {
        Pressed = true;
        PointerId = eventData.pointerId;
    }

    public void OnPointerUp ( PointerEventData eventData )
    {
        Pressed = false;
        InputVector = Vector2.zero;
        Handle.anchoredPosition = Vector2.zero;
        directionLR = 0;
        directionUD = 0;
    }

#endif
    void PlayerHorizontalMove( int v)
    {
        directionLR = v;
    }
}
