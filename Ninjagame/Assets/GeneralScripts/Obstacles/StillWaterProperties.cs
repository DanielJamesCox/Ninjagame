using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StillWaterProperties : MonoBehaviour {

	[SerializeField]
	float density;
	[SerializeField]
	bool isCloud;
    [SerializeField]
    bool isPoison = false;
	[SerializeField]
	PlayerStateMachine player;
	[SerializeField]
	float jump;

    float poisonTimer = 0f;
    int poisonAmount = 10;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerStateMachine>();
        jump = player.GetJumpStrength ();
	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Player") {
			player.SetIsUnderwater (true);
			player.GetComponent<Rigidbody2D> ().gravityScale = density;
			player.GetComponent<Rigidbody2D> ().drag = density;
			player.SetJumpStrength (jump * density);
		}
	}
	void OnTriggerStay2D (Collider2D other){
		if (other.tag == "Player") {
			player.SetIsUnderwater (true);
			player.GetComponent<Rigidbody2D> ().gravityScale = density;
			player.GetComponent<Rigidbody2D> ().drag = density;
			player.SetJumpStrength (jump * density);

            if (isPoison) {
                if(poisonTimer >= 1f)
                {
                    poisonTimer = 0f;
                    player.TakeDamage(poisonAmount);
                }
                poisonTimer += Time.deltaTime;
            }
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player") {
			player.SetIsUnderwater (false);
			player.GetComponent<Rigidbody2D> ().gravityScale = 1;
			player.GetComponent<Rigidbody2D> ().drag = 0;
			player.SetJumpStrength (jump);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
