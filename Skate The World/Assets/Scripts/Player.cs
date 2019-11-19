using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool wallsFinished { get; set; }

    private Player player;
    private Rigidbody rigidbody;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float jumpSpeed = 75;
    [SerializeField]
    private float speed = 20;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private LayerMask layerMask;

    private SkateMoves skateMove;
    private Vector3 dstToCmr;
    private Vector3 EulerAngleVelocity;
    private float dstToGnd;  // Distance to the road
    private int rot;
    private bool onRail;
    private bool isNextLevelReady;
    float tmp = -1;

    private Transform objectTransfom;

    private float noMovementThreshold = 0.01f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;

    //Vector3 beforePos;
    //private bool isMoving;

    private enum SkateMoves
    {
        red,
        green,
        yellow,
        empty,
    };

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(camera);
        
        if (player == null)
        {
            player = this;
        }
        else
        {
            Destroy(gameObject);
            Destroy(camera);
        }

        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        dstToCmr = transform.position - camera.transform.position;
        dstToGnd = GetComponent<BoxCollider>().bounds.extents.y;

        rigidbody = GetComponent<Rigidbody>();
        //beforePos = Vector3.zero;

    }



    private void Update()
    {
        animator.SetFloat("velY", rigidbody.velocity.y);

        cameraFollow();
        if (isNextLevelReady)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 40, rigidbody.velocity.z);
        if (!isNextLevelReady)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, speed);


        if (wallsFinished)
        {
            if (transform.position.y <= 20f)
            {
                nextLevel();
            }
        }

        if (rigidbody.velocity.y < 0f)
        {
            if (isGroundBelow())
                animator.SetTrigger("land");
        }

        if (onGround())
        {
            animator.ResetTrigger("land");
            neutral();
        }

        if (onRail)
            rail();

        checkForward();
        moveChange();



        //Store the newest vector at the end of the list of vectors
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i + 1];
        }
        previousLocations[previousLocations.Length - 1] = transform.position;

        //Check the distances between the points in your previous locations
        //If for the past several updates, there are no movements smaller than the threshold,
        //you can most likely assume that the object is not moving
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                //The minimum movement has been detected between frames
                isMoving = true;
                break;
            }
            else
            {
                isMoving = false;
            }
        }



        if (!isMoving)
            gameOver();



    }



    private void gameOver()
    {
        animator.SetTrigger("die");
        
    }

    //IEnumerator WaitIfDead(float v)
    //{
    //    yield return new WaitForSeconds(v);
    //}

    private void nextLevel()
    {
        wallsFinished = false;
        FindObjectOfType<Game>().nextLevel();
    }

    private void neutral()
    {
        onRail = false;
        rot = 0;
        animator.ResetTrigger("onRail");
        animator.SetTrigger("ground");
        transform.eulerAngles = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.Space))
            jump();
    }


    public void pullDown()
    {
        isNextLevelReady = false;
        rigidbody.AddForce(0, -2000, 0);
        speed /= 5;
    }

    private void cameraFollow()
    {
        camera.transform.position = transform.position - dstToCmr;
    }
    private void rail()
    {

        animator.SetTrigger("onRail");
        //transform.GetChild(2).transform.eulerAngles = new Vector3(-90, 180, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            rot = 1;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            rot = -1;

        transform.eulerAngles = new Vector3(0, 0, 60 * rot);
    }

    private void moveChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            skateMove = SkateMoves.red;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            skateMove = SkateMoves.green;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            skateMove = SkateMoves.yellow;

        //switch (skateMove)
        //{
        //    case SkateMoves.red:
        //        GetComponent<Renderer>().material.color = Color.red;
        //        break;
        //    case SkateMoves.green:
        //        GetComponent<Renderer>().material.color = Color.green;
        //        break;
        //    case SkateMoves.yellow:
        //        GetComponent<Renderer>().material.color = Color.yellow;
        //        break;
        //    default:
        //        GetComponent<Renderer>().material.color = Color.blue;
        //        break;

        //}
    }

    private void checkForward()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f) && !onGround())
        {

            if (skateMove == SkateMoves.red && hit.collider.CompareTag("RedWall"))
            {
                Destroy(hit.collider.gameObject);
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if (skateMove == SkateMoves.green && hit.collider.CompareTag("GreenWall"))
            {
                Destroy(hit.collider.gameObject);
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if (skateMove == SkateMoves.yellow && hit.collider.CompareTag("YellowWall"))
            {
                Destroy(hit.collider.gameObject);
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else
            {
                Destroy(hit.collider.gameObject);
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }

        }
    }

    IEnumerator waitNextWall()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<Game>().spawnWall(transform.position, rigidbody.velocity);

    }

    private void jump()
    {

        animator.SetTrigger("jump");
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
    }

    private bool isGroundBelow()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 1f, layerMask);
    }

    private bool onGround()
    {
        
        return Physics.Raycast(transform.position, -Vector3.up, dstToGnd-0.5f, layerMask);
    }

    private void OnTriggerEnter(Collider collider)
    {

        //if(collider.CompareTag("Road")){
        //    animator.SetTrigger("land");
        //}

        if (collider.CompareTag("SpawnPoint"))
            FindObjectOfType<Game>().spawnRoad(); // Spawn Road
        else if (collider.CompareTag("FinishPoint"))
        {
            //speed /= 5;
            StartCoroutine(waitForNextLevel());// Finish Level
        }
        else if (collider.CompareTag("SpeedBoostPoint"))
            speed *= 5;
        else if (collider.CompareTag("TwistPoint"))
        {
            //animator.enabled = true;
            //animator.SetTrigger("TwistTrigger");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rail"))
        {

            
            onRail = true;
        }
    }

    IEnumerator waitForNextLevel()
    {
        yield return new WaitForSeconds(.25f);
        FindObjectOfType<Game>().spawnWall(transform.position, rigidbody.velocity);
        isNextLevelReady = true;

    }
}
