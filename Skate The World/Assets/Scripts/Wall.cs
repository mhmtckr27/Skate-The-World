using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Road parentRoad;
    // Start is called before the first frame update
    void Start()
    {
        parentRoad = GetComponentInParent<Road>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
