using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBehavior : MonoBehaviour {

    [SerializeField]
    EnemyStateMachine[] enemies;
    [SerializeField]
    Vector3 playerOffset;

    // Use this for initialization
    public void OnEnable()
    {
        transform.position = FindObjectOfType<PlayerStateMachine>().transform.position + playerOffset;
        foreach (EnemyStateMachine enemy in enemies) {
            EnemyStateMachine clone = Instantiate(enemy);
            clone.transform.position = transform.position;
            clone.transform.Translate(new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0));
            clone.GetComponent<Rigidbody2D>().AddForce((FindObjectOfType<PlayerStateMachine>().transform.position - clone.transform.position).normalized * 10);
        }
        Destroy(gameObject);
    }
    public void SetOffset(float x, float y, float z) {
        playerOffset = new Vector3(x, y, z);
    }
}
