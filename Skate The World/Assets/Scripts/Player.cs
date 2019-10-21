using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpSpeed=75;
    [SerializeField] private float speed = 20;
    private Rigidbody rigidbody;
    private float dstToGnd;  // Distance to the road
    private SkateMoves skateMove;
    private bool balance;
    private Vector3 EulerAngleVelocity;

    private enum SkateMoves
    {
        red,
        green,
        yellow,
        empty,
    };


    // Start is called before the first frame update
    void Start()
    {
        dstToGnd = GetComponent<BoxCollider>().bounds.extents.y;
        rigidbody = FindObjectOfType<Rigidbody>();

        //rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
    }

    private void FixedUpdate()
    {
        if (balance)
            balanceGame();

        if (onGround())
        {
            
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, speed);
            //transform.position += transform.forward * Time.deltaTime*speed;
            balance = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && onGround())
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
        }
    }

    private void Update()
    {

        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.forward, out hit, 1f) && !onGround())
        {
            
            if (skateMove == SkateMoves.red && hit.collider.CompareTag("RedWall"))
                Destroy(hit.collider.gameObject);
            else if (skateMove == SkateMoves.green && hit.collider.CompareTag("GreenWall"))
                Destroy(hit.collider.gameObject);
            else if (skateMove == SkateMoves.yellow && hit.collider.CompareTag("YellowWall"))
                Destroy(hit.collider.gameObject);
        }

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

    private void randomRotation()
    {
        float randomSlide = UnityEngine.Random.Range(0, 1);
        if (randomSlide <= 0.5f)
            randomSlide = -45;
        else
            randomSlide = 45;
        EulerAngleVelocity = new Vector3(0, 0, randomSlide);
        balance = true;
    }

    private void balanceGame()
    {
        transform.eulerAngles = Vector3.back * Time.deltaTime * EulerAngleVelocity.z*5;
        //rigidbody.angularVelocity += EulerAngleVelocity * Time.deltaTime * 50;
        //Quaternion deltaRotaiton = Quaternion.Euler(EulerAngleVelocity * Time.deltaTime);
        //rigidbody.MoveRotation(rigidbody.rotation * deltaRotaiton);
        
    }

    private bool onGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, dstToGnd + 0.1f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        spawnRoad();
    }

    private void spawnRoad()
    {
        FindObjectOfType<Game>().spawnRoad();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rail"))
        {
            
            randomRotation();
        }
    }
}
