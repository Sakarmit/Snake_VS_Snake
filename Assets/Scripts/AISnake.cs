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
        xRange = Global.widthMinMax;
        yRange = Global.heightMinMax;
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
            getNextStep();
            print(nextPosition);
            Debug.Break();
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
            bodyPrefab.GetComponent<SpriteRenderer>().color = Color.red;
        }  
    }

    void OnCollisionEnter2D(Collision2D collider) {
        if(collider.gameObject.tag == "Snake" || collider.gameObject.tag == "SnakeBody") {
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
        Vector3 newPos = GeneralFunctions.loopToBoard(transform.position);

        //If snake was looped update all position variables
        if (transform.position != newPos) {
            expectedGridPosition = expectedGridPosition + newPos - transform.position;
            nextPosition = nextPosition + newPos - transform.position;
            transform.position = newPos;
        }
    }

    void getNextStep() {
        GameObject[] Eggs = GameObject.FindGameObjectsWithTag("Egg");
        int closestEgg = 0;
        float closestDistance = GeneralFunctions.manhattanDistance(transform.position, Eggs[0].transform.position);
        for (int i = 0; i < Eggs.Length; i++) {
            float distance = GeneralFunctions.manhattanDistance(transform.position, Eggs[i].transform.position);
            if (distance < closestDistance) {
                closestEgg = i;
                closestDistance = distance;
            }
            Eggs[closestEgg].GetComponent<SpriteRenderer>().color = Color.white;
        }
        Eggs[closestEgg].GetComponent<SpriteRenderer>().color = Color.green;
        Vector2 target = new Vector2(Eggs[closestEgg].transform.position.x, 
                                     Eggs[closestEgg].transform.position.y);
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Node start = new Node(transform.position.x, transform.position.y, 
                                transform.rotation.z, target, null);
        openList.Add(start);
        while(openList.Count > 0) {
            //Get node with lowest f value
			int minFIndex = 0;
			for (int i = 0; i < openList.Count; i++) { 
				if (openList[i].f < openList[minFIndex].f) {
					minFIndex = i;
				}
			}

            Node current = openList[minFIndex];
            openList.RemoveAt(minFIndex);
            closedList.Add(current);
            if (current.x == target.x && current.y == target.y) {
                Node temp = current;
                while (temp.nodeParent.nodeParent != null) {
                    temp = temp.nodeParent;
                }
                
                

                nextPosition = new Vector3(temp.x, temp.y, 0);
                break;
            }
            List<Node> children = new List<Node>();
            children.Add(new Node(GeneralFunctions.nextLocation(current.x, current.y, current.angle - 90),
                                    current.angle - 90, target, current));
            children.Add(new Node(GeneralFunctions.nextLocation(current.x, current.y, current.angle),
                                    current.angle, target, current));
            children.Add(new Node(GeneralFunctions.nextLocation(current.x, current.y, current.angle + 90),
                                    current.angle + 90, target, current));
            foreach (Node child in children) {
                print($"Child: {child.x}, {child.y}");
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(child.x, child.y), Vector2.zero);
                if (hit.collider != null && (hit.collider.CompareTag("Snake") || hit.collider.CompareTag("SnakeBody"))) {
                    continue;
                }

                bool skipAfterLoop = false;
				foreach (Node n in closedList) {
					if (n.x == child.x && n.y == child.y && n.angle == child.angle) {
						skipAfterLoop = true;
						break;
					}
				}
				//If node was in closedList, skip the rest of the loop
				if (skipAfterLoop) {
					continue;
				}

                foreach (Node n in openList) {
					if (n.x == child.x && n.y == child.y && n.angle == child.angle) {
						//Since node is in openList, update it if the new g value is lower
						n.g = child.g;
						n.f = child.f;
						n.nodeParent = child.nodeParent;
						skipAfterLoop = true;
						break;
					}
				}
                //If  node was in openList and we updated it, skip the rest of the loop
				if (skipAfterLoop) {
					continue;
				}

                openList.Add(child);
            }
        }
    }

    void deathSequence() {
        //Grey out body
        foreach (var bodyElem in bodyElems) {
            Destroy(bodyElem);
        }

        GameManager.snakeCount--;
        Destroy(gameObject);
    }
}
