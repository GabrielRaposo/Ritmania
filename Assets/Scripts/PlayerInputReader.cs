using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformState 
{
    Any,      // para ajudar a testar no editor sem afetar o fluxograma final do código 
    Desktop,
    Mobile
}

public class PlayerInputReader : MonoBehaviour
{
    static KeyCode CenterHitKey = KeyCode.Space;
    static KeyCode LeftHitKey   = KeyCode.LeftArrow;
    static KeyCode RightHitKey  = KeyCode.RightArrow;
    static KeyCode StartKey     = KeyCode.Return;
 
    static PlatformState platformState = PlatformState.Any;

    void Start()
    {
        
    }

    public static bool OnDesktop => platformState == PlatformState.Any || platformState == PlatformState.Desktop;
    public static bool OnMobile  => platformState == PlatformState.Any || platformState == PlatformState.Mobile;

    public static bool StartInput 
    {
        get 
        {
            if (OnDesktop)
            {
                if (Input.GetKeyDown(StartKey))
                    return true;
            }

            if (OnMobile)
            {
                if (Input.touchCount < 1)
                    return false;

                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Ended)
                        return true;
                }
            }
            return false;
        }
    }

    public static bool AnyHitInput
    {
        get 
        {
            if (OnDesktop)
            {
                if (Input.GetKeyDown(CenterHitKey) || Input.GetKeyDown(LeftHitKey) || Input.GetKeyDown(RightHitKey))
                    return true;
            }

            if (OnMobile)
            {
                if (Input.touchCount < 1)
                    return false;

                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                        return true;
                }
            }
            return false;
        }
    }
}
