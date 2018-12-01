using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour {

	public List<GameObject> ZombieList = new List<GameObject>();
	public GameObject zombie;
	//public int amt; have user input??

	GameObject zombieInstance;
	// Use this for initialization
	void Start () {

		for (int i = 0; i < 5; i++)
		{
			zombieInstance = Instantiate(zombie, new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity);
			zombieInstance.GetComponent<Zombie>().mass = 1;
			ZombieList.Add(zombieInstance);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}	
