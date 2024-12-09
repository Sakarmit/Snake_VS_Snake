using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject endText = GameObject.Find("EndMessage");
        endText.GetComponent<TMP_Text>().text += Global.score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeToMainMenuScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
