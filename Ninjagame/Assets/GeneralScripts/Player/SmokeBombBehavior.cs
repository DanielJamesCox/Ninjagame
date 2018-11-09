using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombBehavior : MonoBehaviour {

	[SerializeField]
	GameObject puff;

	void OnTriggerEnter2D(Collider2D other){
		if (other.isTrigger == false && other.tag != "Player") {
			GameObject newpuff = Instantiate (puff);
			newpuff.transform.position = transform.position;
			Destroy (gameObject);
		}
	}
}
