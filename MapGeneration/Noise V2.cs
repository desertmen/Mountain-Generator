using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// class used to calculate perlin noise with octaves, which is used to generate mountain like terrain
/// </summary>
public static class NoiseV2
{
    // generates height map of size (mapWidth, mapHeight) in range of values from minNoiseHeight to maxNoiseHeight
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, float XoffSet, float Yoffset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        
        if(scale <= 0)
        {
            scale = 0.0001f;
        }
        if(octaves < 1)
        {
            octaves = 1;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + XoffSet;
                    float sampleY = y / scale * frequency + Yoffset;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    // generates rectangular heightmap of size (resolution, resolution) in range of values from minNoiseHeight to maxNoiseHeight
    public static float[,] generateHDNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, float XoffSet, float Yoffset, int resolution)
    {
        float[,] noiseMap = new float[resolution, resolution];
        resolution -= 1;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        int yIndex = 0;
        for (float y = 0; y <= mapHeight; y += (float)mapHeight/(float)resolution)
        {
            int xIndex = 0;
            for (float x = 0; x <= mapWidth; x += (float)mapWidth/(float)resolution)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + XoffSet;
                    float sampleY = y / scale * frequency + Yoffset;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                /*                if(xIndex < noiseMap.GetLength(0) && yIndex < noiseMap.GetLength(1))
                                {
                                    noiseMap[yIndex, xIndex] = noiseHeight;
                                }*/
                noiseMap[yIndex, xIndex] = noiseHeight;

                xIndex++;
            }
            yIndex++;
        }

        for (int y = 0; y < noiseMap.GetLength(0); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(1); x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
