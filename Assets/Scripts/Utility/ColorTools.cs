using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorTools 
{
    public static Color AdjustHue(this Color c, float amount)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);

        h += amount;
        
        return Color.HSVToRGB(h, s, v);
    }
    
    public static Color AdjustSaturation(this Color c, float amount)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);

        s += amount;
        
        return Color.HSVToRGB(h, s, v);
    }
    
    public static Color AdjustValue(this Color c, float amount)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);

        v += amount;
        
        return Color.HSVToRGB(h, s, v);
    }
    
}
