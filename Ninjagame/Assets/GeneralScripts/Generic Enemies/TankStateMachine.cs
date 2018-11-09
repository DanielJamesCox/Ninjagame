using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankStateMachine : EnemyStateMachine {

	bool inAir = false;
	float jumpTimer = 2.8f;

    [SerializeField]
    Vector2 patrolBounds;
    [SerializeField]
    int patrolMode = 0;
    [SerializeField]
    float patrolSpeed = 5f;

    int movingRight = 1;
    float patrolTimer = 2f;

	public void SetInAir(bool nu){
		inAir = nu;
	}

	void Aggressive(){
        if (patrolMode == 2) return;
		if (!inAir && jumpTimer > 3f) {
			jumpTimer = 0f;
			SetInAir (false);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			gameObject.GetComponent<Rigidbody2D> ().AddForce (Vector2.up * 400);
            patrolTimer = -3f;
		}
		jumpTimer += Time.deltaTime;
	}
	void Passive(){
		jumpTimer = 2.8f;
        Patrol();
	}

    void Patrol() {
        if (patrolMode == 3)
            return;
        if (patrolTimer >= 2f && patrolMode != 1) {
            GetComponent<Rigidbody2D>().velocity = new Vector2(movingRight * patrolSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }
        else if (patrolMode == 1)
        {
            GetComponent<Rigidbody2D>().velocity = transform.right * patrolSpeed;
            patrolBounds = new Vector2(GetPlayer().transform.position.x - 10, GetPlayer().transform.position.x + 10);
        }
        if ((transform.position.x >= patrolBounds.y && movingRight == 1) || (transform.position.x <= patrolBounds.x && movingRight == -1)) {
            patrolTimer = 0f;
            switch (patrolMode) {
                case 0:
                    movingRight *= -1;
                    break;
                case 1:
                    transform.position = GetPlayer().transform.position;
                    transform.Rotate(0, 0, Random.Range(0, 360));
                    transform.position += transform.right * -10;
                    patrolTimer = 2f;
                    break;
                case 2:
                    movingRight *= -1;
                    break;
            }
        }
        patrolTimer += Time.deltaTime;
    }

    private void OnEnable()
    {
        if (patrolBounds == Vector2.zero) {
            Debug.Log("Had to generate patrol bounds");
            patrolBounds = new Vector2(transform.position.x - 2, transform.position.x + 2);
        }
    }

    new void Update(){
		switch (GetAggro()) {
		case true:
			Aggressive ();
			break;
		case false:
			Passive ();
			break;
		}

		if(GetHealth() <= 0){
			//play particle and reward player
			if(GetAggro()) FindObjectOfType<EnemyCounterBehavior> ().SetCount(FindObjectOfType<EnemyCounterBehavior> ().GetCount()-1);
			Destroy(gameObject);
            return;
		}
        SetColor();
    }
}
