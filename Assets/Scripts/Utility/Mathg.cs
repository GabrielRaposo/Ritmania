using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathg
{
    public static Vector3 AngleToDirection(float angle)
    {
        return ((Vector3) (Quaternion.Euler(0, 0, angle) * Vector2.right)).normalized;
    }
    
    public static Vector3 AngleToDirection(float angle, Vector2 forward, bool invertRotation)
    {
        return ((Vector3) (Quaternion.Euler(0, 0, angle*(invertRotation?-1:1)) * forward.normalized)).normalized;
    }

    public static float VectorToAngle(Vector3 vector)
    {
        return Vector3.Angle(Vector3.up, vector);
    }



    public static float AngleOfTheLineBetweenTwoPoints(Vector3 start, Vector3 end)
    {
        Vector3 rotationVector = end - start;

        float side = (end.x > start.x) ? -1 : 1;
        
        return VectorToAngle(rotationVector) * side;
    }

    public static float Remap (this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public static float GetFraction(this float f)
    {
        if (f >= 0)
            return f - Mathf.FloorToInt(f);

        else
            return -(Mathf.Abs(f) - Mathf.Abs(Mathf.FloorToInt(f)));

    }
    
}