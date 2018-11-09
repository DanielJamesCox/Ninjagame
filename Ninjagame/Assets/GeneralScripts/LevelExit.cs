﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {

    [SerializeField]
    int scene = 0;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player")
			SceneManager.LoadScene (scene);	
	}
}
