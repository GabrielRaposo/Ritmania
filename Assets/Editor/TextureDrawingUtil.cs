using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TextureDrawingUtil
{
    public static Texture2D GetFill(int width, int height, Color color)
    {
        var t = new Texture2D(width, height, TextureFormat.RGBA32, false);

        t.PaintColorBlock(width, height, color);

        t.Apply();

        return t;
    }

    public static Texture2D GetArrow(int width, int height, float pointSize, Color color)
    {
        pointSize = Mathf.Clamp01(pointSize);
        
        var t = new Texture2D(width, height, TextureFormat.RGBA32, false);

        int rectangleH = Mathf.FloorToInt((float)height * (1f - pointSize));
        
        t.PaintColorBlock(width, rectangleH, color, startingY:height-rectangleH);


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
    public static void OverlayTexture(this Texture2D t, int x, int y, Texture2D overlay, float alpha=1f)
    {
        for (int i = 0; i < overlay.width; i++)
        {
            for (int j = 0; j < overlay.height; j++)
            {
                int posX = x + i;
                int posY = y + j;
                
                if(t.width<=posX)
                    continue;
                
                if(t.height<=posY)
                    continue;

                Color baseColor = t.GetPixel(posX, posY);
                Color overlayColor = overlay.GetPixel(i, j);

                Color newColor = baseColor + overlayColor * alpha;
                
                t.SetPixel(posX, posY, newColor);

            }
        }
        t.Apply();
    }

    public static Texture2D GetNewTintedTexture(Texture2D originalTex, Color color)
    {
        var t = new Texture2D(originalTex.width, originalTex.height, originalTex.format, false);
        
        for (int i = 0; i < originalTex.width; i++)
        {
            for (int j = 0; j < originalTex.height; j++)
            {
                t.SetPixel(i,j, originalTex.GetPixel(i,j)*color);
            }
        }
        
        t.Apply();

        return t;
    }
    
    public static void PaintColorBlock(this Texture2D t, int width, int height, Color color, int startingX = 0, int startingY = 0)
    {
        for (int i = startingX; i < startingX+width; i++)
        {
            for (int j = startingY; j < startingY+height; j++)
            {
                if(i>=t.width)
                    continue;
                
                if(j>=t.height)
                    continue;
                
                t.SetPixel(i, j, color);
            }
        }
        t.Apply();
    }
}
