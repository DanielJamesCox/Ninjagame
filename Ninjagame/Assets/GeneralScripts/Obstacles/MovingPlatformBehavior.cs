using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehavior : MonoBehaviour {

    [SerializeField]
    Vector2[] points;
    [SerializeField]
    int pointIndex = 0;
    [SerializeField]
    float slowness = 2f;
    [SerializeField]
    float debounceGoal = 2f;
    [SerializeField]
    float debounceTimer = 0f;
    float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (debounceGoal > debounceTimer) {
            debounceTimer += Time.deltaTime;
            transform.tag = "Untagged";
        }
        else
        {
            Vector2 point1; Vector2 point2;
            if (pointIndex == points.Length - 1)
            {
                point1 = points[pointIndex];
                point2 = points[0];
            }
            else
            {
                point1 = points[pointIndex];
                point2 = points[pointIndex + 1];
            }
            if (Vector2.Distance(transform.position, point2) < .1f)
            {
                pointIndex++;
                if (pointIndex >= points.Length) pointIndex = 0;
                debounceTimer = 0f;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                float speed = (1 + (Vector2.Distance(transform.position, point1) * Vector2.Distance(transform.position, point2)) / 2);
                Vector2 direction = (point2 - new Vector2(transform.position.x, transform.position.y)).normalized;
                GetComponent<Rigidbody2D>().velocity = new Vector3(direction.x, direction.y, 0) * speed / slowness;
            }
            transform.tag = "Moving";
        }
	}
}
