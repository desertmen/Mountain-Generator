using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetSlider : MonoBehaviour
{
    public void resetSlider()
    {
        GetComponent<Slider>().value = 0;
        MapProperties mapProps = FindObjectOfType<MapProperties>();
        for(int i = 0; i < mapProps.maps.Length; i++)
        {
            mapProps.maps[i].Map.transform.rotation = Quaternion.identity;
        }
    }
}
