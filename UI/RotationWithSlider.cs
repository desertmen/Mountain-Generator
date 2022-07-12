using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationWithSlider : MonoBehaviour
{
    public void sliderRotation(float rotation)
    {
        MapProperties mapProps = GetComponent<MapProperties>();
        if (mapProps.maps == null)
        {
            return;
        }
        for(int i = 0; i < mapProps.maps.Length; i++)
        {
            if(mapProps.maps[i].Index == 2)
            {
                mapProps.maps[i].Map.transform.rotation = Quaternion.Euler(0, -rotation, 0);
            }
        }
    }
}
