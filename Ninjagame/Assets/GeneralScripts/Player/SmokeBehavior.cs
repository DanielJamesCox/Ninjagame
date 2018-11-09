using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBehavior : MonoBehaviour {
	float deathTimer = 0f;

	void Start(){
	}

	//Full description: on touch, conceal player/enemies, on dissipate/exit, reveal them
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
            other.GetComponent<PlayerStateMachine>().stealth = 2;
        } else if (other.tag == "Enemy") {
            other.GetComponent<EnemyStateMachine>().stealth = true;
        }
	}

	void OnTriggerStay2D(Collider2D other){
		if (deathTimer > 5f) {
            if (other.tag == "Player"){
                other.GetComponent<PlayerStateMachine>().stealth = 0;
            }
            else if (other.tag == "Enemy"){
                other.GetComponent<EnemyStateMachine>().stealth = false;
            }
        }
	}

	void OnTriggerExit2D(Collider2D other){
        if (other.tag == "Player") {
            other.GetComponent<PlayerStateMachine>().stealth = 0;
        } else if (other.tag == "Enemy") {
            other.GetComponent<EnemyStateMachine>().stealth = false;
        }
	}

	void Update(){
		deathTimer += Time.deltaTime;
		if (deathTimer > 5f) {
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
		if (deathTimer > 5.1f)
			Destroy (gameObject);
	}
}
