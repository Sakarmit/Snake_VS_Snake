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

    // Positioning/Movement related vars
    Vector3 expectedGridPosition, nextPosition;
    bool canUpdatePosition = true;
    private float satisfactionDistance = 0.09f;

    // Snake Info
    [SerializeField] GameObject bodyPrefab;
    Queue<GameObject> bodyElems = new Queue<GameObject>();
    int size = 3;

    //Temp object to visualize expectedPosition change
    GameObject targetObj;

    void Start()
    {
        expectedGridPosition = transform.position;
        nextPosition = transform.position;
        targetObj = GameObject.Find("Target");
    }

    void Update()
    {
        if (canUpdatePosition) {
            getInput();
        }
    }

    void FixedUpdate() {
        updatePosition();
        if (Vector3.Distance(transform.position, expectedGridPosition) < satisfactionDistance) {
            loopToBoard();
            transform.position = expectedGridPosition;
            expectedGridPosition = nextPosition;

            transform.RotateAround(transform.position, Vector3.forward, 90*nextRotationDirection);
            nextRotationDirection = 0;
            
            if (bodyElems.Count > size) {
                Destroy(bodyElems.Dequeue());
            }
            GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
            bodyElems.Enqueue(body);

            canUpdatePosition = true;
        }
    }

    void getInput() {
        if (Input.GetKeyDown(inputKeys[rightKeyIndex])) {
            nextRotationDirection = -1;
            rightKeyIndex = (rightKeyIndex+1) % 4;
            leftKeyIndex = (leftKeyIndex+1) % 4;
            canUpdatePosition = false;
        } else if (Input.GetKeyDown(inputKeys[leftKeyIndex])) {
            nextRotationDirection = 1;
            leftKeyIndex = (leftKeyIndex+3) % 4;
            rightKeyIndex = (rightKeyIndex+3) % 4;
            canUpdatePosition = false;
        }
        nextPosition = expectedGridPosition + 0.5f * transform.up.normalized;
        targetObj.transform.position = expectedGridPosition;
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
        if (newPos.x > xRange[1]) {
            newPos.x = ((newPos.x + xRange[0])%(2*xRange[1]))+xRange[0]-0.5f;
        } else if (newPos.x < xRange[0]) {
            newPos.x = ((newPos.x + xRange[1])%(2*xRange[1]))+xRange[1]+0.5f;
        } 

        if (newPos.y > yRange[1]) {
            newPos.y = ((newPos.y + yRange[0])%(2*yRange[1]))+yRange[0]-0.5f;
        } else if (newPos.y < yRange[0]) {
            newPos.y = ((newPos.y + yRange[1])%(2*yRange[1]))+yRange[1]+0.5f;
        } 

        if (transform.position != newPos) {
            expectedGridPosition = expectedGridPosition + newPos - transform.position;
            nextPosition = nextPosition + newPos - transform.position;
            transform.position = newPos;
        }
    }
}