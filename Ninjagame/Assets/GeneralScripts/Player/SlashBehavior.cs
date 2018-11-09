using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashBehavior : MonoBehaviour {

	float deathTimer = 0f;
	public float liveGoal = .5f;
	public float deathGoal = 2f;

    float width;
    bool hasCast = false;

	[SerializeField]
	public string wanted;
	[SerializeField]
	LayerMask mask;
	[SerializeField]
	int damage = 30;

	Vector2 castA; Vector2 castB;

	void OnTriggerEnter2D(Collider2D other){
		if (deathTimer > .5 && other.tag == wanted) {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerStateMachine>().TakeDamage(damage);
            }
            else if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<EnemyStateMachine>().TakeDamage(damage);
            }
        }
	}

	public void SetEdges(Vector2 a, Vector2 b){
		castA = a; castB = b;
        for (int i = 0; i < GetComponent<LineRenderer>().positionCount; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, new Vector3((.5f - (float)(i / 10f) + Random.Range(0, .1f)), 0, 0));
        }
    }

    private void Start()
    {
        width = GetComponent<LineRenderer>().widthMultiplier;
    }

    // Update is called once per frame
    void Update () {

        if (deathTimer < deathGoal)
        {
            deathTimer += Time.deltaTime;
            GetComponent<LineRenderer>().widthMultiplier = width * (1 - (deathTimer/liveGoal));
        }
        else
        {
           Destroy(gameObject);
        }
        if (deathTimer >= liveGoal)
        {
            RaycastHit2D line = Physics2D.Linecast(castA, castB, mask);
            GetComponent<LineRenderer>().widthMultiplier = width;
            if (line) {
                if ((line.collider.tag == "Killable" || line.collider.tag == "Arrow") && line.collider != GetComponent<BoxCollider2D>())
                {
                    //play particle on Killable's gameObject
                    Destroy(line.collider.gameObject);
                }
                else if ((line.collider.tag == "Enemy" || line.collider.tag == "Rock") && !hasCast) {
                    hasCast = true;
                    line.collider.gameObject.GetComponent<EnemyStateMachine>().TakeDamage(damage);
                }
                if (line.collider.tag == "Player" && !hasCast)
                {
                    hasCast = true;
                    line.collider.gameObject.GetComponent<PlayerStateMachine>().TakeDamage(damage);
                }
                if(line.collider.tag == "Frog" && !hasCast)
                {
                    hasCast = true;
                    line.collider.gameObject.GetComponent<FrogBehavior>().TakeDamage(damage/2);
                }
                if(line.collider.tag == "Lightswitch" && !hasCast)
                {
                    hasCast = true;
                    line.collider.GetComponent<LightswitchBehavior>().ToggleLight();
                }
            }
        }
	}
}
