using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour {

    [SerializeField]
    WaveBehavior wave;
    bool isTripped = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isTripped) {
            isTripped = true;
            WaveBehavior wav = Instantiate(wave);
            Destroy(gameObject);
        }
    }
}
