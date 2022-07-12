using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// script controlling movement of camera
/// </summary>
public class CameraMovement : MonoBehaviour
{
    Vector3 defaulCamPosition;
    float time;
    bool isOnChosenPeak = false;

    [SerializeField]
    [Range(1, 10)]
    float speed;

    private void Start()
    {
        defaulCamPosition = gameObject.transform.position;
        time = 1 / speed;
    }

    public void setSpeed(float inSpeed)
    {
        speed = inSpeed;
        time = 1 / inSpeed;
    }

    public void moveCamToPosition(Vector3 position)
    {
        if(isOnChosenPeak)
        {
            return;
        }
        StartCoroutine(moveCamToPos(position, time));
        isOnChosenPeak = true;
    }
    
    public void moveCamToDefaultPos()
    {
        StartCoroutine(moveCamToPos(defaulCamPosition, time));
        isOnChosenPeak = false;
    }
    // moves camera smoothly to desired position in time specified
    IEnumerator moveCamToPos(Vector3 position, float time, float timeStep = 0.02f)
    {
        Vector3 camPosition = gameObject.transform.position;
        float distance = Vector3.Distance(camPosition, position);
        float positionStep = distance * (timeStep / time);

        while (gameObject.transform.position != position)
        {
            camPosition = gameObject.transform.position;
            gameObject.transform.position = Vector3.MoveTowards(camPosition, position, positionStep);
            yield return new WaitForSeconds(timeStep);
        }
    }
}
