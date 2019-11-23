using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Header("Level Info")]
    public string Name;
    //public int DistanceBtwnObs;
    //public int RailroadCount, ObsCount, RampCount, TwisterCount, WallCount;

    [Header("Player Info")]
    public float MovementSpeed;
    public float MovementAcceleration;

    [Header("Roads in Level")]
    public List<GameObject> Roads = new List<GameObject>();
}
