using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaposUtil
{
    public static Vector2 AlignWithTransform (Transform t, Vector2 v)
    {
        return (t.right * v.x) + (t.up * v.y);
    }

    public static Vector2 RotateVector (Vector2 v, float a)
    {
        float radians = a * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static double AngleDifference (double angle1, double angle2)
    {
        double diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    public static void Wait (MonoBehaviour script, int frames, UnityAction action) 
    {
        script.StartCoroutine( Wait(frames, action) );
    }

    public static void WaitSeconds (MonoBehaviour script, float duration, UnityAction action) 
    {
        script.StartCoroutine( WaitSeconds(duration, action) );
    }

    public static void WaitSecondsRealtime (MonoBehaviour script, float duration, UnityAction action) 
    {
        script.StartCoroutine( WaitSecondsRealtime(duration, action) );
    }

    public static IEnumerator Wait (int frames, UnityAction action)
    {
        if (frames > 0)
        {
            for (int i = 0; i < frames; i++)
                yield return new WaitForEndOfFrame();
        }

        action?.Invoke();
    }

    public static IEnumerator WaitSeconds (float duration, UnityAction action)
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }

        action?.Invoke();
    }

    public static IEnumerator WaitSecondsRealtime (float duration, UnityAction action)
    {
        if (duration > 0)
        {
            yield return new WaitForSecondsRealtime(duration);
        }

        action?.Invoke();
    }

    public static void ClearLog()
    {
        #if UNITY_EDITOR

            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);

        #endif
    }
}

public static class MonobehaviourExtension
{
    public static void ShowErrorAndDisable (this MonoBehaviour m, string msg)
    {
        Debug.LogError(msg);
        m.enabled = false;
    }

    public static void Wait (this MonoBehaviour m, int frames, UnityAction action)
    {
        RaposUtil.Wait(m, frames, action);
    }

    public static void WaitSeconds (this MonoBehaviour m, float duration, UnityAction action)
    {
        RaposUtil.WaitSeconds(m, duration, action);
    }

    public static void WaitSecondsRealtime (this MonoBehaviour m, float duration, UnityAction action)
    {
        RaposUtil.WaitSecondsRealtime(m, duration, action);
    }
}

public static class Vector2Extension
{
    public static Vector2 SetX (this Vector2 v, float x)
    {
        return new Vector2 (x, v.y);        
    }

    public static Vector2 SetY (this Vector2 v, float y)
    {
        return new Vector2 (v.x, y);        
    }

    public static Vector2 To8Directions(this Vector2 v)
    {
        float threshold = .5f;
        Vector2 result = Vector2.zero;
        if (v.x != 0)
        {
            if (v.x > threshold)
                result = result.SetX (1);
            if (v.x < -threshold)
                result = result.SetX (-1);
        }
        if (v.y != 0)
        {
            if (v.y > threshold)
                result = result.SetY (1);
            if (v.y < -threshold)
                result = result.SetY (-1);
        }
        return result;
    } 

    public static Vector2 To8Normalized(this Vector2 v)
    {
        return v.To8Directions().normalized;
    }
}
public static class Vector4Extension
{
    public static Vector4 SetAlpha (this Vector4 v, float a)
    {
        return new Vector4 (v.x, v.y, v.z, a);        
    }
}

public static class RectTransformExtensions
 {
    public static void MoveX (this RectTransform rt, float x)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        rt.offsetMax = new Vector2(x, rt.offsetMax.y);
    }

    public static void MoveY (this RectTransform rt, float y)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        rt.offsetMax = new Vector2(rt.offsetMax.x, y);
    }

    public static void Move (this RectTransform rt, float x, float y)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        rt.offsetMax = new Vector2(x, rt.offsetMax.y);

        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        rt.offsetMax = new Vector2(rt.offsetMax.x, y);
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
 
    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
 
    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
 
    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
 }

public static class ExtDebug
 {
     //Draws just the box at where it is currently hitting.
     public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
     {
         origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
         DrawBox(origin, halfExtents, orientation, color);
     }
     
     //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
     public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
     {
         direction.Normalize();
         Box bottomBox = new Box(origin, halfExtents, orientation);
         Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);
             
         Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft,    color);
         Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
         Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
         Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight,    color);
         Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft,    color);
         Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
         Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
         Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight,    color);
     
         DrawBox(bottomBox, color);
         DrawBox(topBox, color);
     }
     
     public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
     {
         DrawBox(new Box(origin, halfExtents, orientation), color);
     }
     public static void DrawBox(Box box, Color color)
     {
         Debug.DrawLine(box.frontTopLeft,     box.frontTopRight,    color);
         Debug.DrawLine(box.frontTopRight,     box.frontBottomRight, color);
         Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
         Debug.DrawLine(box.frontBottomLeft,     box.frontTopLeft, color);
                                                  
         Debug.DrawLine(box.backTopLeft,         box.backTopRight, color);
         Debug.DrawLine(box.backTopRight,     box.backBottomRight, color);
         Debug.DrawLine(box.backBottomRight,     box.backBottomLeft, color);
         Debug.DrawLine(box.backBottomLeft,     box.backTopLeft, color);
                                                  
         Debug.DrawLine(box.frontTopLeft,     box.backTopLeft, color);
         Debug.DrawLine(box.frontTopRight,     box.backTopRight, color);
         Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
         Debug.DrawLine(box.frontBottomLeft,     box.backBottomLeft, color);
     }
     
     public struct Box
     {
         public Vector3 localFrontTopLeft     {get; private set;}
         public Vector3 localFrontTopRight    {get; private set;}
         public Vector3 localFrontBottomLeft  {get; private set;}
         public Vector3 localFrontBottomRight {get; private set;}
         public Vector3 localBackTopLeft      {get {return -localFrontBottomRight;}}
         public Vector3 localBackTopRight     {get {return -localFrontBottomLeft;}}
         public Vector3 localBackBottomLeft   {get {return -localFrontTopRight;}}
         public Vector3 localBackBottomRight  {get {return -localFrontTopLeft;}}
 
         public Vector3 frontTopLeft     {get {return localFrontTopLeft + origin;}}
         public Vector3 frontTopRight    {get {return localFrontTopRight + origin;}}
         public Vector3 frontBottomLeft  {get {return localFrontBottomLeft + origin;}}
         public Vector3 frontBottomRight {get {return localFrontBottomRight + origin;}}
         public Vector3 backTopLeft      {get {return localBackTopLeft + origin;}}
         public Vector3 backTopRight     {get {return localBackTopRight + origin;}}
         public Vector3 backBottomLeft   {get {return localBackBottomLeft + origin;}}
         public Vector3 backBottomRight  {get {return localBackBottomRight + origin;}}
 
         public Vector3 origin {get; private set;}
 
         public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
         {
             Rotate(orientation);
         }
         public Box(Vector3 origin, Vector3 halfExtents)
         {
             this.localFrontTopLeft     = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontTopRight    = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontBottomLeft  = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
             this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
 
             this.origin = origin;
         }
 
 
         public void Rotate(Quaternion orientation)
         {
             localFrontTopLeft     = RotatePointAroundPivot(localFrontTopLeft    , Vector3.zero, orientation);
             localFrontTopRight    = RotatePointAroundPivot(localFrontTopRight   , Vector3.zero, orientation);
             localFrontBottomLeft  = RotatePointAroundPivot(localFrontBottomLeft , Vector3.zero, orientation);
             localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
         }
     }
 
      //This should work for all cast types
     static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
     {
         return origin + (direction.normalized * hitInfoDistance);
     }
     
     static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
     {
         Vector3 direction = point - pivot;
         return pivot + rotation * direction;
     }
 }