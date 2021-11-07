using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    // this method allow to create Texture with color map, width and height
    public static Texture2D textureFromColorMap(Color[] colorMap,int width,int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.wrapMode = TextureWrapMode.Clamp;
       // texture.filterMode = FilterMode.Point; 
        texture.Apply();
        return texture;
    }

    // this methid create colormap with heightMap, this color will be between black and white
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] ColorMap = new Color[width * height];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // noise map give a value between 0 and 1 so the lerp can give us a color between black and white
              
                ColorMap[i] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                i++;
            }
        }

        return textureFromColorMap(ColorMap, width, height);
    }

}
