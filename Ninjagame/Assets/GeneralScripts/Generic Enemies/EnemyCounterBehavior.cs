using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounterBehavior : MonoBehaviour {

	[SerializeField]
	int count;

	public int GetCount(){return count;}
	public void SetCount(int c){count = c;}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
