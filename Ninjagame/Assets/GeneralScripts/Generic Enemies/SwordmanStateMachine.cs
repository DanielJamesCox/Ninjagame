using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordmanStateMachine : EnemyStateMachine {

	bool inAir = false;
	[SerializeField]
	float jumpTimer = 2.8f;
	[SerializeField]
	float slashGoal = 1f;
    float slashTimer = 0f;
	[SerializeField]
	float jumpGoal = 3f;
    [SerializeField]
    bool isScared = false; //check for if the swordman is really a scout, who will jump away from the player, and spawn a wave and dissapear after getting off-screen
    [SerializeField]
    bool isEscaping = false; //specific check for if the scout is the paperboy for a stealth mission, who will simply go on one specific path and survive after spawning a wave
    bool hasBeenSeen = false; //set once the player gets into aggro range
	Vector2 target;

	[SerializeField]
	GameObject pointer;
	[SerializeField]
	SlashBehavior slash;
    [SerializeField]
    WaveBehavior wave;

	public void SetInAir(bool nu){
		inAir = nu;
	}

    public void Jump() {
        jumpTimer = 0f;
        SetInAir(false);
        target = new Vector2(GetPlayer().transform.position.x, GetPlayer().transform.position.y);
        if (!isScared)
        {
            target.y += 2;
            pointer.transform.LookAt(new Vector3(target.x, target.y, pointer.transform.position.z));
        }
        else
        {
            target.y -= 2;
            pointer.transform.LookAt(new Vector3(target.x, target.y, pointer.transform.position.z));
            pointer.transform.Rotate(180, 0, 0);

        }
        GetComponent<Rigidbody2D>().AddForce(pointer.transform.forward * 500);
    }

    void Aggressive(){
        hasBeenSeen = true;
        if (isScared && !isEscaping) {
            Passive();
            return;
        }
		if (!inAir && jumpTimer > jumpGoal) {//Jump at or away from the player at a fixed interval
            Jump();
		}
        if (slashTimer > slashGoal && slashGoal > 0f) {//Slash at the player at a fixed interval if there is in fact an interval
			slashTimer = 0f;
			SlashBehavior slesh = Instantiate(slash);
			slesh.transform.localScale = new Vector2(Random.Range(1.25f,2f),0.3f);

			//determine X position of slash
			float playerX = GetPlayer ().transform.position.x;
			float targetX = playerX + Random.Range(-1.5f,1.5f);
			while(Mathf.Abs(targetX - playerX) < .5f) targetX = playerX + Random.Range(-1.5f,1.5f);
			//determine Y position of slash
			float playerY = GetPlayer ().transform.position.y;
			float targetY = playerY + Random.Range(-1.5f,1.5f);
			while(Mathf.Abs(targetY - playerY) < .5f) targetY = playerY + Random.Range(-1.5f,1.5f);

			slesh.transform.position = new Vector2 (targetX, targetY);
			slesh.transform.Rotate(0,0,Random.Range(0,359));
			slesh.transform.rotation = new Quaternion (0, 0, slesh.transform.rotation.z, slesh.transform.rotation.w);
		}
		jumpTimer += Time.deltaTime;
		slashTimer += Time.deltaTime;
	}
	void Passive(){
		jumpTimer = 2.8f;
        if (isScared && !isEscaping) {
            GetComponent<Rigidbody2D>().velocity =  new Vector2(transform.right.x * 3, GetComponent<Rigidbody2D>().velocity.y);
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
        SetColor();
        if (GetHealth() <= 0){
			//play particle and reward player
			if(GetAggro()) FindObjectOfType<EnemyCounterBehavior> ().SetCount(FindObjectOfType<EnemyCounterBehavior> ().GetCount()-1);
			Destroy(gameObject);
            return;
		}
        if (GetComponent<SpriteRenderer>().isVisible == false && isScared && hasBeenSeen) {
            WaveBehavior newWave = Instantiate(wave);
            Vector2 newWavePos = transform.position - GetPlayer().transform.position;
            newWave.SetOffset(newWavePos.x,newWavePos.y+2f, 0f);
            newWave.OnEnable();
            if (isEscaping) Destroy(gameObject);
            else hasBeenSeen = false;
        }
	}
}
