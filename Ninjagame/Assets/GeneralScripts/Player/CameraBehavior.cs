using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	PlayerStateMachine player;
	Camera thisCamera;
	Vector3 zero;
	float relaxTimer = 0f;

	float currentSize = 5f;
	[SerializeField]
	float maxSize = 10f;
	[SerializeField]
	float minSize = 5f;
	[SerializeField]
	float[] camBounds;
    [SerializeField]
    bool endlessLevel = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindObjectOfType<PlayerStateMachine> ();
		thisCamera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 camPos = transform.position;
        Vector2 playPos = player.transform.position;
        //set relative position
        Vector3 currentVelocity = player.GetComponent<Rigidbody2D>().velocity;
        Vector3 camVelocity = Vector3.zero;
        if (currentVelocity.magnitude > 5)
        {
            zero += (currentVelocity.normalized * Time.deltaTime * 2);
            relaxTimer = 0f;
        }
        else
            relaxTimer += Time.deltaTime;

        //apply relative position, constrain to camera bounds if past them, move player if endless level
        transform.position = player.transform.position + zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        if (endlessLevel) {
            if (playPos.x < camBounds[0])
            {
                player.transform.position = new Vector2(camBounds[1]-.1f, player.transform.position.y);
                transform.position = player.transform.position + zero;
                transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            }
            else if (playPos.x > camBounds[1])
            {
                player.transform.position = new Vector2(camBounds[0]+.1f, player.transform.position.y);
                transform.position = player.transform.position + zero;
                transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            }
            else if (playPos.y < camBounds[2])
            {
                player.transform.position = new Vector2(player.transform.position.x, camBounds[3]-.1f);
                transform.position = player.transform.position + zero;
                transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            }
            else if (playPos.y > camBounds[3])
            {
                player.transform.position = new Vector2(player.transform.position.x, camBounds[2]+.1f);
                transform.position = player.transform.position + zero;
                transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            }
        }
        else
        {
            if (playPos.x < camBounds[0])
            {
                transform.position = new Vector3(camBounds[0], transform.position.y, -10);
            }
            else if (playPos.x > camBounds[1])
            {
                transform.position = new Vector3(camBounds[1], transform.position.y, -10);
            }
            else if (playPos.y < camBounds[2])
            {
                transform.position = new Vector3(transform.position.x, camBounds[2], -10);
            }
            else if (playPos.y > camBounds[3])
            {
                transform.position = new Vector3(transform.position.x, camBounds[3], -10);
            }
        }


        //scale currentSize
        if (currentSize < maxSize && (currentVelocity.magnitude > 15 || zero.magnitude > 5))
            {
                currentSize += Time.deltaTime;
            }

            if (relaxTimer >= 1f)
            {
                zero = Vector3.SmoothDamp(zero, Vector3.zero, ref camVelocity, .1f);
                currentSize -= Time.deltaTime * 6;
            }

        //constrain scale
        if (currentSize > maxSize)
            currentSize = maxSize;
        else if (currentSize < minSize)
            currentSize = minSize;

        //apply scale
        thisCamera.orthographicSize = currentSize;
    }
}
