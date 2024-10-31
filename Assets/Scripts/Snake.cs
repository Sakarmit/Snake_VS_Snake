using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Snake : MonoBehaviour
{
    // Grid related vars
    public float[] xRange = new float[2];
    public float[] yRange = new float[2];

    // Rotation related vars
    private KeyCode[] inputKeys = new KeyCode[] {KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A};
    private int leftKeyIndex = 3;
    private int rightKeyIndex= 1;
    // Should only ever me -1 (left), 0 (none), or 1 (right)
    private int nextRotationDirection = 0;
    //Variable used to make sure snake moved at least one grid forward after rotating
    //Fixes rotating 180 onto itself
    private bool movedAfterRotation = true;

    // Positioning/Movement related vars
    Vector3 expectedGridPosition, nextPosition;
    private float satisfactionDistance = 0.09f;
    float loopTolerance = 0.02f;
    
    // Snake Info
    [SerializeField] GameObject bodyPrefab;
    Queue<GameObject> bodyElems = new Queue<GameObject>();
    public int size = 3;

    void Start()
    {
        expectedGridPosition = transform.position;
        nextPosition = transform.position;
    }

    void Update()
    {
        getInput();
    }

    void FixedUpdate() {
        updatePosition();

        float distanceFromExpectedPosition = Vector3.Distance(transform.position, expectedGridPosition);
        if (distanceFromExpectedPosition < satisfactionDistance) {
            loopToBoard();
            transform.position = expectedGridPosition;
            expectedGridPosition = nextPosition;
            
            movedAfterRotation = true;

            //If the current body length is greater than snake size remove body in at the tail
            if (bodyElems.Count > size) {
                Destroy(bodyElems.Dequeue());
            }
            //Spawn new body prefab
            GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
            bodyElems.Enqueue(body);
        } else {
            //If the snaked needs to be rotated then rotating and updating input keys
            if (movedAfterRotation && nextRotationDirection != 0) {
                transform.RotateAround(transform.position, Vector3.forward, 90*nextRotationDirection);
                rightKeyIndex = (rightKeyIndex + nextRotationDirection + 2) % 4;
                leftKeyIndex = (leftKeyIndex + nextRotationDirection + 2) % 4;
                nextRotationDirection = 0;
                movedAfterRotation = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collider) {
        if(collider.gameObject.tag == "Snake") {
            deathSequence();
        }
    }

    void getInput() {
        if (Input.GetKeyDown(inputKeys[rightKeyIndex])) {
            nextRotationDirection = -1;
        } else if (Input.GetKeyDown(inputKeys[leftKeyIndex])) {
            nextRotationDirection = 1;
        }
        nextPosition = expectedGridPosition + 0.5f * transform.up.normalized;
    }

    void updatePosition() {
        transform.position = Vector3.MoveTowards(
            transform.position,
            expectedGridPosition,
            0.03f
        );
    }
    void loopToBoard() {
        //If the snake is off grid move it back on the other size relatively
        Vector3 newPos = transform.position;
        if (newPos.x - xRange[1] > loopTolerance) {
            print(newPos.x - xRange[1]);
            newPos.x = ((newPos.x + xRange[0])%(2*xRange[1]))+xRange[0]-0.5f;
        } else if (xRange[0] - newPos.x > loopTolerance) {
            newPos.x = ((newPos.x + xRange[1])%(2*xRange[1]))+xRange[1]+0.5f;
        } 

        if (newPos.y - yRange[1] > loopTolerance) {
            newPos.y = ((newPos.y + yRange[0])%(2*yRange[1]))+yRange[0]-0.5f;
        } else if (yRange[0] - newPos.y > loopTolerance) {
            newPos.y = ((newPos.y + yRange[1])%(2*yRange[1]))+yRange[1]+0.5f;
        } 

        //If snake was looped update all position variables
        if (transform.position != newPos) {
            expectedGridPosition = expectedGridPosition + newPos - transform.position;
            nextPosition = nextPosition + newPos - transform.position;
            transform.position = newPos;
        }
    }

    void deathSequence() {
        //Destroy body
        while (bodyElems.Count > 0) {
            Destroy(bodyElems.Dequeue());
        }
        GameManager.snakeCount--;
        //Destroy snake (head)
        Destroy(gameObject);
    }
}