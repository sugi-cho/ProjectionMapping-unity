using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportPos : MonoBehaviour {
    public Vector3 viewPortPos;
    public Transform targetPoint;
    Camera c;
	// Use this for initialization
	void Start () {
        c = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        viewPortPos = c.WorldToViewportPoint(targetPoint.position);
	}
}
