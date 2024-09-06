using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 10;
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
    }
}
