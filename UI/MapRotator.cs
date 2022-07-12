using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotator : MonoBehaviour
{
    public AnimationCurve speedCurve;
    public float speed;
    public float mapRotationRadius;
    public float rotationOffset;

    MapProperties mapPropertiesClass;

    float angle;    
    float finalAngle;
    float fullRotationAngle;

    int rotationDirection = 1;
    bool move = false;

    private void Awake()
    {
        mapPropertiesClass = GetComponent<MapProperties>();
        mapPropertiesClass.getMapsProperties();
        fullRotationAngle = 360 / transform.childCount;
        angle = rotationOffset;
        updateAngle();
    }

    public void changeMapInFront()
    {
        int mapsLen = mapPropertiesClass.maps.Length;
        for(int i = 0; i < mapsLen; i++)
        {
            if (mapPropertiesClass.maps[i].IsInFront)
            {
                mapPropertiesClass.maps[i].IsInFront = false;
            }
            if(mapPropertiesClass.maps[i].Index == mapsLen - 1)
            {
                mapPropertiesClass.maps[i].IsInFront = true;
            }
        }
    }

    public void moveIndexOfMaps(int indexIncrement)
    {
        int mapsLen = mapPropertiesClass.maps.Length;
        for (int i = 0; i < mapsLen; i++)
        {
            mapPropertiesClass.maps[i].Index += (int) Mathf.Sign(indexIncrement);
            if(mapPropertiesClass.maps[i].Index >= mapsLen)
            {
                mapPropertiesClass.maps[i].Index -= mapsLen;
            }
            if(mapPropertiesClass.maps[i].Index < 0)
            {
                mapPropertiesClass.maps[i].Index += mapsLen;
            }
        }
    }

    public int findArrayIndexOfMapInFront()
    {
        for(int i = 0; i < mapPropertiesClass.maps.Length; i++)
        {
            if (mapPropertiesClass.maps[i].IsInFront)
            {
                return i;
            }
        }
        return -1;
    }

    public void rotateCounterClockWise()
    {
        if(move == true)
        {
            return;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        if (angle < 0)
        {
            angle += 360;
        }
        move = true;
        finalAngle = angle + fullRotationAngle;
        rotationDirection = 1;
        moveIndexOfMaps(1);
        changeMapInFront();
    }

    public void rotateClockWise()
    {
        if (move == true)
        {
            return;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        if (angle < 0)
        {
            angle += 360;
        }
        move = true;
        finalAngle = angle - fullRotationAngle;
        rotationDirection = -1;
        moveIndexOfMaps(-1);
        changeMapInFront();
    }

    private void Update()
    {
        if(move == false)
        {
            return;
        }

        angle += rotationDirection * 0.1f;
        float angleChange = speed * Time.deltaTime * speedCurve.Evaluate(Mathf.InverseLerp(0, fullRotationAngle, fullRotationAngle - angle));
        angleChange *= rotationDirection;

        if(rotationDirection == 1 && angle + angleChange < finalAngle)
        {
            updateAngle();
            angle += angleChange;
        }
        else if (rotationDirection == -1 && angle + angleChange > finalAngle)
        {
            updateAngle();
            angle += angleChange;
        }
        else
        {
            angle = finalAngle;
            updateAngle();
            move = false;
        }
    }

    public void updateAngle()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            float localAngle = i * fullRotationAngle + angle;
            float x = gameObject.transform.position.x + mapRotationRadius * Mathf.Cos(localAngle * Mathf.Deg2Rad);
            float z = gameObject.transform.position.z + mapRotationRadius * Mathf.Sin(localAngle * Mathf.Deg2Rad);
            Transform child = gameObject.transform.GetChild(i);
            child.position = new Vector3(x, child.position.y, z);
        }
    }
}
