using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour {

    public int currentNode = 0; //Selected node: Level Info references this when displaying info
    public int currentLevel = 0; //Selected node's reference scene: Level Info references this when switching levels
    [SerializeField]
    private GameObject cam; //Only refers to the position of the camera, and therefore the camera can be placed inside an empty to make movement code easier
    public LevelNode[] nodes; //

    private void Start()
    {
        SaveManager manager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
        foreach (LevelNode node in nodes)
        {
            node.starCount = manager.starCount[node.id];
            node.levelTime = manager.levelTime[node.id];
            node.unlockCollected = manager.unlockCollected[node.id];
            node.unhurt = manager.unhurt[node.id];        
        }
    }

    void Update () {
        if (nodes[currentNode] != null){
            transform.position = new Vector2(nodes[currentNode].visual.transform.position.x, nodes[currentNode].visual.transform.position.y + .5f);
            cam.transform.position = nodes[currentNode].transform.position;
        }
    }

    public void SetLevel(LevelNode target) {
        if (nodes[target.reference] != null){
            currentLevel = target.reference;
            currentNode = target.id;
        }
    }
}
