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

    bool rotateAtNextGrid = false;

    //Positioning/Movement related vars
    Vector3 expectedGridPosition, nextPosition;
    private float satisfactionDistance = 0.09f;
    
    //Snake body Info
    [SerializeField] GameObject bodyPrefab;
    [SerializeField] Sprite straightBody;
    [SerializeField] Sprite cornerBody;
    Queue<GameObject> bodyElems = new Queue<GameObject>();
    GameObject lastBody;
    //Whether the body should be rotated 180 degrees
    //Only needed for body spawned on snake turning right
    bool rotationModifier = false;
    public int size;

    void Start()
    {
        size = Global.snakeSize;
        xRange = Global.widthMinMax;
        yRange = Global.heightMinMax;
        expectedGridPosition = transform.position;
        getInput();
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

            //If last body is not null enable it's collider
            if (lastBody) {
                lastBody.GetComponent<BoxCollider2D>().enabled = true;
            }

            //If the current body length is greater than snake size remove body in at the tail
            if (bodyElems.Count > size) {
                Destroy(bodyElems.Dequeue());
            }
            //Spawn new body prefab
            Vector3 newBodyRotation = transform.rotation.eulerAngles;
            if (rotationModifier) {
                newBodyRotation.z += 90;
                rotationModifier = false;
            }
            lastBody = Instantiate(bodyPrefab, transform.position, Quaternion.Euler(newBodyRotation));
            bodyElems.Enqueue(lastBody);
            bodyPrefab.GetComponent<SpriteRenderer>().sprite = straightBody;

            if (rotateAtNextGrid) {
                transform.RotateAround(transform.position, Vector3.forward, 90*nextRotationDirection);

                //If the snaked needs to be rotated then rotating and updating input keys
                rightKeyIndex = (rightKeyIndex + nextRotationDirection + 2) % 4;
                leftKeyIndex = (leftKeyIndex + nextRotationDirection + 2) % 4;
                
                expectedGridPosition = transform.position + 0.5f * transform.up.normalized;
                nextPosition = expectedGridPosition;

                nextRotationDirection = 0;
                rotateAtNextGrid = false;
            }
        } else if (nextRotationDirection != 0) {
            //Update texture for next body
            bodyPrefab.GetComponent<SpriteRenderer>().sprite = cornerBody;
            if (nextRotationDirection == -1) {
                rotationModifier = true;
            } else {
                rotationModifier = false;
            }

            rotateAtNextGrid = true;
        }

        if (lastBody) {
            lastBody.GetComponent<SpriteRenderer>().color = Color.green;
        }     
    }

    void OnCollisionEnter2D(Collision2D collider) {
        if(collider.gameObject.tag == "SnakeBody") {
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
        Vector3 newPos = GeneralFunctions.loopToBoard(transform.position);

        //If snake was looped update all position variables
        if (transform.position != newPos) {
            expectedGridPosition = expectedGridPosition + newPos - transform.position;
            nextPosition = nextPosition + newPos - transform.position;
            transform.position = newPos;
        }
    }

    void deathSequence() {
        Global.score = size - Global.snakeSize;
        //Grey out body
        foreach (var bodyElem in bodyElems) {
            bodyElem.GetComponent<SpriteRenderer>().color = Color.grey;
        }
        //Grey out head
        GetComponent<SpriteRenderer>().color = Color.grey;

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }
}