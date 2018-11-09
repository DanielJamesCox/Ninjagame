using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour
{

    [SerializeField]
    ArrowBehavior explosion;
    [SerializeField]
    string wanted = "Player";
    bool debounce = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!debounce && (other.tag != "Enemy" && other.isTrigger == false) || other.tag == "Player")
        {
            ArrowBehavior exp = Instantiate(explosion, transform);
            exp.SetBehavior(0, transform.position, 0, 0);
            debounce = true;
            Destroy(gameObject);
        }
    }
}
