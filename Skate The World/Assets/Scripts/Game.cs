using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Json;
using UnityEngine.Networking;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject[] roadTypes;
    [SerializeField] private GameObject[] wallTypes;
    [SerializeField] private int levelNo;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mainCamera;
    private string jsonPath;
    private GameObject rd;
    private GameObject[] roads;
    private GameObject currentRoad = null;
    private GameObject currentWall = null;
    private Road[] roadsObject;
    private int roadCount;
    private int currentRoadInd;
    private int currentWallInd;
    public int wallHitCount { get; set; }
    private float wallOffset=0.25f;
    public bool isOnNextLevel = false;
    public int initLevelInd;
    //private static Game game;
    //public static Game Instance { get { return game; } }



    private void Awake()
    {
        if (!FindObjectOfType<Player>())
        {
            player = Instantiate(player);
            player.GetComponent<Player>().enabled = true;
        }
        if (!FindObjectOfType<Camera>())
        {
            mainCamera = Instantiate(mainCamera);
            player.GetComponent<Player>().camera = mainCamera.GetComponent<Camera>();
            FindObjectOfType<CurveController>().GetComponent<CurveController>().CurveOrigin = mainCamera.transform;
        }
    //    //if(game != null && game != this)
    //    //{
    //    //    Destroy(gameObject);
    //    //}
    //    //else
    //    //{
    //    //    game = this;
    //    //}
    //    //SceneManager.LoadScene(PlayerPrefs.GetInt("levelNo", 0));
    }

    private void Start()
    {
        BetterStreamingAssets.Initialize();
        roadsObject = ReadFromJson("levelconfig.json");
        initLevelConfig();
        initRoads();
    }



    private void initRoads()
    {
        roadCount = 1;
        roads = new GameObject[50];
        //if (SceneManager.GetActiveScene().buildIndex == initLevelInd)
        //{
        //    rd = Instantiate(currentRoad, currentRoad.transform.position + new Vector3(0,0, currentRoad.transform.lossyScale.z * 2.5f), Quaternion.identity);
        //}
        //else
        {
            rd = Instantiate(currentRoad, FindObjectOfType<Player>().transform.position + new Vector3(0, -8, currentRoad.transform.lossyScale.z * 4f), Quaternion.identity);
            isOnNextLevel = false;
        }
        roads[++roadCount] = rd;
    }

    public static Road[] ReadFromJson(string path)
    {
        if (!BetterStreamingAssets.FileExists(path))
        {
            Debug.LogErrorFormat("Streaming asset not found: {0}", path);
            return null;
        }

        using (var stream = BetterStreamingAssets.OpenRead(path))
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            Road[] rds = JsonHelper.FromJson<Road>(text);
            
            return rds;
        }

    }


    private void initLevelConfig()
    {

        //AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "LevelConfig.json");
        //jsonPath = Path.Combine(Application.streamingAssetsPath, "LevelConfig.json");

        //if (!BetterStreamingAssets.FileExists(path))
        //{
        //    Debug.LogErrorFormat("Streaming asset not found: {0}", path);
        //    return null;
        //}
        //using(var stream = BetterStreamingAssets.OpenRead(path))
        //{
        //    var serializer = 

        //}
        //string json = File.ReadAllText(jsonPath);
        //roadsObject = JsonHelper.FromJson<Road>(json);
        //var bundle = BetterStreamingAssets.LoadAssetBundle(path);

        getNextRoad();
    }

    private void getNextRoad()
    {
        int i = 0;
        currentRoad = null;
        while (i < roadTypes.Length && !currentRoad)
        {
            if (roadsObject[levelNo].roads[currentRoadInd].Equals(roadTypes[i].name))
                currentRoad = roadTypes[i];
            i++;
        }
        currentRoadInd++;
    }

    public void spawnRoad()
    {
        getNextRoad();
        if (currentRoad)
        {
            rd = Instantiate(currentRoad, rd.transform.position + new Vector3(0, -0.001f, currentRoad.transform.lossyScale.z * 10), Quaternion.identity);
            roads[++roadCount] = rd;
            Destroy(roads[roadCount - 2]);
        }

    }

    public void spawnWall(Vector3 playerPos,Vector3 playerVel)
    {

        if (wallHitCount == roadsObject[levelNo].walls.Length)
        {
            FindObjectOfType<Player>().wallsFinished = true;
            FindObjectOfType<Player>().pullDown();
            return;
        }
        getNextWall();

        if (currentWall)
        {
            Instantiate(currentWall, (playerPos + playerVel * wallOffset),Quaternion.identity);
        }



    }
    private void getNextWall()
    {

        int i = 0;
        currentWall = null;
        if(currentWallInd != roadsObject[levelNo].walls.Length)
        {
            while (i < wallTypes.Length && !currentWall)
            {
                if (roadsObject[levelNo].walls[currentWallInd].Equals(wallTypes[i].name))
                    currentWall = wallTypes[i];
                i++;
            }
        currentWallInd++;
        }

    }


    public void nextLevel()
    {
        //PlayerPrefs.SetInt("levelNo", levelNo + 1);
        //levelNo++;
        isOnNextLevel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

[System.Serializable]
public class Road
{
    public int level;
    public string[] roads;
    public string[] walls;
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