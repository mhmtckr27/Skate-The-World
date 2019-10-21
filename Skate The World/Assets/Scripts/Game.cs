using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject obstacleRoad;
    [SerializeField] private GameObject railRoad;
    private GameObject rd;
    private int roadCount;
    private GameObject[] roads;

    private void Start()
    {
        roadCount = 1;
        roads = new GameObject[50];
        rd = Instantiate(road, road.transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        roads[++roadCount] = rd;
    }

    public void spawnRoad()
    {
        rd = Instantiate(railRoad, rd.transform.position + new Vector3(0,0,railRoad.transform.lossyScale.z*10),Quaternion.identity);
        roads[++roadCount] = rd;
        Destroy(roads[roadCount -2]);

    }

    private void Update()
    {
        //Debug.Log(obstacleRoad.GetComponentsInChildren<GameObject>());
    }


}
