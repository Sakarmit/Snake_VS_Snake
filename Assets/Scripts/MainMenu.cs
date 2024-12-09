using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField WidthField;
    [SerializeField] private TMP_InputField HeightField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() {
        int width = int.Parse(WidthField.text);
        int height = int.Parse(HeightField.text);
        if (width < 5 || width > 20 || height < 5 || height > 20) {
            print("Invalid grid size");
            return;
        }
        Global.width = width;
        Global.height = height;
        GameManager.snakeCount = 0;
        GameManager.currentFoodCount = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void quitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void limitGridSize(string field) {
        switch (field) {
            case "WidthField":
                if (WidthField.text.Length > 2) {
                    WidthField.text = WidthField.text.Substring(0, 2);
                }
                break;
            case "HeightField":
                if (HeightField.text.Length > 2) {
                    HeightField.text = HeightField.text.Substring(0, 2);
                }
                break;
            default:
                print("Unimplemented field - " + field);
                break;
        }
    }
}
