using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Json;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject obstacleRoad;
    [SerializeField] private GameObject railRoad;
    [SerializeField] private string jsonPath;
    [SerializeField] private int levelNo;
    private GameObject rd;
    private GameObject[] roads;
    private GameObject currentRoad;
    private Road[] roadsObject;
    private int roadCount;
    private int currentRoadInd;


    private void Start()
    {
        initLevelConfig();
        initRoads();
    }

    private void initRoads()
    {
        roadCount = 1;
        roads = new GameObject[50];
        rd = Instantiate(currentRoad, currentRoad.transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        roads[++roadCount] = rd;
    }

    private void initLevelConfig()
    {
        jsonPath = Application.dataPath + jsonPath;
        string json = File.ReadAllText(jsonPath);
        roadsObject = JsonHelper.FromJson<Road>(json);
        getNextRoad();
    }

    private void getNextRoad()
    {
        if (roadsObject[levelNo].roads[currentRoadInd].Equals("road"))
            currentRoad = road;
        else if (roadsObject[levelNo].roads[currentRoadInd].Equals("railRoad"))
            currentRoad = railRoad;
        else if (roadsObject[levelNo].roads[currentRoadInd].Equals("obstacleRoad"))
            currentRoad = obstacleRoad;
        else
            currentRoad = null;
        currentRoadInd++;
    }

    public void spawnRoad()
    {
        getNextRoad();
        if (currentRoad)
        {
        rd = Instantiate(currentRoad, rd.transform.position + new Vector3(0,0,currentRoad.transform.lossyScale.z*10),Quaternion.identity);
        roads[++roadCount] = rd;
        Destroy(roads[roadCount -2]);
        }

    }

}

[System.Serializable]
public class Road
{
    public int level;
    public string[] roads;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}