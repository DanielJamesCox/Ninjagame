using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerRockBehavior : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Enemy" || other.tag == "Killable") {
			Destroy (other.gameObject);
		}
	}
}
