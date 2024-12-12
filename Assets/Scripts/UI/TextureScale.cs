using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScale
{
    public static Texture2D Bilinear(Texture2D originalTexture, int newWidth, int newHeight)
    {
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight, originalTexture.format, false);

        float scaleX = 1.0f / newWidth;
        float scaleY = 1.0f / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                float gx = x * scaleX * originalTexture.width;
                float gy = y * scaleY * originalTexture.height;
                resizedTexture.SetPixel(x, y, originalTexture.GetPixelBilinear(gx, gy));
            }
        }

        resizedTexture.Apply();
        return resizedTexture;
    }
}