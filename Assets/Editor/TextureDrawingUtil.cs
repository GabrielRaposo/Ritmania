using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TextureDrawingUtil
{
    public static Texture2D GetFill(int width, int height, Color color)
    {
        var t = new Texture2D(width, height, TextureFormat.RGBA32, false);

        t.ColorBlock(width, height, color);

        t.Apply();

        return t;
    }


    public static Texture2D GetArrow(int width, int height, float pointSize, Color color)
    {
        pointSize = Mathf.Clamp01(pointSize);
        
        var t = new Texture2D(width, height, TextureFormat.RGBA32, false);

        int rectangleH = Mathf.FloorToInt((float)height * (1f - pointSize));
        
        t.ColorBlock(width, rectangleH, color, startingY:height-rectangleH);


        for (int i = 0; i < width; i++)
        {
            for (int j = height-rectangleH; j >= 0; j--)
            {
                //Debug.Log($"{j}");
                float l = 1f-Mathf.InverseLerp(height-rectangleH, 0, j);
                int pixelSpaceOnThisLine = Mathf.FloorToInt((width-(width * l ))/2f);
                
                

                if (i >= pixelSpaceOnThisLine && i < width - pixelSpaceOnThisLine)
                {
                    t.SetPixel(i, j, color);
                }
                else
                {
                    //t.SetPixel(i,j,Color.black);
                }
            }
        }
        
        t.Apply();

        return t;

    }
    
    
    
    private static void ColorBlock(this Texture2D t, int width, int height, Color color,
        int startingX = 0, int startingY = 0)
    {
        for (int i = startingX; i < startingX+width; i++)
        {
            for (int j = startingY; j < startingY+height; j++)
            {
                t.SetPixel(i, j, color);
            }
        }
    }
}
