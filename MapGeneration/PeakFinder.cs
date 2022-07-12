using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// class holding methods for finding peaks of mountai and drawing them with peak identified prefab
/// </summary>
public class PeakFinder : MonoBehaviour
{
    // minimal distance from edge peak needs to have to be drawn
    public int distanceFromMapEdge;
    public GameObject peakIdentifier;
    public bool drawPeakIndicators;

    MapGenerator mapGenerator;
    GameObject[,] chunkGrid;
    List<GameObject> peakChunks;
    [HideInInspector]
    List<GameObject> peakIndicators;

    int chunkCountX;
    int chunkCountY;

    [ContextMenu("Find Peaks")]

    public void findPeaks()
    {

        peakChunks = new List<GameObject>();
        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator.chunkGrid == null)
        {
            mapGenerator.GenerateMap();
        }
        if(distanceFromMapEdge < 0)
        {
            distanceFromMapEdge = 0;
        }
        if (distanceFromMapEdge > (mapGenerator.chunkCountInLine - 1) / 2)
        {
            distanceFromMapEdge = (mapGenerator.chunkCountInLine - 1) / 2;
        }
        chunkGrid = mapGenerator.chunkGrid;
        chunkCountX = chunkGrid.GetLength(0);
        chunkCountY = chunkGrid.GetLength(0);

        for(int y = distanceFromMapEdge; y < chunkCountY - distanceFromMapEdge; y++)
        {
            for(int x = distanceFromMapEdge; x < chunkCountX - distanceFromMapEdge; x++)
            {
                if(isPeak(compareToNeighbours(x, y)))
                {
                    peakChunks.Add(chunkGrid[x, y]);
                }
            }
        }

        drawPeaks();

    }

    public void drawPeaks()
    {

        int i = 0;
        while (i < gameObject.transform.childCount)
        {
            if (gameObject.transform.GetChild(i).tag == "Peak Identifier")
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
        /*            foreach (GameObject peakIndicator in GameObject.FindGameObjectsWithTag("Peak Identifier"))
                    {
                        if (Application.isEditor)
                            Object.DestroyImmediate(peakIndicator);
                        else
                            Object.Destroy(peakIndicator);
                    }*/

        peakIndicators = new List<GameObject>();

        foreach (GameObject peak in peakChunks)
        {
            int index = peakIndicators.Count;
            peakIndicators.Add(Instantiate(peakIdentifier, findHighestPoint(peak), Quaternion.identity));
            peakIndicators[index].name = "Peak Indicator";
            peakIndicators[index].tag = "Peak Identifier";
            peakIndicators[index].transform.parent = gameObject.transform;
            peakIndicators[index].GetComponent<PeakIndicatorDrawer>().DrawLine();
        }
    }
    // returns highest point of selected chunk
    public Vector3 findHighestPoint(GameObject chunk)
    {
        Vector3 highestVertex = Vector3.zero;
        Vector3[] vertices = chunk.GetComponent<MeshFilter>().sharedMesh.vertices;
        foreach(Vector3 vertex in vertices)
        {
            if(vertex.y > highestVertex.y)
            {
                highestVertex = vertex + chunk.transform.position;
            }
        }
        return highestVertex;
    }

    // checks how chunk hight compares to its neighbours
    public int[] compareToNeighbours(int chunkGridPositionX, int chunkGridPositionY)
    {
        int[] goesUp = new int[4] {0, 0, 0, 0}; // right, down, left, up (going outwards, 1 - goes up, 0 no chunk, -1 goes down)
        // right
        if(chunkGridPositionX < chunkCountX - 1)
        {
            goesUp[0] = (int) Mathf.Sign(getChunkHight(chunkGridPositionX + 1, chunkGridPositionY) - getChunkHight(chunkGridPositionX, chunkGridPositionY));
        }
        // down
        if (chunkGridPositionY < chunkCountY - 1)
        {
            goesUp[1] = (int)Mathf.Sign(getChunkHight(chunkGridPositionX, chunkGridPositionY + 1) - getChunkHight(chunkGridPositionX, chunkGridPositionY));
        }
        //left
        if (chunkGridPositionX > 1)
        {
            goesUp[2] = (int)Mathf.Sign(getChunkHight(chunkGridPositionX - 1, chunkGridPositionY) - getChunkHight(chunkGridPositionX, chunkGridPositionY));
        }
        // up
        if (chunkGridPositionY > 1)
        {
            goesUp[3] = (int)Mathf.Sign(getChunkHight(chunkGridPositionX , chunkGridPositionY - 1) - getChunkHight(chunkGridPositionX, chunkGridPositionY));
        }
        return goesUp;
    }

    // check if is peak based on comparison of neighbours, returns true if all neighbours are lower
    public bool isPeak(int[] goesUp)
    {
        foreach(int i in goesUp)
        {
            if(i == 1)
            {
                return false;
            }
        }
        return true;
    }
    // returns maximal height of all vertices in chunk
    public float getChunkHight(int chunkGridPositionX, int chunkGridPositionY)
    {
        GameObject currentChunk = chunkGrid[chunkGridPositionX, chunkGridPositionY];
        float chunkHeight = currentChunk.GetComponent<MeshCollider>().bounds.max.y;
        return chunkHeight;
    }
}
