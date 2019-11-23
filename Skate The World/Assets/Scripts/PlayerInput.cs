using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public bool isEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool jumpPressed()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            return true;
        return false;
    }

    public bool leftArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            return true;
        return false;
    }

    public bool rightArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            return true;
        return false;
    }

    public bool upArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return true;
        return false;
    }

    public bool downArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            return true;
        return false;
    }
}
