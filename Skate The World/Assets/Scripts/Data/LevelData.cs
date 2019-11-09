using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Header("Level Info")]
    public string Name;
    public int DistanceBtwnObs, RailroadCount, ObsCount, RampCount, TwisterCount, WallCount;

    [Header("Player Info")]
    public float MovementSpeed;
    public float MovementAcceleration;

}
