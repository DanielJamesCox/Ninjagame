using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour {

    SpriteMask mask;

    private void Start()
    {
        mask = transform.GetChild(1).GetComponent<SpriteMask>();
    }


    public void ToggleLights() {
        mask.enabled = !mask.enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerStateMachine>().stealth != 2)
        {
            other.GetComponent<PlayerStateMachine>().stealth = 0;
        }
        else if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStateMachine>().stealth = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerStateMachine>().stealth != 2)
        {
            if (!mask.enabled) {
                other.GetComponent<PlayerStateMachine>().stealth = 1;
                return;
            }
            other.GetComponent<PlayerStateMachine>().stealth = 0;
        }
        else if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStateMachine>().stealth = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<PlayerStateMachine>().stealth != 2)
        {
            other.GetComponent<PlayerStateMachine>().stealth = 1;
        }
        else if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStateMachine>().stealth = true;
        }
    }
}
