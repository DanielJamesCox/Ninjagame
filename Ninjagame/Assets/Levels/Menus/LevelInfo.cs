using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelInfo : MonoBehaviour {

    [SerializeField]
    LevelSelectController controller;
    [SerializeField]
    Text textbox;
    [SerializeField]
    Button goToLevel;

    int selectedLevel;
    int starCount;
    float levelTime;
    bool unlockCollected;
    bool unhurt = false;
    string levelDescription;

    private void Start()
    {
        
    }

    private void Update()
    {
        selectedLevel = controller.nodes[controller.currentNode].id;
        textbox.text = "Level " + (controller.nodes[selectedLevel].id + 1) + ": " + levelDescription;
        levelTime = controller.nodes[selectedLevel].levelTime;
        string levelTimeFormatted = string.Format("{0}:{1:00}", (levelTime - levelTime % 60) / 60, levelTime % 60);
        textbox.text = textbox.text + "\n" + "Best Clear Time: " + levelTimeFormatted;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(controller.currentLevel);
    }
}
