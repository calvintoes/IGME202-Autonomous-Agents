﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {

	public List<GameObject> obstacles = new List<GameObject>();
	public GameObject obstacle;
	float obstacleAmt = 20;
	
	// Use this for initialization
	void Start () {

		for (int i = 0; i < obstacleAmt; i++)
		{
			obstacles.Add(Instantiate(obstacle, new Vector3(Random.Range(-25f, 25f), transform.position.y, Random.Range(-25f, 25f)), Quaternion.identity));
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
