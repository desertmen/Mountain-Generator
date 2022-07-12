using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseDrawerV2 : MonoBehaviour
{
    public Renderer textureRenderer;

    public string DrawNoise(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Debug.Log(width);
        Debug.Log(height);
        Texture2D texture = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        byte[] rawTextureData = texture.GetRawTextureData();

        System.IO.FileInfo fileInfo = new System.IO.FileInfo(Application.persistentDataPath+"/GeneratedHeightMap");
        System.IO.FileStream stream = fileInfo.Create();

        stream.Write(rawTextureData, 0, rawTextureData.Length);
        stream.Close();

        if(textureRenderer != null)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
        }

        return fileInfo.DirectoryName;
    }
}
