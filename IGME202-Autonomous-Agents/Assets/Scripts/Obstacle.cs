using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	public float radius;
	public Vector3 obstaclePosition;

	
	// Use this for initialization
	void Start () {
		obstaclePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
