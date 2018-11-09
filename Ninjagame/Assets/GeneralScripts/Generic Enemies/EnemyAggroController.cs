using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroController : MonoBehaviour {

	[SerializeField]
	EnemyStateMachine papa;
	[SerializeField]
	EnemyCounterBehavior count;

	void OnTriggerEnter2D(Collider2D other){
		if (count.GetCount () < 4 && other.tag == "Player" && papa.GetAggro() == false) {
			count.SetCount (count.GetCount () + 1);
			papa.SetAggro (true);
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player" && papa.GetAggro() == true) {
			count.SetCount (count.GetCount () - 1);
			papa.SetAggro (false);
		}
	}

	// Use this for initialization
	void Start () {
		count = FindObjectOfType<EnemyCounterBehavior> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
