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
    }
    // Start is called before the first frame update
    void Start()
    {

        dstToCmr = transform.position - camera.transform.position;
        dstToGnd = GetComponent<BoxCollider>().bounds.extents.y;
        rigidbody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        cameraFollow();
        if (isNextLevelReady)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 40, rigidbody.velocity.z);
        if (!isNextLevelReady)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, speed);


        //transform.parent.position = transform.localPosition;
        //transform.parent.position = transform.position;

        if (wallsFinished)
        {
            if (transform.position.y <= 20f)
            {
                nextLevel();
            }
        }

            if (Input.GetKeyDown(KeyCode.Space))
                jump();
        if (onGround())
        {
            neutral();
        }

        if (onRail)
            rail();

        checkForward();
        moveChange();

    }

    private void nextLevel()
    {
        wallsFinished = false;
        FindObjectOfType<Game>().nextLevel();
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

    private void neutral()
    {
        onRail = false;
        transform.eulerAngles = Vector3.zero;
    }

    private void rail()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            rot = 1;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            rot = -1;

        transform.eulerAngles = new Vector3(0, 0, 60) * rot;
    }

    private void moveChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            skateMove = SkateMoves.red;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            skateMove = SkateMoves.green;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            skateMove = SkateMoves.yellow;

        switch (skateMove)
        {
            case SkateMoves.red:
                GetComponent<Renderer>().material.color = Color.red;
                break;
            case SkateMoves.green:
                GetComponent<Renderer>().material.color = Color.green;
                break;
            case SkateMoves.yellow:
                GetComponent<Renderer>().material.color = Color.yellow;
                break;
            default:
                GetComponent<Renderer>().material.color = Color.blue;
                break;

        }
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
        }
    }

    IEnumerator waitNextWall()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<Game>().spawnWall(transform.position, rigidbody.velocity);

    }

    private void jump()
    {
        animator.SetTrigger("Jump");
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
    }

    private bool onGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, dstToGnd + 1f, layerMask);
    }

    private void OnTriggerEnter(Collider collider)
    {
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
