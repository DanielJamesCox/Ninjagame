using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustSupportBehavior : MonoBehaviour {

    PolygonCollider2D parent;

    private void Start()
    {
        parent = gameObject.transform.parent.GetComponent<PolygonCollider2D>();
        GetComponent<PolygonCollider2D>().points = parent.points;
    }
}
