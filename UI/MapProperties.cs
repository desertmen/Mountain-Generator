using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapProperties : MonoBehaviour
{
    public mapProperties[] maps;

    public mapProperties[] getMapsProperties()
    {
        maps = getMaps();
        setDificulties(maps);
        return maps;
    }

    public struct mapProperties
    {
        public GameObject Map;
        public bool IsInFront;
        public int Index;
        public int Difficulty;
    }

    mapProperties[] getMaps()
    {
        mapProperties[] maps = new mapProperties[gameObject.transform.childCount];
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            mapProperties newMap;
            newMap.Map = gameObject.transform.GetChild(i).gameObject;
            newMap.IsInFront = false;
            newMap.Index = i;
            newMap.Difficulty = -1;
            maps[i] = newMap;
            if(i == gameObject.transform.childCount - 1)
            {
                newMap.IsInFront = true;
            }
        }
        return maps;
    }

    void setDificulties(mapProperties[] maps)
    {
        // looks on wallmaps and set difficulty based on max height
        mapProperties[] sortedMaps = maps;
        System.Array.Sort(sortedMaps, delegate (mapProperties map1, mapProperties map2) { return map1.Map.GetComponent<MeshFilter>().sharedMesh.bounds.max.y.CompareTo(map2.Map.GetComponent<MeshFilter>().sharedMesh.bounds.max.y); });
        for(int i = 0; i < maps.Length; i++)
        {
            maps[sortedMaps[i].Index].Difficulty = i;
        }
    }
}
