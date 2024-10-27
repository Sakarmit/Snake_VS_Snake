using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : MonoBehaviour
{
    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collider) {
        GameObject gameObj = collider.gameObject;
        if (gameObj.tag == "Snake") {
            Snake snake = gameObj.GetComponent<Snake>();
            snake.size+=score;
            GameManager.currentFoodCount--;
            Destroy(gameObject);
        }
    }
}
