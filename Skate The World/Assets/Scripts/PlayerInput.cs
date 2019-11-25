using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public bool isEnabled = true;
    // If the touch is longer than MAX_SWIPE_TIME, we dont consider it a swipe
    public const float MAX_SWIPE_TIME = 1f;

    // Factor of the screen width that we consider a swipe
    // 0.17 works well for portrait mode 16:9 phone
    public const float MIN_SWIPE_DISTANCE = 0.05f;

    public static bool swipedRight = false;
    public static bool swipedLeft = false;
    public static bool swipedUp = false;
    public static bool swipedDown = false;


    public bool debugWithArrowKeys = true;

    Vector2 startPos;
    float startTime;

#pragma warning disable MS002 // Cyclomatic Complexity does not follow metric rules.
    public void Update()
#pragma warning restore MS002 // Cyclomatic Complexity does not follow metric rules.
    {
        swipedRight = false;
        swipedLeft = false;
        swipedUp = false;
        swipedDown = false;

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
                startTime = Time.time;
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (Time.time - startTime > MAX_SWIPE_TIME) // press too long
                    return;

                Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

                Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

                if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
                    return;

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                { // Horizontal swipe
                    if (swipe.x > 0)
                    {
                        swipedRight = true;
                    }
                    else
                    {
                        swipedLeft = true;
                    }
                }
                else
                { // Vertical swipe
                    if (swipe.y > 0)
                    {
                        swipedUp = true;
                    }
                    else
                    {
                        swipedDown = true;
                    }
                }
            }
        }

    }
    public bool jumpPressed()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) || swipedUp)
            return true;
        return false;
    }

    public bool leftArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || swipedLeft)
            return true;
        return false;
    }

    public bool rightArrowPressed()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || swipedRight)
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
        if (Input.GetKeyDown(KeyCode.DownArrow) || swipedDown)
            return true;
        return false;
    }
}