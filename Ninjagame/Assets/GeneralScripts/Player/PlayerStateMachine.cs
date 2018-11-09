using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStateMachine : MonoBehaviour {

	#region PrivateVariables
	short swordDamage = 30; //damage of slashes
	int smokeRange; //size of smokebomb
	bool isDead = false; //am i dead??/

    public int stealth = 0; //0 for revealed, 1 for in the dark, 2 for in smoke;

    Vector2 targetA; //target used for single taps; jumps or hookshots
	Vector2 targetB; //target used for swipes; slashes or throws
    [SerializeField]
    bool isSecondary; //is this touch a swipe?
    [SerializeField]
    float holdTimer = 0f; //amount of time holding down a touch used for throwing objects
    [SerializeField]
    bool tapOnPlayer = false; //whether or not a touch began on the player model

    float impactTimer; //once the player hits a surface, they're stopped for a certain amount of time in order to walljump
	float tapDebounce = 1f; //make the player wait a certain amount of time between touches
	float momentum; //magnitude of velocity used for impact damage

	RaycastHit2D hookHit; //cast a ray to see if a hookshot hit anything
	RaycastHit2D ropeA; RaycastHit2D ropeB;
	//[SerializeField]
	List<Vector2> ropeTargets = new List<Vector2> ();

    Animator animrig; //empty reference to animator controller, populated on start
	GameObject rock; //empty reference to roller rock, find in OnTriggerEnter2D and use in Rock
    float tapTimer = 0f; //count of taps while in Rock
	float currentSize = 5f; //Current camera size
	float spriteTimer = 0f; //some sprites take time to come back
	float deadTimer = 0f;
	#endregion

	#region PublicVariables
	#region BasicStats
	[SerializeField]
	int health = 100; //if this is 0, die
	[SerializeField]
	int maxHealth = 100; //highest health can go
	[SerializeField]
	float jumpStrength; //full strength of addforce of jump
	[SerializeField]
	float tapGoal; //goal for tapTimer
	#endregion
	#region Battle
	[SerializeField]
	GameObject pointer; //turn this around to direct hookshots and jumps
	[SerializeField]
	SlashBehavior slosh; //reference slash
	[SerializeField]
	HookBehavior hook; //reference hook
    public int hookAmmo = 5; //amount of hooks in player's inventory
	[SerializeField]
    SmokeBombBehavior pellet; //reference smoke bomb
    public int smokeAmmo = 3; //amount of smokebombs in player's inventory
    public int frogHealth = 50; //amount of health the frog has if it's there
	[SerializeField]
	Camera cam; // reference to main camera
	[SerializeField]
	bool isUnderwater = false; //determined by StillWaterBehavior
	[SerializeField]
	bool pushed = false;
    [SerializeField]
    int kickDamage = 10;
    #endregion
    #region RopeSwinging
    HookBehavior currentHook; //hook player is currently attached to
	[SerializeField]
	DistanceJoint2D joint; //reference DistanceJoint2D
	[SerializeField]
	LineRenderer rope; //reference line renderer
	[SerializeField]
	LayerMask mask; //control what hookHit can look for
	[SerializeField]
	float hookDistance = 5f; //length of rope
	[SerializeField]
	float hookMaxDistance = 5f;
    #endregion
    #region Cosmetic
    Color color = Color.white;
	[SerializeField]
	Sprite[] playerSprites; //1 for idle, 2 for in_air, 3 and 4 for slash, 5 for hookshot, 6 for rock
	[SerializeField]
	Sprite[] sashSprites; //same as playerSprites
	[SerializeField]
	SpriteRenderer ninjaRobe; //player sprite
	[SerializeField]
	SpriteRenderer sash; //sash sprite
	#endregion
	#endregion

	#region StateMachine
	enum ControlState{ //state list
		IDLE,
		IN_AIR,
		WALKING,
		SLASH,
		HOOKSHOT,
		ROCK
	}
	[SerializeField]
	ControlState curState;

	ControlState GetCurrentState(){
		return curState;
	}

	void SetCurrentState(ControlState nu){
		curState = nu;
	}
	#endregion

	#region StateBehaviors
	void Idle(){
		momentum = 0;
		Move ();
	}

	void InAir(){
		momentum = gameObject.GetComponent<Rigidbody2D> ().velocity.magnitude;
        GetComponent<Rigidbody2D>().freezeRotation = false;
		Move ();
	}

	void Walking(){
		Move ();
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;
        //GetComponent<Rigidbody2D>().velocity = new Vector2(Vector2.Distance(targetA, targetB)/3, vel.y);
        GetComponent<Rigidbody2D>().velocity = transform.right * (targetB.x - targetA.x)/2;
    }

    void Slash(){
		//Determine which animation to play
        animrig.SetTrigger("Slash");
        animrig.SetInteger("RNG", (int)Random.Range(0f, 3f));
		spriteTimer = 0f;
		//Spawn a Slash object, set its endpoints, reset cursor targets, and go back to IN_AIR
		SpawnSlash (targetA, targetB);
		targetA = Vector2.zero;
		targetB = Vector2.zero;
        if (GetComponent<Rigidbody2D>().velocity.magnitude > 0f)
            SetCurrentState(ControlState.IN_AIR);
        else SetCurrentState(ControlState.IDLE);
	}

	void Throw(){
        if (smokeAmmo > 0)
        {
            smokeAmmo--;
            SpawnSmoke(targetB);
        }
		targetA = Vector2.zero;
		targetB = Vector2.zero;
	}

	void Hookshot(){
		ninjaRobe.sprite = playerSprites [4];
		sash.sprite = sashSprites [4];
		HookMove ();
		targetA = Vector2.zero;
		targetB = Vector2.zero;
	}

	void Rock(){
		ninjaRobe.sprite = playerSprites [5];
		sash.sprite = sashSprites [5];
		if (Input.touchCount > 0)
			tapTimer+= Input.touchCount;
		if (tapTimer > tapGoal) {
			joint.connectedBody = null;
			joint.enabled = false;
			SetCurrentState (ControlState.IDLE);
			for(int x = 0; x < GetComponents<CircleCollider2D>().GetLength(0);x++) {
				if(GetComponents<CircleCollider2D>()[x].isTrigger == false)
					GetComponents<CircleCollider2D>()[x].enabled = true;
			}
            Jump(targetA = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position),jumpStrength);
		}
		if(tapTimer > 0f) tapTimer -= .5f;
	}
	#endregion

	#region SecondaryBehaviors
	public void SetIsUnderwater(bool f){//for StillWater objects to use
		isUnderwater = f;
	}

	void Move(){
		if (Input.touchCount > 0 && tapDebounce > .2f) {
			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                //RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction);
                  //  if (rayhit == true && rayhit.collider.tag == "Player") {
                    //    tapOnPlayer = true;
                    //}
                isSecondary = false;
				targetA = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
                    if (Vector2.Distance(targetA, (Vector2)transform.position) < 2) tapOnPlayer = true;
				tapDebounce = 1f;
				break;
            case TouchPhase.Stationary:
                    if (tapOnPlayer && !isSecondary)
                    {
                        holdTimer += Time.deltaTime;
                    }
                    else if (tapOnPlayer && isSecondary && curState != ControlState.IN_AIR && holdTimer < .25f) {
                        SetCurrentState(ControlState.WALKING);
                    }
                break;
			case TouchPhase.Moved:
				isSecondary = true;
                if (targetA == Vector2.zero) {
                    targetA = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }
				targetB = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
				tapDebounce += Vector2.Distance (targetA, targetB);
                    if (tapOnPlayer && curState != ControlState.IN_AIR && holdTimer < .25f) {
                        SetCurrentState(ControlState.WALKING);
                    }
				break;
			case TouchPhase.Ended:
				//After a swipe
				if (isSecondary) {
                        if (tapOnPlayer) {
                            if (holdTimer > .25f) {//throw a grenade if touch was held on player more than a second
                                Throw();
                            }
                            else if (holdTimer < .25f) {//do nothing if the touch was held on player less than a second
                            }
                            tapDebounce = 0f;
                            holdTimer = 0f;
                            isSecondary = false;
                            tapOnPlayer = false;
                            break;
                        }
                        Slash(); //slash if swipe was between two points in the air
                    } 
				//After a tap
				else {
					if (curState == ControlState.IN_AIR && !isUnderwater) { //Shoot hook or spin player if you were in the air
                        if (tapOnPlayer) //if the player was tapped, spin
                            {
                                float angular = GetComponent<Rigidbody2D>().angularVelocity;
                                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                                Jump(transform.position + Vector3.up, jumpStrength);
                                if(angular != 0)
                                    GetComponent<Rigidbody2D>().angularVelocity = 540 * (Mathf.Abs(angular) / angular);
                            }
                        else
                            {
                                SpawnHook (targetA);
                            }
                        }
					else { //Jump or heal if you were on the ground
                        if (holdTimer >= .01f) {
                             TakeDamage(-20);
                            //BandageAnim();
                        }
                        else Jump(targetA,jumpStrength);
					}
				}
                tapDebounce = 0f;
                holdTimer = 0f;
                isSecondary = false;
                tapOnPlayer = false;
				break;
			}
		}

	}

    public void HookCheck(Vector2 target,HookBehavior hoke) {
        hookHit = Physics2D.Raycast(transform.position, new Vector2(target.x - transform.position.x, target.y - transform.position.y), hookDistance, mask);
        if (hookHit)
        {
            joint.enabled = true;
            joint.connectedAnchor = hookHit.point;
            joint.distance = hookHit.distance;
            ropeTargets.Add(hookHit.point);
            rope.enabled = true;
            currentHook = hoke;
            SetCurrentState(ControlState.HOOKSHOT);
        }
    }

	void HookMove(){
		//break up the rope
		if(ropeTargets.Count > 1){
			for (int i = 1; i < ropeTargets.Count; i++) {
				if(Vector2.Distance(ropeTargets[i],ropeTargets[i-1])<.2){
					ropeTargets.RemoveAt (i);
					i--;
				}
			}
		}
		if(ropeTargets.Count > 1){
            for (int i = 0; i < ropeTargets.Count-1; i++)
            {
                Vector2 ropeTargetA = ropeTargets[i];
                ropeA = Physics2D.Linecast(transform.position, ropeTargetA, mask);
                if (ropeA.point == ropeTargetA)
                {
                    for (int j = i + 1; j < ropeTargets.Count; j++)
                    {
                        ropeTargets.RemoveAt(j);
                        if(ropeTargets.Count > j)
                            ropeTargetA = ropeTargets[j];
                        joint.distance = Vector2.Distance(transform.position, ropeTargetA);
                        joint.connectedAnchor = ropeTargetA;
                    }
                    joint.distance = Vector2.Distance(transform.position, ropeTargetA);
                    joint.connectedAnchor = ropeTargetA;
                    i = ropeTargets.Count;
                }
            }
		}
		Vector2 ropeTargetB = ropeTargets [ropeTargets.Count - 1];
		ropeB = Physics2D.Linecast (transform.position, ropeTargetB,mask);
		if (ropeB.collider != null && ropeB.point != ropeTargetB) {
			ropeTargets.Add (ropeB.point);
			joint.connectedAnchor = ropeB.point;
			joint.distance = ropeB.distance;
		}

		hookDistance = hookMaxDistance;
		for(int i = 1; i < ropeTargets.Count; i++) {
			if(ropeTargets[i] != Vector2.zero){
				hookDistance -= Vector2.Distance (ropeTargets [i], ropeTargets [i - 1]);
			}
		}
        if (joint.distance > hookDistance)
        {
            joint.distance = hookDistance;
        }
		//point the player toward the rope
		transform.LookAt (joint.connectedAnchor);
		transform.rotation = new Quaternion (0, 0, transform.rotation.z, transform.rotation.w);

		if (Input.touchCount > 0) {
			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
				targetA = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
				break;
			case TouchPhase.Moved:
				isSecondary = true;
				if (Mathf.Abs(Input.GetTouch (0).deltaPosition.y) > 0) {
					joint.distance+= -.1f * (Input.GetTouch(0).deltaPosition.y / Mathf.Abs(Input.GetTouch(0).deltaPosition.y));
					if (joint.distance > hookDistance)
						joint.distance = hookDistance;
				}
				if (Mathf.Abs(Input.GetTouch (0).deltaPosition.x) > 0) {
					GetComponent<Rigidbody2D> ().AddForce (Input.GetTouch (0).deltaPosition.normalized * 20);
				}
				break;
			case TouchPhase.Ended:
				if (!isSecondary) {
					joint.enabled = false;
					rope.enabled = false;
					ropeTargets.Clear();
					hookDistance = hookMaxDistance;
                        currentHook.KillYourself();
					SetCurrentState (ControlState.IN_AIR);
				}
				isSecondary = false;
				break;
			}
		}
	}

	void Jump(Vector2 targ, float strength){
        GetComponent<Rigidbody2D>().freezeRotation = false;
        pointer.transform.LookAt (new Vector3 (targ.x, targ.y, pointer.transform.position.z));
		GetComponent<Rigidbody2D> ().AddForce (pointer.transform.forward * strength);
		SetCurrentState (ControlState.IN_AIR);
        float difference = pointer.transform.forward.x * -1;
        if (difference >= 0){
            difference = 1;
            ninjaRobe.flipX = false;
        }
        else if (difference < 0) {
            difference = -1;
            ninjaRobe.flipX = true;
        }
        GetComponent<Rigidbody2D>().angularVelocity = 360 * difference;
        targetA = Vector2.zero;
        targetB = Vector2.zero;
    }

	void SpawnSmoke(Vector2 target){//Throw a smoke grenade at the current position
		SmokeBombBehavior bullet = Instantiate (pellet);
		bullet.transform.position = transform.position;
		pointer.transform.LookAt (new Vector3 (target.x, target.y, pointer.transform.position.z));
		bullet.GetComponent<Rigidbody2D> ().AddForce(pointer.transform.forward * 500);
	}

	void SpawnSlash(Vector2 start, Vector2 end){
		SlashBehavior slesh = Instantiate(slosh);
        slesh.SetEdges(start, end);
		slesh.transform.localScale = new Vector2(Vector2.Distance(start, end),1f);
		slesh.transform.position = new Vector2 ((start.x + end.x) / 2, (start.y + end.y) / 2);
		slesh.transform.LookAt (start);
		slesh.transform.rotation = new Quaternion (0, 0, slesh.transform.rotation.z, slesh.transform.rotation.w);
	}

	void SpawnHook(Vector2 target){
		HookBehavior hoke = Instantiate (hook);
        hoke.SetBehavior(transform.position, target, target - new Vector2(transform.position.x, transform.position.y), 15f);
    }

	public float GetJumpStrength(){
		return jumpStrength;
	}

	public void SetJumpStrength(float n)
	{
		jumpStrength = n;
	}

	public void TakeDamage(int amount){
        if (health - amount < 100 && health - amount > 0)
            health -= amount;
        else if (health - amount >= 100)
            health = 100;
        else if (health - amount <= 0)
            health = 0;
        if (pellet.tag == "Frog" && smokeAmmo > 0) {
            frogHealth -= amount;
            if (frogHealth < 0) health = 0;
        }
	}
	#endregion

	// Use this for initialization
	void Start () {
		joint = GetComponent<DistanceJoint2D> ();
		joint.enabled = false;
		joint.distance = hookDistance;
		rope = GetComponent<LineRenderer> ();
		rope.enabled = false;
        animrig = GetComponent<Animator>();
        if (pellet.tag == "Frog") smokeAmmo = 1;
	}

	void OnCollisionEnter2D (Collision2D pointer){
		Collider2D other = pointer.collider;
		if (curState != ControlState.HOOKSHOT && curState != ControlState.ROCK && other.tag != "Arrow" && (other.isTrigger == false || other.tag == "StillWater")) {
			transform.LookAt (pointer.contacts [0].point);
			transform.rotation = new Quaternion (0, 0, transform.rotation.z, transform.rotation.w);
			if (Mathf.Abs(transform.rotation.eulerAngles.z) > 180)
				transform.Rotate (0, 0, 90);
			else
				transform.Rotate (0, 0, -90);
			if (pointer.contacts [0].point.y > transform.position.y)
				transform.Rotate (0, 0, 180);
            GetComponent<Rigidbody2D>().freezeRotation = true;
        }
	}

	void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Spikes" || (pushed && other.tag != "Enemy"))
        {
            if (momentum > 1f)
                TakeDamage(Mathf.RoundToInt(momentum) * 5);
            pushed = false;
        }
        else if (other.tag == "Enemy" && GetCurrentState() == ControlState.IN_AIR)
        {
            other.GetComponent<EnemyStateMachine>().TakeDamage(kickDamage);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Jump(transform.position + Vector3.up, jumpStrength / 5);
        }
        else if (other.tag == "Hook")
        {
            HookCheck(other.transform.parent.position, other.transform.parent.GetComponent<HookBehavior>());
        }
        else if (other.tag == "Rock")
        {
            rock = other.gameObject;
            joint.connectedBody = rock.GetComponent<Rigidbody2D>();
            Vector2 newAnchor = transform.position - rock.transform.position;
            newAnchor.Scale(new Vector2(1 / rock.transform.localScale.x, 1 / rock.transform.localScale.y));
            joint.connectedAnchor = newAnchor;
            joint.distance = 0;
            joint.enabled = true;
            for (int x = 0; x < GetComponents<CircleCollider2D>().GetLength(0); x++)
            {
                if (GetComponents<CircleCollider2D>()[x].isTrigger == false)
                    GetComponents<CircleCollider2D>()[x].enabled = false;
            }
            SetCurrentState(ControlState.ROCK);
            tapTimer = 0f;
        }
        else if (other.tag == "Pusher")
        {
            pushed = true;
        }
        else if (curState != ControlState.HOOKSHOT && curState != ControlState.ROCK && other.tag != "Arrow" && (other.isTrigger == false || other.tag == "StillWater"))
        {
            SetCurrentState(ControlState.IDLE);
            //gameObject.transform.rotation = other.transform.rotation;
            gameObject.GetComponent<Rigidbody2D>().Sleep();
            impactTimer = 0;
        }
    }

	void OnTriggerStay2D(Collider2D other){
		if (curState != ControlState.HOOKSHOT && curState != ControlState.ROCK && (other.isTrigger == false || other.tag == "StillWater")) {
			SetCurrentState (ControlState.IDLE);
			GetComponent<Rigidbody2D> ().drag = 1.5f;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (curState != ControlState.HOOKSHOT && (other.isTrigger == false || other.tag == "StillWater" || other.tag == "RunningWater")) {
			GetComponent<Rigidbody2D> ().drag = 0f;
            SetCurrentState(ControlState.IN_AIR);
		}
	}

	// Update is called once per frame
	void Update () {
        if (isDead)
        {
            deadTimer += Time.deltaTime / 4;
            if (deadTimer >= 1)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        switch (curState)
        {
            case ControlState.IDLE:
                Idle();
                break;
            case ControlState.IN_AIR:
                InAir();
                break;
            case ControlState.WALKING:
                Walking();
                break;
            case ControlState.SLASH:
                Slash();
                break;
            case ControlState.HOOKSHOT:
                Hookshot();
                break;
            case ControlState.ROCK:
                Rock();
                break;
        }

		spriteTimer += Time.deltaTime;
        //transform.rotation = Quaternion.identity;

        //check health
        float remainingHealth = (float)health / maxHealth;
        ninjaRobe.GetComponent<SpriteRenderer>().color = new Color(color.r + ((1 - color.r) - ((1 - color.r) * remainingHealth)), color.g - (color.g - color.g * remainingHealth), color.b - (color.b - color.b * remainingHealth), color.a);
        /*if (health >= 51 && ninjaRobe.GetComponent<SpriteRenderer>().color == Color.magenta)
            ninjaRobe.GetComponent<SpriteRenderer>().color = Color.white;
        if (health > 0 && health <= 50){
			ninjaRobe.GetComponent<SpriteRenderer>().color = Color.magenta;
		}
		if (health <= 0 && !isDead) {
            //freeze player input and run death sequence
            ninjaRobe.GetComponent<SpriteRenderer>().color = Color.black;
			isDead = true;
		}*/
		

		//render rope
		if(rope.enabled){
			rope.positionCount = ropeTargets.Count+1;
			for (int i = 0; i < ropeTargets.Count; i++) {
				rope.SetPosition (i, ropeTargets [i]);
			}
			rope.SetPosition (rope.positionCount - 1, transform.position);
		}

		//check impact timer
		impactTimer += Time.deltaTime;
		if(impactTimer > 1){
			gameObject.GetComponent<Rigidbody2D> ().WakeUp ();
		}

		tapDebounce += Time.deltaTime;
	}
}
