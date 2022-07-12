using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWallGenertor : MonoBehaviour
{
    public float baseYcoord;
    public Vector2 wallOffset;
    
    //TODO FIX triangles or verts?
    public Mesh generateSideWallMesh(GameObject[,] chunkGrid)
    {
        Mesh sideWallMesh = new Mesh();
        Vector3 [] meshBorderVertices = getMeshBorderVerices(chunkGrid);
        Vector3[] baseVertices = generateBaseVertices(meshBorderVertices);
        Vector3[] wallVertices = new Vector3[meshBorderVertices.Length + baseVertices.Length];
        for(int i = 0; i < meshBorderVertices.Length; i++)
        {
            wallVertices[i] = meshBorderVertices[i];
        }
        int index = 0;
        for(int i = meshBorderVertices.Length; i < wallVertices.Length; i++)
        {
            wallVertices[i] = baseVertices[index];
            index++;
        }

        sideWallMesh.vertices = wallVertices;
        sideWallMesh.triangles = generateWallTriangles(meshBorderVertices, baseVertices);
        sideWallMesh.uv = generateUVs(meshBorderVertices.Length);
        sideWallMesh.RecalculateNormals();

        return sideWallMesh;
    }

    Vector2[] generateUVs(int borderLength)
    {
        Vector2[] UVs = new Vector2[borderLength * 2];
        float uvCoord = 0;
        float uvIncrement = 1 / borderLength;
        for(int i = 0; i < borderLength; i++)
        {
            UVs[i] = new Vector2(uvCoord, 1);
            uvCoord += uvIncrement;
        }

        uvCoord = 0;
        for (int i = borderLength; i < borderLength * 2; i++)
        {
            UVs[i] = new Vector2(uvCoord, 0);
            uvCoord += uvIncrement;
        }
        return UVs;
    }

    int[] generateWallTriangles(Vector3[] meshBorderVertices, Vector3[] baseVertices)
    {
        int borderLength = meshBorderVertices.Length;
        int[] triangles = new int[borderLength * 2 * 3];
        int index = 0;
        for(int i = 0; i < borderLength - 1; i++)
        {
            triangles[index] = i;
            triangles[index + 1] = i + 1;
            triangles[index + 2] = i + borderLength + 1;

            triangles[index + 3] = i;
            triangles[index + 4] = i + borderLength + 1;
            triangles[index + 5] = i + borderLength;

            index += 6;
        }
        //create last 2 triangles to complete wall (last to first)
        triangles[index] = borderLength - 1;
        triangles[index + 1] = 0;
        triangles[index + 2] = borderLength;

        triangles[index + 3] = borderLength - 1;
        triangles[index + 4] = borderLength;
        triangles[index + 5] = borderLength * 2 -1;

        return triangles;
    }

    Vector3[] generateBaseVertices(Vector3[] meshBorderVertices)
    {
        Vector3[] baseVertices = new Vector3[meshBorderVertices.Length];
        for(int i = 0; i < baseVertices.Length; i++)
        {
            baseVertices[i] = meshBorderVertices[i];
            baseVertices[i].y = baseYcoord;
        }
        return baseVertices;
    }

    //TODO create vector3[] with border verts
    Vector3[] getMeshBorderVerices(GameObject[,] chunkGrid)
    {
        int chunkGridSize = chunkGrid.GetLength(0);
        int verticesCountInChunk = chunkGrid[0, 0].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
        int verticesInLineInChunk = (int) Mathf.Sqrt(verticesCountInChunk);
        int borderChunksCount = (chunkGridSize - 2) * 4 + 4;
        float chunkSize = chunkGrid[0, 0].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;

        List<Vector3> borderVertices = new List<Vector3>();
        Vector2[] borderMeshesIndicies = generateBorderMeshIndicies(chunkGridSize, borderChunksCount);

        foreach(Vector2 borderMeshIndex in borderMeshesIndicies)
        {
            int x = (int)borderMeshIndex.x;
            int y = (int)borderMeshIndex.y;
            Vector3[] vertices = chunkGrid[x, y].GetComponent<MeshFilter>().sharedMesh.vertices;
            //bottom meshes
            if (y == 0)
            {
                Vector3[] bottomBorder = getBottomBorder(vertices, verticesInLineInChunk, chunkSize, x, y, chunkGridSize);
                if (borderVertices.Count > 0)
                {
                    if (bottomBorder[0] == borderVertices[borderVertices.Count - 1])
                    {
                        borderVertices.RemoveAt(borderVertices.Count - 1);
                    }
                }
                foreach (Vector3 vertex in bottomBorder)
                {
                    borderVertices.Add(vertex);
                }
            }
            // right side meshes
            if(x == chunkGridSize - 1)
            {
                Vector3[] rightBorder = getRightBorder(vertices, verticesInLineInChunk, chunkSize, x, y, chunkGridSize);
                if(rightBorder[0] == borderVertices[borderVertices.Count - 1])
                {
                    borderVertices.RemoveAt(borderVertices.Count - 1);
                }
                foreach(Vector3 vertex in rightBorder)
                {
                    borderVertices.Add(vertex);
                }
            }
            // top meshes
            if (y == chunkGridSize - 1)
            {
                Vector3[] topBorder = getTopBorder(vertices, verticesInLineInChunk, chunkSize, x, y, chunkGridSize);
                if (topBorder[0] == borderVertices[borderVertices.Count - 1])
                {
                    borderVertices.RemoveAt(borderVertices.Count - 1);
                }
                foreach (Vector3 vertex in topBorder)
                {
                    borderVertices.Add(vertex);
                }
            }
            //left side meshes
            if (x == 0 && y != 0)
            {
                Vector3[] leftBorder = getLeftBorder(vertices, verticesInLineInChunk, chunkSize, x, y, chunkGridSize);
                if (leftBorder[0] == borderVertices[borderVertices.Count - 1])
                {
                    borderVertices.RemoveAt(borderVertices.Count - 1);
                }
                foreach (Vector3 vertex in leftBorder)
                {
                    borderVertices.Add(vertex);
                }
            }
        }

        Vector3[] lastVertices = chunkGrid[0, 0].GetComponent<MeshFilter>().sharedMesh.vertices;
        Vector3[] lastLeftBorder = getLeftBorder(lastVertices, verticesInLineInChunk, chunkSize, 0, 0, chunkGridSize);
        if (lastLeftBorder[0] == borderVertices[borderVertices.Count - 1])
        {
            borderVertices.RemoveAt(borderVertices.Count - 1);
        }
        foreach (Vector3 vertex in lastLeftBorder)
        {
            borderVertices.Add(vertex);
        }

        // remove duplicate of start and end point
        borderVertices.RemoveAt(borderVertices.Count - 1);

        return borderVertices.ToArray();
    }

    Vector2[] generateBorderMeshIndicies(int chunkGridSize, int borderChunksCount)
    {
        Vector2[] borderMeshesIndicies = new Vector2[borderChunksCount];

        //bottom border meshes
        int index = 0;
        for(int i = 0; i < chunkGridSize; i++)
        {
            borderMeshesIndicies[index] = new Vector2(i, 0);
            index++;
        }
        //right border meshes
        for(int i = 1; i < chunkGridSize; i++)
        {
            borderMeshesIndicies[index] = new Vector2(chunkGridSize - 1, i);
            index++;
        }
        //top border meshes
        for(int i = chunkGridSize - 2; i >= 0; i--)
        {
            borderMeshesIndicies[index] = new Vector2(i, chunkGridSize - 1);
            index++;
        }
        //left border meshes
        for(int i = chunkGridSize - 2; i > 0; i--)
        {
            borderMeshesIndicies[index] = new Vector2(0, i);
            index++;
        }

        return borderMeshesIndicies;
    }

    Vector3[] getBottomBorder(Vector3[] vertices, int verticesInLine, float chunkSize, int x, int y, int chunksInLine) // from left to right
    {
        Vector3[] bottomBorder = new Vector3[verticesInLine];
        Vector3 offSet = new Vector3((-chunksInLine / 2f + x + wallOffset.x) * chunkSize, 0, (-chunksInLine / 2f + y + wallOffset.y) * chunkSize);
        for (int i = 0; i <verticesInLine; i++)
        {
            bottomBorder[i] = vertices[i] + offSet;
        }
        return bottomBorder;
    }

    Vector3[] getRightBorder(Vector3[] vertices, int verticesInLine, float chunkSize, int x, int y, int chunksInLine) // from bottom to top
    {
        Vector3[] rightBorder = new Vector3[verticesInLine];
        Vector3 offSet = new Vector3((-chunksInLine / 2f + x + wallOffset.x) * chunkSize, 0, (-chunksInLine / 2f + y + wallOffset.y) * chunkSize);
        int index = 0;
        for (int i = verticesInLine - 1; i < vertices.Length; i += verticesInLine)
        {
            rightBorder[index] = vertices[i] + offSet;
            index++;
        }
        return rightBorder;
    }

    Vector3[] getTopBorder(Vector3[] vertices, int verticesInLine, float chunkSize, int x, int y, int chunksInLine) // from right to left
    {
        Vector3[] topBorder = new Vector3[verticesInLine];
        Vector3 offSet = new Vector3((-chunksInLine / 2f + x + wallOffset.x) * chunkSize, 0, (-chunksInLine / 2f + y + wallOffset.y) * chunkSize);
        int index = 0;
        for (int i = verticesInLine * verticesInLine - 1; i >= verticesInLine * (verticesInLine - 1); i--)
        {
            topBorder[index] = vertices[i] + offSet;
            index++;
        }
        return topBorder;
    }

    Vector3[] getLeftBorder(Vector3[] vertices, int verticesInLine, float chunkSize, int x, int y, int chunksInLine) // from top to bottom
    {
        Vector3[] leftBorder = new Vector3[verticesInLine];
        Vector3 offSet = new Vector3((-chunksInLine / 2f + x + wallOffset.x) * chunkSize, 0, (-chunksInLine / 2f + y + wallOffset.y) * chunkSize);
        int index = 0;
        for (int i = verticesInLine * (verticesInLine - 1); i >= 0; i -= verticesInLine)
        {
            leftBorder[index] = vertices[i] + offSet;
            index++;
        }
        return leftBorder;
    }
}
