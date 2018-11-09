using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableObjectBehavior : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Slash")
			Destroy (gameObject);
	}
}
