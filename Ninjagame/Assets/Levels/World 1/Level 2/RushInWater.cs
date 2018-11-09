using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushInWater : MonoBehaviour {
	[SerializeField]
	GameObject canary;
	[SerializeField]
	StillWaterProperties me;
    [SerializeField]
    RunningWaterProperties alsoMe;
	// Update is called once per frame
	void Update () {
		if (canary == null) {
            if(me != null)
		    	me.enabled = true;
            if (alsoMe != null)
                alsoMe.enabled = true;
            GetComponent<Renderer>().enabled = true;
			GetComponent<Collider2D> ().enabled = true;
		}
	}
}
