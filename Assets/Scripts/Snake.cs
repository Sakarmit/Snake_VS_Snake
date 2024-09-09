using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Snake : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        fixGridPosition();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void fixGridPosition() {
        if (gameManager.oddX) {
            transform.position += new Vector3(0.25f,0,0);
        }
        if (gameManager.oddY) {
            transform.position += new Vector3(0,0.25f,0);
        }
    }
}
