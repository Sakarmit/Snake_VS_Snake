using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISnake : MonoBehaviour
{
    // Grid related vars
    public float[] xRange = new float[2];
    public float[] yRange = new float[2];

    // Should only ever me -1 (left), 0 (none), or 1 (right)
    private int nextRotationDirection = 0;
    bool rotateAtNextGrid = false;

    //Positioning/Movement related vars
    Vector3 expectedGridPosition, nextPosition;
    private float satisfactionDistance = 0.09f;
    float loopTolerance = 0.02f;
    
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

    public ObjectTypes[,] worldView;
    // Start is called before the first frame update
    void Start()
    {
        expectedGridPosition = transform.position;
        size = Global.snakeSize;
        worldView = new ObjectTypes[Global.width,Global.height];
    }

    // Update is called once per frame
    void Update()
    {
        
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
            bodyPrefab.GetComponent<SpriteRenderer>().color = Color.red;

            if (rotateAtNextGrid) {
                transform.RotateAround(transform.position, Vector3.forward, 90*nextRotationDirection);
                
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
    }

    void OnCollisionEnter2D(Collision2D collider) {
        if(collider.gameObject.tag == "Snake") {
            deathSequence();
        }
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
        //Grey out body
        foreach (var bodyElem in bodyElems) {
            bodyElem.GetComponent<SpriteRenderer>().color = Color.grey;
        }
        //Grey out head
        GetComponent<SpriteRenderer>().color = Color.grey;

        GameManager.snakeAICount--;
        Destroy(gameObject);
    }
}
