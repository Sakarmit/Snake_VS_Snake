using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 10;

    float[] widthMinMax;
    float[] heightMinMax;

    public GameObject snakePrefab;
    new GameObject camera;
    GameObject board;

    // Start is called before the first frame update
    void Start()
    {
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
        
    }

    void UpdateCameraAndBoardSize() {
        //Get background grid object

        //Get the render of the grid object
        SpriteRenderer gridRender = board.GetComponent<SpriteRenderer>();

        //Set actual grid width and height based on width and height fields
        gridRender.size = new Vector2(width * 0.5f, height * 0.5f);

        Camera cameraComp = camera.GetComponent<Camera>();
        cameraComp.orthographicSize = Mathf.Max(height * 0.25f, width * 0.140714f);

        widthMinMax = new float[2] {0.25f - 0.25f*width, 0.25f*width - 0.25f};
        heightMinMax = new float[2] {0.25f - 0.25f*height, 0.25f*height - 0.25f};
    }

    void spawnSnake() {
        float randX = 0.50f*Random.Range(1, width) - 0.25f*(width + 1);
        float randY = 0.50f*Random.Range(1, height) - 0.25f*(height + 1);
        GameObject snake = Instantiate(snakePrefab, new Vector3(randX, randY, 0), Quaternion.identity);
        Snake snakeScript = snake.GetComponent<Snake>();
        
        snakeScript.xRange = widthMinMax;
        snakeScript.yRange = heightMinMax;
    }
}
