using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// main script generating mountain
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public GameObject chunkPrefab;
    public int chunkCountInLine;
    public int chunkSize;

    public int worldSeed;
    public int MountainHeight;

    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;

    [Range(0,3)]
    public int detailLevel;

    public AnimationCurve curve;
    public Gradient gradient;

    public bool autoUpdate;
    public bool customSeed;

    float Xoffset;
    float Yoffset;
    float[,] noiseMap;
    [HideInInspector]
    public GameObject[,] chunkGrid;
    Vector3 position;
    int mapSize;


    private void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        position = gameObject.transform.position;
        gameObject.transform.position = Vector3.zero;
        if(chunkCountInLine < 1)
        {
            chunkCountInLine = 1;
        }
        if(scale <= 0)
        {
            scale = 0.0001f;
        }
        if (!customSeed)
        {
            worldSeed = (int)Random.Range(-100000, 100000);
        }

        gameObject.transform.localRotation = Quaternion.identity;

        int vertexCountMultiplier = (int)Mathf.Pow(2, detailLevel);
        int verticiesInLineCount = vertexCountMultiplier * chunkSize + 1;
        mapSize = chunkCountInLine * verticiesInLineCount;

        Random.InitState(worldSeed);
        Xoffset = Random.Range(-100000, 100000);
        Yoffset = Random.Range(-100000, 100000);
        // +2 for normal calculations
        noiseMap = NoiseV2.GenerateNoiseMap(mapSize + 2, mapSize + 2, scale * verticiesInLineCount, octaves, persistance, lacunarity, Xoffset, Yoffset);
        GenerateChunks(vertexCountMultiplier);
        findPeaks();
        generateSideWall();
        gameObject.transform.position = position;
    }

    public void generateSideWall()
    {
        SideWallGenertor sideWallGenertor = GetComponent<SideWallGenertor>();
        Mesh wallMesh = sideWallGenertor.generateSideWallMesh(chunkGrid);
        GetComponent<MeshFilter>().sharedMesh = wallMesh;
    }

    public void findPeaks()
    {
        PeakFinder peakFinder = GetComponent<PeakFinder>();
        peakFinder.findPeaks();
    }

    public void GenerateChunks(int vertexCountMultiplier)
    {
        DeletaAllChunks();
        chunkGrid = new GameObject[chunkCountInLine, chunkCountInLine];
        for(int y = 0; y < chunkCountInLine; y++)
        {
            for (int x = 0; x < chunkCountInLine; x++)
            {
                generateChunk(chunkGrid,vertexCountMultiplier ,x, y);
            }
        }
    }

    public void generateChunk(GameObject[,] chunkGrid, int vertexCountMultiplier, int x, int y)
    {
        float start = -(chunkCountInLine * chunkSize - 10) / 2f;
        float chunkYCoord = start / 2f + y * chunkSize / 2f;
        float chunkXCoord = start / 2f + x * chunkSize / 2f;
        Vector3 chunkPosition = new Vector3(chunkXCoord, 0, chunkYCoord);
        chunkGrid[x, y] = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
        chunkGrid[x, y].name = "Chunk";
        chunkGrid[x, y].tag = "Chunk";
        chunkGrid[x, y].transform.parent = gameObject.transform;
        MeshGenerator chunkMeshGenerator = chunkGrid[x, y].GetComponent<MeshGenerator>();
        chunkMeshGenerator.curve = curve;
        chunkMeshGenerator.MountainHeight = MountainHeight;
        chunkMeshGenerator.GenerateSquareMesh(noiseMap, chunkPosition, gradient, chunkSize, vertexCountMultiplier, x, y);
    }

    public void DeletaAllChunks()
    {
        int i = 0;
        while (i < gameObject.transform.childCount)
        {
            if (gameObject.transform.GetChild(i).name == "Chunk")
            {
                // TODO FIX - application iseditor runs in play mode
                if (Application.isEditor)
                {
                    GameObject.DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
                    i--;
                }
                else
                {
                    GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
                    i--;

                }
            }
            i++;
        }
    }

    public float[,] getNoiseMap()
    {
        return noiseMap;
    }

    public float[,] getHDNoiseMap()
    {
        int vertexCountMultiplier = (int)Mathf.Pow(2, detailLevel);
        int verticiesInLineCount = vertexCountMultiplier * chunkSize + 1;
        float[,] HDnoiseMap = NoiseV2.generateHDNoiseMap(mapSize + 2, mapSize + 2, scale * verticiesInLineCount, octaves, persistance, lacunarity, Xoffset, Yoffset, 513);
        return HDnoiseMap;
    }
}
