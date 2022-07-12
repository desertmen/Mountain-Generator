using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  class generating meshChunks of mountain object
/// </summary>
public class MeshGenerator : MonoBehaviour
{
    MeshFilter meshFilter;
    [HideInInspector]
    public AnimationCurve curve;
    [HideInInspector]
    public int MountainHeight;
    
    // generates a single chunk of mountain terrain
    public void GenerateSquareMesh(float[,] noiseMap, Vector3 center, Gradient gradient, int size, int vertexCountMultiplier, int XpositionInGrid, int YpositionInGrid)
    {
        meshFilter = GetComponent<MeshFilter>();
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        Mesh mesh = new Mesh();

        if (size <= 0)
        {
            size = 1;
        }

        int verticiesInLine = vertexCountMultiplier * size + 1;
        Vector3[,] extendedVerticiesGrid = CreateExtendedVerticiesGrid(noiseMap, center, size, verticiesInLine, XpositionInGrid, YpositionInGrid);
        int[,] extendedVerticiesIndiciesGrid = CreateExtendedVerticesIndicies(verticiesInLine);

        mesh.vertices = getVerticiesFromGrid(extendedVerticiesGrid); 
        mesh.triangles = GenerateTriangles(size, vertexCountMultiplier);
        mesh.uv = GenerateUVs(size, vertexCountMultiplier);
        mesh.colors = GenerateColors(noiseMap, gradient, center, verticiesInLine, XpositionInGrid, YpositionInGrid);
        mesh.normals = GenerateNormals(extendedVerticiesGrid, extendedVerticiesIndiciesGrid);
        
        meshFilter.sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public Vector2[] GenerateUVs(int size, int vertexCountMultiplier)
    {
        Vector2[] UVs = new Vector2[(size * vertexCountMultiplier + 1) * (size * vertexCountMultiplier + 1)];
        float step = 1f / vertexCountMultiplier;
        int index = 0;
        for (float y = 0; y < size + step; y += step)
        {
            for (float x = 0; x < size + step; x += step)
            {
                float xUV = Mathf.InverseLerp(0, size, x);
                float yUV = Mathf.InverseLerp(0, size, y);
                UVs[index] = new Vector2(xUV, yUV);
                index++;
            }
        }

        return UVs;
    }

    public Color[] GenerateColors(float[,] noiseMap, Gradient gradient, Vector3 center, int vertsInLine, int XchunkPosition, int YchunkPosition)
    {
        Color[] colors = new Color[vertsInLine * vertsInLine];
        int startXNoiseCoords = XchunkPosition * vertsInLine - XchunkPosition;
        int startYnoiseCoords = YchunkPosition * vertsInLine - YchunkPosition;

        int index = 0;
        for (int y = 0; y < vertsInLine; y++)
        {
            for (int x = 0; x < vertsInLine; x++)
            {
                colors[index] = gradient.Evaluate(noiseMap[x + startXNoiseCoords + 1, y + startYnoiseCoords + 1]);
                index++;
            }
        }
        return colors;
    }

    public int[] GenerateTriangles(int size, int vertexCountMultiplier)
    {
        int[] triangles = new int[(size * vertexCountMultiplier) * (size * vertexCountMultiplier) * 6];
        int index = 0;
        for (int y = 0; y < size * vertexCountMultiplier; y++)
        {
            for (int x = 0; x < size * vertexCountMultiplier; x++)
            {
                int point = y * (size * vertexCountMultiplier + 1) + x;

                triangles[index] = point + size * vertexCountMultiplier + 1;
                triangles[index + 1] = point + 1;
                triangles[index + 2] = point;

                triangles[index + 3] = point + 1;
                triangles[index + 4] = point + size * vertexCountMultiplier + 1;
                triangles[index + 5] = point + size * vertexCountMultiplier + 2;

                index += 6;
            }
        }
        return triangles;
    }

    public int[,] CreateExtendedVerticesIndicies(int RealVerticiesInLineCount)
    {
        int[,] extendedVerticiesIncicies = new int[RealVerticiesInLineCount + 2, RealVerticiesInLineCount + 2];
        int realVertIndex = 0;
        int outsideBorderVertIndex = -1;
        for (int y = 0; y < RealVerticiesInLineCount + 2; y++)
        {
            for (int x = 0; x < RealVerticiesInLineCount + 2; x++)
            {
                bool isOutsideBorder = x <= 0 || x > RealVerticiesInLineCount || y <= 0 || y > RealVerticiesInLineCount;
                if (isOutsideBorder)
                {
                    extendedVerticiesIncicies[x, y] = outsideBorderVertIndex;
                    outsideBorderVertIndex--;
                }
                else
                {
                    extendedVerticiesIncicies[x, y] = realVertIndex;
                    realVertIndex++;
                }
            }
        }
        return extendedVerticiesIncicies;
    }

    public Vector3[,] CreateExtendedVerticiesGrid(float[,] noiseMap, Vector3 center, int size, int RealVerticiesInLineCount, int XchunkGridPosition, int YchunkGridPosition)
    {
        Vector3[,] extendedMeshVerticies = new Vector3[RealVerticiesInLineCount + 2, RealVerticiesInLineCount + 2];

        float step = size / (float) (RealVerticiesInLineCount - 1);
        int startXNoiseCoords = XchunkGridPosition * RealVerticiesInLineCount - XchunkGridPosition;
        int startYnoiseCoords = YchunkGridPosition * RealVerticiesInLineCount - YchunkGridPosition;

        for (int y = 0; y < RealVerticiesInLineCount + 2; y++)
        {
            for (int x = 0; x < RealVerticiesInLineCount + 2; x++)
            {
                float height = center.y + noiseMap[x + startXNoiseCoords, y + startYnoiseCoords] * MountainHeight * curve.Evaluate(noiseMap[x + startXNoiseCoords, y + startYnoiseCoords]);
                float Xposition = center.x - size / 2f + (x - 1) * step;
                float Yposition = center.z - size / 2f + (y - 1) * step;
                extendedMeshVerticies[x, y] = new Vector3(Xposition, height, Yposition);
            }
        }
        return extendedMeshVerticies;
    }

    public Vector3[] getVerticiesFromGrid(Vector3[,] extendedVerticiesGrid)
    {
        int realVertsInLine = (extendedVerticiesGrid.GetLength(0) - 2);
        Vector3[] verticies = new Vector3[realVertsInLine * realVertsInLine];

        int index = 0;
        for (int y = 0; y < realVertsInLine + 2; y++)
        {
            for(int x = 0; x < realVertsInLine + 2; x++)
            {
                if (x > 0 && x < realVertsInLine + 1 && y > 0 && y < realVertsInLine + 1)
                {
                    verticies[index] = extendedVerticiesGrid[x, y];
                    index++;
                }
            }
        }
        return verticies;
    }
    // calculates normals accounting for vertices outside this mesh chunk so there is no hard line between chunks
    public Vector3[] GenerateNormals(Vector3[,] extendedVerticiesGrid, int[,] verticiesIndiciesGrid)
    {
        int vertsInLine = extendedVerticiesGrid.GetLength(0) - 2;
        Vector3[] normals = new Vector3[vertsInLine * vertsInLine];

        for(int y = 0; y < vertsInLine + 1; y++)
        {
            for(int x = 0; x < vertsInLine + 1; x++)
            {
                Vector3 vert1 = extendedVerticiesGrid[x, y];
                Vector3 vert2 = extendedVerticiesGrid[x + 1, y];
                Vector3 vert3 = extendedVerticiesGrid[x, y + 1];
                Vector3 vert4 = extendedVerticiesGrid[x + 1, y + 1];

                Vector3 right = vert2 - vert1;
                Vector3 up = vert3 - vert1;
                Vector3 diag = vert4 - vert1;

                Vector3 normalUD = Vector3.Cross(up, diag);
                Vector3 normalDR = Vector3.Cross(diag, right);

                if(verticiesIndiciesGrid[x, y] >= 0)
                {
                    normals[(y - 1) * vertsInLine + x - 1] += normalUD;
                    normals[(y - 1) * vertsInLine + x - 1] += normalDR;
                }
                if (verticiesIndiciesGrid[x + 1, y] >= 0)
                {
                    normals[(y - 1) * vertsInLine + x] += normalDR;
                }
                if (verticiesIndiciesGrid[x, y + 1] >= 0)
                {
                    normals[y * vertsInLine + x - 1] += normalUD;
                }
                if (verticiesIndiciesGrid[x + 1, y + 1] >= 0)
                {
                    normals[y * vertsInLine + x] += normalUD;
                    normals[y * vertsInLine + x] += normalDR;
                }
            }
        }

        for(int i = 0; i < normals.Length; i++)
        {
            normals[i].Normalize();
        }

        return normals;
    }
}
