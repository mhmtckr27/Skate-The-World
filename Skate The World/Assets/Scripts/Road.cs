using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public int roadType; // 0 == Empty Road ,, 1== Obstacle Road(3 different Walls)
    // Start is called before the first frame update
    void Start()
    {
        if (tag == "Road")
            roadType = 0;
        else if (tag == "ObstacleRoad")
            roadType = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (roadType == 1)
        {

        }
    }
}
