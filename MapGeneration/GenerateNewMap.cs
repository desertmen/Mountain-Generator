using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  class handling which map is main and currently worked with
/// </summary>
public class GenerateNewMap : MonoBehaviour
{
    MapProperties mapProps;

    void Start()
    {
        mapProps = GetComponent<MapProperties>();
    }

    public void GenerateMap()
    {
        int mapLen = mapProps.maps.Length;
        GameObject currentMap;
        for (int i = 0; i < mapLen; i++)
        {
            // map indexes update after -> index in front is 2
            if(mapProps.maps[i].Index == 2)
            {
                currentMap = mapProps.maps[i].Map;
                currentMap.GetComponent<MapGenerator>().GenerateMap();
            }
        }        
    }
}
