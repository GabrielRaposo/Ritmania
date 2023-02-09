using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlatformState 
{
    Any,     // para ajudar a testar no editor sem afetar o fluxograma final do código 
    Desktop, 
    Mobile
}

public enum InputRegion { Any, Left, Right }

public class PlayerInputReader : MonoBehaviour
{
    const float SWIPE_THRESHOLD = -.5f;
    const float SWIPE_MAX_DELAY = .1f;

    static KeyCode CenterHitKey = KeyCode.Space;
    static KeyCode LeftHitKey   = KeyCode.LeftArrow;
    static KeyCode RightHitKey  = KeyCode.RightArrow;
    static KeyCode StartKey     = KeyCode.Return;
 
    static PlatformState platformState = PlatformState.Any;
    public static bool OnDesktop => platformState == PlatformState.Any || platformState == PlatformState.Desktop;
    public static bool OnMobile  => platformState == PlatformState.Any || platformState == PlatformState.Mobile;

    public static UnityAction StartInputEvent;
    public static UnityAction AnyHitEvent;
    public static UnityAction LeftHitEvent;
    public static UnityAction RightHitEvent;

    class FingerOrigin 
    {
        public FingerOrigin (int id, Vector2 origin)
        {
            this.id = id;
            this.origin = origin;
        }

        public int id;
        public Vector2 origin;
    }

    List<FingerOrigin> touchesOrigins;
    List<float> swipeCountList;

    private void Awake() 
    {
        // Reset input events every time the scene is reloaded
        StartInputEvent = null;
        AnyHitEvent     = null;
        LeftHitEvent    = null;
        RightHitEvent   = null;
    }

    private void Start() 
    {
        touchesOrigins = new List<FingerOrigin>();
        swipeCountList = new List<float>();
    }

    private void LateUpdate() 
    {
        ReadStartInput();
        ReadAnyHitInput();
        ReadLeftHitInput();
        ReadRightHitInput();
    }

    #region Start Input
    private void ReadStartInput() 
    {
        if (OnDesktop)
        {
            if (Input.GetKeyDown(StartKey))
            {
                StartInputEvent?.Invoke();
                return;
            }
        }

        if (OnMobile)
        {
            if (MobileSwipeInputLogic())
                StartInputEvent?.Invoke();
        }
    }

    private bool MobileSwipeInputLogic()
    { 
        // Printing touch points for testing purposes
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (touchesOrigins == null || touchesOrigins.Count < 1)
                break;

            Touch touch = Input.touches[i];
            FingerOrigin fingerOrigin = touchesOrigins.Find( (fOrigin) => fOrigin.id == touch.fingerId );
            if (fingerOrigin == null)
                continue;

            Ray rayA = Camera.main.ScreenPointToRay (fingerOrigin.origin);
            Ray rayB = Camera.main.ScreenPointToRay (touch.position);
            Debug.DrawLine(rayA.origin, rayB.origin, Color.yellow);
        }

        // Adds touch origin points
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];

            if (touch.phase == TouchPhase.Began)
                touchesOrigins.Add( new FingerOrigin(touch.fingerId, touch.position) );
        }

        // When a valid swipe is detected, adds it to the SwipeCountList
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector2 swipeGesture = ReadSwipeInput( Input.touches[i] );
                if (swipeGesture != Vector2.zero)
                    swipeCountList.Add(SWIPE_MAX_DELAY);

                //Debug.Log($"{i} - swipeGesture: {swipeGesture}");
            }
        }

        // Countdown the time on the SwipeCountList. If any reaches 0: it's removed from the list.
        // The longer the SWIPE_MAX_DELAY is, the higger the leniency is for unsynced swipes
        for (int i = swipeCountList.Count - 1; i > -1; i--)
        {
            swipeCountList[i] -= Time.deltaTime;
            if (swipeCountList[i] <= 0)
                swipeCountList.RemoveAt(i);
        }

        // Removes touch origin points
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                //Debug.Log($"-- Remove {touch.fingerId} - {touch.phase}");
                touchesOrigins.RemoveAll( (fOrigin) => fOrigin.id == touch.fingerId );
            }
        }

        // If identified at least two valid swipes, call action
        if (swipeCountList.Count >= 2)
        {
            swipeCountList = new List<float>();
            return true;
        }

        return false;
    }

    private Vector2 ReadSwipeInput(Touch touch)
    {
        // If the finger isn't being released, return
        if (touch.phase != TouchPhase.Ended)
            return Vector2.zero;

        // If the finger being checked wasn't there on the previous frame, return
        FingerOrigin fingerOrigin = touchesOrigins.Find( (fOrigin) => fOrigin.id == touch.fingerId );
        if (fingerOrigin == null)
            return Vector2.zero;

        // If the swipe was "far enough", returns 'movement'
        Vector2 movement = (touch.position - fingerOrigin.origin).normalized;
        if (movement.y < SWIPE_THRESHOLD)
            return movement;

        return Vector2.zero;
    }
    #endregion

    private void ReadAnyHitInput()
    {
        if (OnDesktop)
        {
            if (Input.GetKeyDown(CenterHitKey) || Input.GetKeyDown(LeftHitKey) || Input.GetKeyDown(RightHitKey))
            {
                AnyHitEvent?.Invoke();
                return;
            }
        }

        if (OnMobile)
        {
            if (Input.touchCount < 1)
                return;

            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    AnyHitEvent?.Invoke();
                    return;
                }
            }
        }
    }

    private void ReadLeftHitInput()
    {
        if (OnDesktop)
        {
            if (Input.GetKeyDown(LeftHitKey))
            {
                LeftHitEvent?.Invoke();
                return;
            }
        }

        if (OnMobile)
        {
            if (Input.touchCount < 1)
                return;

            foreach (Touch touch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (touch.phase == TouchPhase.Began && ray.origin.x < 0)
                {
                    LeftHitEvent?.Invoke();
                    return;
                }
            }
        }
    }

    private void ReadRightHitInput()
    {
        if (OnDesktop)
        {
            if (Input.GetKeyDown(RightHitKey))
            {
                RightHitEvent?.Invoke();
                return;
            }
        }

        if (OnMobile)
        {
            if (Input.touchCount < 1)
                return;

            foreach (Touch touch in Input.touches)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (touch.phase == TouchPhase.Began && ray.origin.x > 0)
                {
                    RightHitEvent?.Invoke();
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos() 
    {
        if (touchesOrigins == null || touchesOrigins.Count < 1)
            return;

        Gizmos.color = Color.yellow;
        foreach (FingerOrigin fOrigin in touchesOrigins)
        {
            Ray ray = Camera.main.ScreenPointToRay(fOrigin.origin);
            Gizmos.DrawWireSphere(ray.origin, .2f);
        }
    }
}
