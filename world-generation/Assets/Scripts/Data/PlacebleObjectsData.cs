using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ScriptableObject, хранящий ассеты (префабы), которые использованы в игре
 */

[CreateAssetMenu(fileName = "AssetsContainer", menuName = "Data/PlacebleObjectsData", order = 51)]
public class PlacebleObjectsData : ScriptableObject
{
    //character
    public GameObject character;

    //structures
    public GameObject chunkPrefab;
    public List<GameObject> obstacles;
    public GameObject fence_line;

    //materials
    public Material grassMaterial;

    public static PlacebleObjectsData LoadFromAssets()
    {
        return Resources.Load("AssetsContainer") as PlacebleObjectsData;
    }
}
