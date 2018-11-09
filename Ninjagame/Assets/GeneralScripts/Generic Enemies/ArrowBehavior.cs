using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour {

	int damage;
	float deathTimer = 10f;

	void OnTriggerEnter2D(Collider2D other){
		GetComponent<Rigidbody2D> ().Sleep ();
		deathTimer = .5f;
		if (other.tag == "Player") {
			other.GetComponent<PlayerStateMachine> ().TakeDamage (damage);
			Destroy (gameObject);
		}
        if(other.tag == "Frog")
        {
            other.GetComponent<FrogBehavior>().TakeDamage(damage / 2);
            Destroy(gameObject);
        }
	}

    public void SetBehavior(int d, Vector2 o, Vector2 t, Vector2 v, float speed)
    {
        damage = d;
        transform.position = o;
        GetComponent<Rigidbody2D>().velocity = v.normalized * speed;
        transform.up = t - new Vector2(transform.position.x,transform.position.y);
    }
    public void SetBehavior(int d, Vector2 o, float a, float speed) {
        damage = d;
        transform.position = o;
        transform.Rotate(0, 0, a);
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
	// Update is called once per frame
	void Update () {
		deathTimer -= Time.deltaTime;
		if (deathTimer <= 1f) {
			Color shade = GetComponent<SpriteRenderer> ().color;
			GetComponent<SpriteRenderer> ().color = new Color (shade.r, shade.b, shade.b, deathTimer);
		}
		if (deathTimer < 0f)
			Destroy (gameObject);
	}
}
