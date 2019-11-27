using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Camera camera;
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField] private GameObject explosion;

    [SerializeField] private Sprite[] sprites;

    private SkateMoves skateMove;
    private Vector3 dstToCmr;
    private Vector3 EulerAngleVelocity;
    private PlayerInput playerInput;
    private float dstToGnd;  // Distance to the road
    private int rot;
    private bool onRail;
    private bool isNextLevelReady;

    public bool isSlideFinished;


    private Transform objectTransfom;

    private float noMovementThreshold = 0.005f;
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
        purple,
        empty,
    };

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        skateMove = SkateMoves.empty;
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


        dstToCmr = transform.position - camera.transform.position;
        dstToGnd = GetComponent<BoxCollider>().bounds.extents.y;

        rigidbody = GetComponent<Rigidbody>();
        //beforePos = Vector3.zero;
        playerInput = GetComponent<PlayerInput>();
        playerInput.isEnabled = true;
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
                animator.SetBool("land", true);
        }

        if (onGround() && isSlideFinished)
        {
            skateMove = SkateMoves.empty;
                isSlideFinished = false;
        }
        if (onGround() && animator.GetCurrentAnimatorStateInfo(0).IsName("Rail"))
        {
            neutral();
        }

        if (onGround() && ((animator.GetCurrentAnimatorStateInfo(0).IsName("Ground")) || (animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))))
        {
            animator.SetBool("land", false);
            neutral();
            if (playerInput.isEnabled && playerInput.downArrowPressed() && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Slide")))
            {
                animator.SetTrigger("slide");
            }
        }

        if (onRail)
            rail();


        if (!onGround())
        {
            if (playerInput.isEnabled && playerInput.downArrowPressed())

                if(transform.position.y+8 > dstToGnd * 1.3f)
                    rigidbody.AddForce(0, -350* Mathf.Abs( rigidbody.velocity.y), 0);
        }



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
            else if(!FindObjectOfType<Game>().isOnNextLevel)
            {
                isMoving = false;
            }
        }



        if (!isMoving)
            gameOver();
        
    }


    //private void deneme()
    //{
    //    animator.SetBool("land", false);
    //    neutral();
    //}
    private void neutral()
    {
        onRail = false;
        rot = 0;
        animator.ResetTrigger("onRail");
        animator.SetTrigger("ground");
        transform.eulerAngles = Vector3.zero;
        if (playerInput.isEnabled && playerInput.jumpPressed())
            jump();
    }


    private void gameOver()
    {
        animator.SetBool("die",true);
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Falling Back Death"))
            animator.SetBool("die", false);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            SceneManager.LoadScene("Start");
            Destroy(gameObject);
            Destroy(camera.gameObject);
        }


    }

    private void nextLevel()
    {
        wallsFinished = false;
        playerInput.isEnabled = true;
        FindObjectOfType<Game>().nextLevel();
    }



    public void pullDown()
    {
        isNextLevelReady = false;
        rigidbody.AddForce(0, -2000 + FindObjectOfType<Game>().wallHitCount*-1000, 0);
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
        if (playerInput.isEnabled && playerInput.leftArrowPressed())
            rot = 1;

        else if (playerInput.isEnabled && playerInput.rightArrowPressed())
            rot = -1;

        transform.eulerAngles = new Vector3(0, 0, 60 * rot);
    }

    private void moveChange()
    {
        if (playerInput.isEnabled && playerInput.leftArrowPressed())
            skateMove = SkateMoves.red;
        else if (playerInput.isEnabled && (playerInput.upArrowPressed() || playerInput.jumpPressed()))
            skateMove = SkateMoves.green;
        else if (playerInput.isEnabled && playerInput.rightArrowPressed())
            skateMove = SkateMoves.yellow;
        else if (playerInput.isEnabled && playerInput.downArrowPressed() && onGround())
            skateMove = SkateMoves.purple;

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        switch (skateMove)
        {
            case SkateMoves.red:
                renderer.sprite = sprites[0];
                break;
            case SkateMoves.green:
                renderer.sprite = sprites[1];
                break;
            case SkateMoves.yellow:
                renderer.sprite = sprites[2];
                break;
            case SkateMoves.purple:
                renderer.sprite = sprites[3];
                break;
            default:
                renderer.sprite = null;
                break;

        }
    }

    private void checkForward()
    {
        RaycastHit hit;
        Vector3 effectNormal = Vector3.back;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f))
        {

            if (skateMove == SkateMoves.red && hit.collider.CompareTag("RedWall"))
            {
                Destroy(hit.collider.gameObject);
                Instantiate(explosion, transform.position + new Vector3(0, 3, 1), Quaternion.FromToRotation(Vector3.up, effectNormal));
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if (skateMove == SkateMoves.green && hit.collider.CompareTag("GreenWall"))
            {
                Destroy(hit.collider.gameObject);
                Instantiate(explosion, transform.position + new Vector3(0, 3, 1), Quaternion.FromToRotation(Vector3.up, effectNormal));
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if (skateMove == SkateMoves.yellow && hit.collider.CompareTag("YellowWall"))
            {
                Destroy(hit.collider.gameObject);
                Instantiate(explosion, transform.position + new Vector3(0, 3, 1), Quaternion.FromToRotation(Vector3.up, effectNormal));
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if (skateMove == SkateMoves.purple && hit.collider.CompareTag("PurpleWall"))
            {
                Destroy(hit.collider.gameObject);
                Instantiate(explosion, transform.position + new Vector3(0, 3, 1), Quaternion.FromToRotation(Vector3.up, effectNormal));
                if (isNextLevelReady)
                {
                    FindObjectOfType<Game>().wallHitCount++;
                    StartCoroutine(waitNextWall());
                }
            }
            else if(hit.collider.CompareTag("Wall"))
            {
                Destroy(hit.collider.gameObject);
                Instantiate(explosion, transform.position + new Vector3(0,3,1) , Quaternion.FromToRotation(Vector3.up, effectNormal));
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
        rigidbody.AddForce(0, 150, 0);
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
            //playerInput.isEnabled = true;
            StartCoroutine(waitForNextLevel());// Finish Level
        }
        else if (collider.CompareTag("SpeedBoostPoint"))
            speed *= 5;

        else if (collider.CompareTag("LockInput"))
        {
            playerInput.isEnabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rail"))
        {
            onRail = true;
        }
        else if (collision.collider.CompareTag("enemy"))
        {
            gameOver();
        }
    }

    IEnumerator waitForNextLevel()
    {
        yield return new WaitForSeconds(.25f);
        isNextLevelReady = true;
        FindObjectOfType<Game>().spawnWall(transform.position, rigidbody.velocity);

    }
}
