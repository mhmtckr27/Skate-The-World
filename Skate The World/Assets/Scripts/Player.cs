using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,20f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        FindObjectOfType<Game>().spawnRoad();
    }
}
