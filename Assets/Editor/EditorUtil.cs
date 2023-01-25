using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorUitl
{
    public static string ToTimeDisplay(this float t)
    {
        float min = Mathf.Floor(t / 60f);

        float sec = Mathf.Floor(t - (min * 60f));

        float fraction = (t - Mathf.Floor(t))*1000f;

        //return $"{min:00}:{sec:00}:{fraction:000}";
        return $"{min:00}:{sec:00}:{fraction:000}";

    }
}
