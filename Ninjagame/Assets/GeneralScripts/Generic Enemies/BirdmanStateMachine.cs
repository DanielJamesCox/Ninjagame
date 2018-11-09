using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdmanStateMachine : EnemyStateMachine {

    [SerializeField]
    float slashGoal = 1f;
    float slashTimer = 0f;

    bool hasBeenSeen = false;

    [SerializeField]
    GameObject pointer;
    [SerializeField]
    SlashBehavior slash;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	new void Update () {
		
	}
}
