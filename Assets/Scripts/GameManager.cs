using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        //Update board size based on width and height variables
        UpdateBoardSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateBoardSize() {
        //Get background grid object
        GameObject board = GameObject.Find("Grid");

        //Get the render of the grid object
        SpriteRenderer gridRender = board.GetComponent<SpriteRenderer>();

        //Set actual grid width and height based on width and height fields
        gridRender.size = new Vector2(width/2f,height/2f);

        //Set scale of grid to fill in screen vertically
        board.transform.localScale = new Vector3(20f/height,20f/height,0);
    }
}
