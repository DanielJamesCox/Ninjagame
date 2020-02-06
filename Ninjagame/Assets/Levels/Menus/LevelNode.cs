using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour {

    //all the following are accessed by LevelSelectController
    public int reference; //level's scene index, referenced when LevelInfo wants to switch scenes
    public int id; //node's position in array, for self-reference
    public GameObject visual;

    [SerializeField]
    private LevelSelectController control;

    //all the following are accessed by LevelInfo
    public int starCount = 0;
    public float levelTime = 0f;
    public bool unlockCollected = true;
    public bool unhurt = false;
    public string levelDescription = "--";

    /*
    private void Start()
    {
        SaveManager manager = GameObject.Find("Manager").GetComponent<SaveManager>();
        starCount = manager.starCount[id];
        levelTime = manager.levelTime[id];
        unlockCollected = manager.unlockCollected[id];
        unhurt = manager.unhurt[id];
    }
    */

    private void SelectedLevelJump() {
        control.SetLevel(this);
    }
}
