using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowmanStateMachine : EnemyStateMachine {

	bool inAir = false;
	float shotTarget;
	[SerializeField]
	float shotTimer = 0f;
	[SerializeField]
	float aggroTimer = 2f;
	[SerializeField]
	float passiveTimer = 5f;
	Vector2 target;

	[SerializeField]
	int damage;
    [SerializeField]
    float arrowSpeed = 1;
	[SerializeField]
	GameObject pointer;
	[SerializeField]
	ArrowBehavior arrow;

	RaycastHit2D shot;
	[SerializeField]
	LayerMask mask;

	void Aggressive(){
		shotTarget = aggroTimer;
		if (shotTimer > shotTarget && shotTarget > 0) {//shoot at the player every two seconds

			//determine player position
			float playerX = GetPlayer ().transform.position.x;
			float playerY = GetPlayer ().transform.position.y;

			//make target mean something
			GenerateTarget(playerX,playerY);
            //raycast to target; if open air, shoot
            RaycastHit2D line = Physics2D.Linecast(transform.position, target, mask);
            if (!line)
            {
                ArrowBehavior arroz = Instantiate(arrow);
                arroz.SetBehavior(damage, transform.position, target, target - new Vector2(transform.position.x, transform.position.y), arrowSpeed);
                shotTimer = 0;
            }
		}
	}

	void Passive(){
		shotTarget = passiveTimer;
		if (shotTimer > shotTarget && shotTarget > 0) {
			target = new Vector2 (transform.position.x + 1*-(transform.right.normalized.x), transform.position.y + 1);
			ArrowBehavior arroz = Instantiate (arrow);
			arroz.SetBehavior (damage, transform.position, target, target - new Vector2(transform.position.x,transform.position.y),arrowSpeed);
			shotTimer = 0;
		}
	}

	void GenerateTarget(float x, float y){
		//determine X position of shot
		target.x = x + Random.Range(-1f,1f);
		//determine Y position of shot
		target.y = y + Random.Range(-1f,1f);
	}

	new void Update(){
		switch (GetAggro()) {
		case true:
             if (GetPlayer().stealth > 0) Passive();
            else Aggressive();
            break;
		case false:
			Passive ();
			break;
		}
		shotTimer += Time.deltaTime;
		if(GetHealth() <= 0){
			//play particle and reward player
			if(GetAggro()) FindObjectOfType<EnemyCounterBehavior> ().SetCount(FindObjectOfType<EnemyCounterBehavior> ().GetCount()-1);
			Destroy(gameObject);
            return;
		}
        SetColor();
    }
}
