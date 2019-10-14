using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject road;

    public void spawnRoad()
    {
        road = Instantiate(road, road.transform.position + new Vector3(0,0,road.transform.lossyScale.z*10),Quaternion.identity);
    }

}
