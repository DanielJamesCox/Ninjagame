using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

	[SerializeField]
	int health = 100;
	[SerializeField]
	int maxHealth = 100;

    public bool stealth = false;

    public Color color;
    Animator animRig;

    PlayerStateMachine player;

	enum AggroState{ //state list
		PASSIVE,
		AGGRESSIVE
	}
		
	[SerializeField]
	AggroState curState = AggroState.PASSIVE;

	void Passive(){
		
	}
	void Aggressive(){
		
	}

	public bool GetAggro(){
		if (curState == AggroState.AGGRESSIVE)
			return true; else return false;
	}

	public void SetAggro(bool nu){
		if (nu)	curState = AggroState.AGGRESSIVE;
		else curState = AggroState.PASSIVE;
	}

	public int GetHealth(){
		return health;
	}

	public int GetMaxHealth(){
		return maxHealth;
	}

    public Animator GetAnimRig() {
        return animRig;
    }

	public PlayerStateMachine GetPlayer(){
		return player;
	}

	public void TakeDamage(int amount){
		health -= amount;
        if (amount < 0) {
            //colorOverlay.renderer.color = Color.red;
        }
	}

	void OnTriggerExit2D(Collider2D other){
	}

    public void SetColor() {
        float remainingHealth = (float)GetHealth() / GetMaxHealth();
        GetComponent<SpriteRenderer>().color = new Color(color.r + ((1 - color.r) - ((1 - color.r) * remainingHealth)), color.g - (color.g - color.g * remainingHealth), color.b - (color.b - color.b * remainingHealth), color.a);
    }

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerStateMachine> ();
        color = GetComponent<SpriteRenderer>().color;
        animRig = GetComponent<Animator>();
    }

	// Update is called once per frame
	public void Update () {
		switch (curState) {
		case AggroState.AGGRESSIVE:
            if (GetPlayer().stealth > 0) Passive();
            else Aggressive ();
			break;
		case AggroState.PASSIVE:
			Passive ();
			break;
		}
		if(health <= 0){
			//play particle and reward player
			Destroy(gameObject);
		}
	}
}
