using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthBossStateMachine : EnemyStateMachine {

    [SerializeField]
    BombBehavior bomb; //normal bomb but all black
    [SerializeField]
    SmokeBombBehavior smokeBomb; //smoke bomb but includes bombbehavior
    bool isPhased = false;
    float throwTimer = 0f;
    [SerializeField]
    GameObject pointer;

    void Passive() {
        if (GetHealth() <= GetMaxHealth() / 2 && !isPhased) {
            isPhased = true;
            bomb = smokeBomb.GetComponent<BombBehavior>();
        }
        //movement code
        //occasionally throw a bomb at the player
        throwTimer += Time.deltaTime;
        if (throwTimer >= 5f) {
            print("Throwing bomb");
            throwTimer = 0f;
            BombBehavior bullet = Instantiate(bomb);
            bullet.transform.position = transform.position;
            pointer.transform.LookAt(GetPlayer().transform.position);
            bullet.GetComponent<Rigidbody2D>().AddForce(pointer.transform.forward * 500);
        }
    }

    private new void Update()
    {
        Passive();
        SetColor();
        if (GetHealth() <= 0)
        {
            //play particle and reward player
            Destroy(gameObject);
        }
    }
}
