using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlavedEnemy : MonoBehaviour {

    [SerializeField]
    GameObject canary;
    [SerializeField]
    int mode = 0;
    [SerializeField]
    Vector3 piggyBackPos;

	// Update is called once per frame
	void Update () {
        if (canary == null && mode == 0) {
            Destroy(gameObject);
        }
        if (canary == null && mode == 1)
        {
            return;
        }
        if (mode == 1) {
            transform.position = canary.transform.position + piggyBackPos;
            GetComponent<EnemyStateMachine>().SetAggro(false);
        }
        
	}
}
