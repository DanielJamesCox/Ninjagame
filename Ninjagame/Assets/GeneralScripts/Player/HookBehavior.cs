using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehavior : MonoBehaviour {

	[SerializeField]
	GameObject trail;
	[SerializeField]
	PlayerStateMachine player;
	[SerializeField]
	DistanceJoint2D joint;

    bool hooked = false;

	void Start(){
		player = FindObjectOfType<PlayerStateMachine> ();
	}

    public void SetBehavior(Vector2 o, Vector2 t, Vector2 v, float speed)
    {
        transform.position = o;
        GetComponent<Rigidbody2D>().velocity = v.normalized * speed;
        transform.up = t - new Vector2(transform.position.x, transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStateMachine>().TakeDamage(50);
        }
        else if (other.tag == "Moving") {
            KillYourself();
        }
        else if (hooked == false && other.isTrigger == false)
        {
            GetComponent<Rigidbody2D>().freezeRotation = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            hooked = true;
            player.HookCheck(transform.position, this);
        }
	}

    public void KillYourself() {
        Destroy(gameObject);
    }
}
