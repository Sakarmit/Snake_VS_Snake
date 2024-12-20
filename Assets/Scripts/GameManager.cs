using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    GameObject board;
    [SerializeField] int width;
    [SerializeField] int height;

    float[] widthMinMax;
    float[] heightMinMax;
    
    new GameObject camera;
    public GameObject snakePrefab;
    public GameObject AISnakePrefab;
    public static int snakeCount = 0;
    public GameObject eggPrefab;
    int maxFoodCount = 3;
    public static int currentFoodCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        width = Global.width;
        height = Global.height;

        //Get camera
        camera = GameObject.Find("Main Camera");

        //Get background grid object
        board = GameObject.Find("Grid");

        //Update camera & board size based on width and height variables
        UpdateCameraAndBoardSize();

        spawnSnake();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFoodCount < maxFoodCount) {
            currentFoodCount++;
            spawnObjectIfEmpty(eggPrefab);
        }

        if (snakeCount <= 1) {
            spawnAISnake();
        }
    }

    void UpdateCameraAndBoardSize() {
        //Get the render of the grid object
        SpriteRenderer gridRender = board.GetComponent<SpriteRenderer>();

        //Set actual grid width and height based on width and height fields
        gridRender.size = new Vector2(width * 0.5f, height * 0.5f);

        //Set the camera to fit grid minimizing on edge on either width or height
        Camera cameraComp = camera.GetComponent<Camera>();
        cameraComp.orthographicSize = Mathf.Max(height * 0.25f, width * 0.140714f);

        //Find the centers of min and max grid width and height cells
        Global.widthMinMax = new float[2] {0.25f - 0.25f*width, 0.25f*width - 0.25f};
        Global.heightMinMax = new float[2] {0.25f - 0.25f*height, 0.25f*height - 0.25f};
    }

    bool isLocationEmpty(float x, float y) {
        Collider2D collider = Physics2D.OverlapCircle(new Vector2(x, y), 0.25f);
        return collider == null;
    }

    GameObject spawnObjectIfEmpty(GameObject spawningObject) {
        float randX, randY;
        do
        {
            randX = 0.50f*Random.Range(1, width) - 0.25f*(width + 1);
            randY = 0.50f*Random.Range(1, height) - 0.25f*(height + 1);
        } while (!isLocationEmpty(randX, randY));
        
        return Instantiate(spawningObject, new Vector3(randX, randY, 0), Quaternion.identity);
    }
    
    void spawnSnake() {
        GameObject snake = spawnObjectIfEmpty(snakePrefab);
        Snake snakeScript = snake.GetComponent<Snake>();
        
        snakeCount++;
    }

    void spawnAISnake() {
        GameObject snake = spawnObjectIfEmpty(AISnakePrefab);
        AISnake AISnakeScript = snake.GetComponent<AISnake>();

        snakeCount++;
    }
}
