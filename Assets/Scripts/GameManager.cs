using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 10;
    
    //Accessible variables indicating whether the width and height are odd
    //Quick access for offsetting elements on grid accordingly
    public bool oddX, oddY;

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

        oddX = width % 2 == 1;
        oddY = height % 2 == 1;
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
