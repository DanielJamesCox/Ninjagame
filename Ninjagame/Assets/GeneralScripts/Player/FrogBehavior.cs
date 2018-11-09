using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehavior : SmokeBombBehavior {

    int health = 50;
    int maxHealth = 50;
    PlayerStateMachine player;

    private void Start()
    {
        player = FindObjectOfType<PlayerStateMachine>();
        if (player.frogHealth < health) TakeDamage(health - player.frogHealth);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        return;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.LookAt(collision.contacts[0].point);
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, transform.rotation.w);
        //Debug.Log (transform.rotation.eulerAngles.z);
        if (Mathf.Abs(transform.rotation.eulerAngles.z) > 180)
            transform.Rotate(0, 0, 90);
        else
            transform.Rotate(0, 0, -90);
        if (collision.contacts[0].point.y > transform.position.y)
            transform.Rotate(0, 0, 180);
        Collider2D other = collision.collider;
        switch (other.tag){
            case "Enemy": GetComponent<Rigidbody2D>().velocity *= -1; break;
            case "Untagged": GetComponent<Rigidbody2D>().Sleep();
                break;
            case "Player":
                player.frogHealth = health;
                player.smokeAmmo = 1;
                Destroy(gameObject);
                break;
        }
    }
    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            FindObjectOfType<PlayerStateMachine>().TakeDamage(100);
        }
    }
    public int GetHealth() {
        return health;
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position,player.transform.position) > .6f) GetComponent<BoxCollider2D>().enabled = true;
    }
}
