using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameData GameData;

    [Space, SerializeField]
    private Player _player;

    [Space, Header("Roads"), SerializeField]
    private GameObject finishRoad;
    
    private int nextLevelStartPos = 0;

    private GameObject[] _roads = new GameObject[20];
    private void Awake()
    {
        if (GameData == null)
        {
            Debug.Log("Game Data is Empty");
        }
    }

    //dışarıdan erişip levelı oluşturmak için kullanılır
    public void SetNewLevel()
    {
        var currentLevel = PlayerPrefs.GetInt("LEVEL", 1);

        //Get and set Level data
        SpawnRoads(currentLevel);
        SetLevelData(currentLevel);
    }

    

    //private void GetRoads(int level)
    //{
    //    _railroadCount = GameData.Levels[level - 1].RailroadCount;
    //    _roadAmount += _railroadCount;

    //    _obstacleRoadCount = GameData.Levels[level - 1].ObsCount;
    //    _roadAmount += _obstacleRoadCount;

    //    _rampRoadCount = GameData.Levels[level - 1].RampCount;
    //    _roadAmount += _rampRoadCount;

    //    _twisterRoadCount = GameData.Levels[level - 1].TwisterCount;
    //    _roadAmount += _twisterRoadCount;
    //}

    //private void SetSpawnOrder()
    //{
    //    var roads = new GameObject[_roadAmount];
    //    int i = 0;

    //    while (i < _railroadCount)
    //    {
    //        roads[i] = railRoad;
    //        i++;
    //    }

    //    while (i < _obstacleRoadCount)
    //    {
    //        roads[i] = obstacleRoad;
    //        i++;
    //    }

    //    while (i < _rampRoadCount)
    //    {
    //        roads[i] = rampRoad;
    //        i++;
    //    }

    //    while (i < _twisterRoadCount)
    //    {
    //        roads[i] = twisterRoad;
    //        i++;
    //    }

    //    RandomizeSpawnOrder(roads);
    //}

    //private void RandomizeSpawnOrder(GameObject[] roads)
    //{
        
    //}

    private void SpawnRoads(int currentLevel)
    {
        GameObject previousRoad = null;

        foreach (GameObject road in GameData.Levels[currentLevel - 1].Roads)
        {
            if (previousRoad == null)
            {
                if(nextLevelStartPos == 0)
                {
                    previousRoad = Instantiate(finishRoad, new Vector3(0, 0, road.transform.lossyScale.z), Quaternion.identity);
                    
                }
                else
                {
                    previousRoad = Instantiate(finishRoad, new Vector3(0, 0, nextLevelStartPos), Quaternion.identity);
                }
            }

            var newRoad = Instantiate(road, previousRoad.transform.position + new Vector3(0, 0, road.transform.lossyScale.z * 10), Quaternion.identity);
            previousRoad = newRoad;

            //_roads[i] = previousRoad;
        }

        nextLevelStartPos = (int)previousRoad.transform.position.z;
    }

    private void SetLevelData(int currentLevel)
    {
        _player.speed = GameData.Levels[currentLevel - 1].MovementSpeed;
    }
}
