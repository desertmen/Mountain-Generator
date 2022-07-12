using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script handeling creation of peak indicator and its interactions with mouse
/// </summary>
public class PeakIndicatorDrawer : MonoBehaviour
{
    public int lineLength;
    public Color colorOnMouseHover;

    Vector3 defaulCamPosition;
    Color defaulColor;
    LineRenderer lineRenderer;
    // creates mesh collider used to detect mouse interactions with drawn peak
    public void DrawLine()
    {
        defaulCamPosition = Camera.main.transform.position;
        lineRenderer = GetComponent<LineRenderer>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        lineRenderer.transform.parent = gameObject.transform;
        Vector3 position = Vector3.zero;
        Vector3 pos1 = position;
        pos1.y -= 1;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pos1);
        Vector3 pos2 = position;
        pos2.y += lineLength;
        lineRenderer.SetPosition(1, pos2);

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];
        int index = 0;
        for(float y = pos1.y; y <= pos2.y; y += pos2.y-pos1.y)
        {
            for (float z = pos1.z - 0.5f; z <= pos1.z + 0.5f; z += 1)
            {
                for (float x = pos1.x - 0.5f; x <= pos1.x + 0.5f; x += 1)
                {
                    vertices[index] = new Vector3(x, y, z);
                    index++;
                }
            }
        }
        int[] triangles = new int[] {0, 1, 3, 0, 3, 2, 0, 4, 5, 0, 5, 1, 0, 2, 4, 2, 6, 4, 2, 3, 6, 3, 7, 6, 1, 5, 3, 3, 5, 7, 4, 6, 5, 6, 7, 5};
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnMouseEnter()
    {
        if(lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        defaulColor = lineRenderer.material.GetColor("_Color");
        lineRenderer.material.SetColor("_Color", colorOnMouseHover);
    }

    private void OnMouseExit()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.material.SetColor("_Color", defaulColor);
    }
    // moves camera to look at peak closely
    private void OnMouseDown()
    {
        // save chosen peak to mapHandler
        GetComponent<ChoosePeak>().saveChosenPeak();

        // zoom camera to the chosen peak
        Vector3 position = gameObject.transform.position;
        position += Vector3.Normalize(Camera.main.transform.position - position) * 10;
        position.y += 3;

        CameraMovement camMovement = Camera.main.GetComponent<CameraMovement>();
        camMovement.moveCamToPosition(position);

        // change ui
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach(Canvas canvas in canvases)
        {
            if(canvas.gameObject.name == "Close up camera UI")
            {
                canvas.GetComponent<CanvasHandler>().fadeUIin();
            }
            if(canvas.gameObject.name == "Main Menu UI")
            {
                canvas.GetComponent<CanvasHandler>().fadeUIAway();
            }
        }
    }


}
