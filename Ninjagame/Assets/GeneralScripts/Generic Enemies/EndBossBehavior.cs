using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBossBehavior : EnemyStateMachine {

    PushToJumpBehavior[] pushers;

    //variables involved in combat
    float attackGoal = 5f;
    float attackTimer = 0f;
    int attackChoice = 0;

    int arrowWaves = 0;
    float arrowTimer = 0f;
    [SerializeField]
    ArrowBehavior shot;
    [SerializeField]
    SlashBehavior beam;

    int phase = 0;
    [SerializeField]
    int enemyType = 0; //0 for first emperor boss, 1-3 for aspects, 4 for true emperor's hands, 5 for true emperor's head
    int collisionType = 0; //0 for nothing, 1 for jump, 2 for dash
    int healingAmount = 0;

    void Passive() {
    }

    void Aggressive() {
        if (attackTimer == attackGoal)
        {
            if (phase == 0 && enemyType == 0) attackChoice = Random.Range(1, 3);
            else if (phase == 1 && enemyType == 0) attackChoice = Random.Range(1, 4);
            else if (enemyType > 0 && enemyType < 4)
            {
                switch (enemyType)
                {
                    case 1:
                        if (Random.Range(0, 1) > 0.2f) attackChoice = 1;
                        else attackChoice = 4;
                        break;
                    case 2: attackChoice = 2; break;
                    case 3: attackChoice = 3; break;
                }
            }
            else if (enemyType == 4) attackChoice = 1;
            else if (enemyType == 5) attackChoice = Random.Range(5, 6);
            switch (attackChoice)
            {
                case 1: //banana slamma
                    GetAnimRig().SetTrigger("Slam");
                    attackTimer = 0f;
                    //attackGoal = attack 1 cooldown;
                    break;
                case 2: //make it rain
                    arrowWaves = 2;
                    attackTimer = 0f;
                    //attackGoal = attack 2 cooldown;
                    break;
                case 3: //bomberman
                    DashAttack();
                    attackTimer = 0f;
                    //attackGoal = attack 3 cooldown;
                    break;
                case 4: //pussy out
                    if (enemyType == 0) healingAmount = 300 - GetHealth();
                    else healingAmount = 100 - GetHealth();
                    if (healingAmount < 0) healingAmount = 0;
                    attackTimer = 0f;
                    //attackGoal = attack 4 cooldown;
                    break;
                case 5: //make it rainer
                    arrowWaves = Random.Range(5, 7);
                    attackTimer = 0f;
                    //attackGoal = attack 5 cooldown;
                    break;
                case 6: //shoop da whoop
                    //animrig.SetTrigger("Slam");
                    attackTimer = 0f;
                    //attackGoal = attack 6 cooldown;
                    break;
            }
        }
        else {
            attackTimer += Time.deltaTime;
        }
        if (arrowWaves > 0) {
            ArrowAttack();
        }
        if (GetHealth() <= GetMaxHealth() / 2) {
            phase = 1;
        }
    }

    #region Attack Behaviors
    void PusherAttack() {
        foreach (PushToJumpBehavior push in pushers) {
            push.ToggleActive();
        }
    }
    void ArrowAttack() {
        if (arrowTimer < 0f)
        {
            for (int i = 0; i < 360; i += 30)
            {
                ShootArrow((float)i + (arrowWaves * 15));
                arrowTimer = .75f;
                arrowWaves--;
            }
        }
        else arrowTimer -= Time.deltaTime;
    }
    void DashAttack() {
        GetAnimRig().SetTrigger("Dash");
        float direction = (transform.position.x - GetPlayer().transform.position.x) / Mathf.Abs(transform.position.x - GetPlayer().transform.position.x);
        GetComponent<Rigidbody2D>().velocity = new Vector2(direction * 10, 0);
        if (enemyType < 4) {
            if (direction < 0) GetComponent<SpriteRenderer>().flipX = true;
            else GetComponent<SpriteRenderer>().flipX = false;
        }
        collisionType = 2;
    }
    void BeamAttack(){
        GetAnimRig().SetTrigger("Beam");
        SlashBehavior beamu = Instantiate(beam);
        Vector3 offset = Vector2.zero;
        Vector2 playerPos = GetPlayer().transform.position;
        if (playerPos.y > transform.position.y && Mathf.Abs(transform.position.y - playerPos.y) < Mathf.Abs(transform.position.x - playerPos.x))
            offset += new Vector3(0, 100, 0);
        if (playerPos.y < transform.position.y && Mathf.Abs(transform.position.y - playerPos.y) < Mathf.Abs(transform.position.x - playerPos.x))
            offset += new Vector3(0, -100, 0);
        if (playerPos.x > transform.position.x && Mathf.Abs(transform.position.y - playerPos.y) > Mathf.Abs(transform.position.x - playerPos.x))
            offset += new Vector3(0, 100, 0);
        if (playerPos.x < transform.position.x && Mathf.Abs(transform.position.y - playerPos.y) > Mathf.Abs(transform.position.x - playerPos.x))
            offset += new Vector3(0, -100, 0);
        offset = transform.position + offset;
        beamu.SetEdges(transform.position, offset);
        beamu.transform.localScale = new Vector2(Vector2.Distance(transform.position,offset), 1f);
        beamu.transform.position = new Vector2((transform.position.x + offset.x) / 2, (transform.position.y + offset.y) / 2);
        beamu.transform.LookAt(transform.position);
        beamu.transform.rotation = new Quaternion(0, 0, beamu.transform.rotation.z, beamu.transform.rotation.w);
    }
    #endregion
    #region Specific Behaviors
    void ShootArrow(float angle) {
        ArrowBehavior currentShot = Instantiate(shot);
        currentShot.SetBehavior(30, transform.position, angle, 5f);
    }
    public void MinorJump() {
        //determine player direction
        //addforce towards player
        //change sprite direction towards player
        //change collision mode to jump
    }
    public void MajorJump() {
        //play particle and teleport to player y-level + player verticle velocity
        DashAttack();
    }
    #endregion
}
