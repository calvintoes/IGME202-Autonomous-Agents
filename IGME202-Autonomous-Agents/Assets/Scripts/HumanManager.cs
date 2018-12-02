using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour {

	public List<GameObject> HumanList = new List<GameObject>();
	public GameObject human;
	public ZombieManager ZombieManager;
	public GameObject zombie;

	//public int amt; have user input??
	
	// Use this for initialization
	void Start () {

		for (int i = 0; i < 10; i++)
		{
			HumanList.Add(Instantiate(human, new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f)), Quaternion.identity));
		
		}

		foreach (GameObject item in HumanList)
		{
			if (HumanList.Count != 0)
			{
				item.GetComponent<Human>().mass = 1;
			}
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		Infected();
	}

	void Infected() {
		List<GameObject> zombies = ZombieManager.ZombieList;
		if (HumanList.Count != 0)
		{
			for (int i = 0; i < HumanList.Count; i++)
			{
				for (int j = 0; j < zombies.Count; j++)
				{
					if (CollisionsWithZombie(HumanList[i], zombies[j]))
					{
						GameObject zombieInstance;
						Vector3 zombiePos = zombies[j].transform.position;
						Destroy(HumanList[i]);
						HumanList.RemoveAt(i);
						zombieInstance = Instantiate(zombie, zombiePos, Quaternion.identity);
						zombieInstance.GetComponent<Zombie>().mass = 1;

					}
				}
			}
		}
	}

	public bool CollisionsWithZombie(GameObject human, GameObject zombie)
	{
		Human h = human.GetComponent<Human>();
		Zombie z = zombie.GetComponent<Zombie>();

		float totalDistance = Vector3.Magnitude(zombie.transform.position - human.transform.position);


		if (totalDistance > (h.radius + z.radius))
		{
			return false;
		}
		else
		{
			return true;
		}
	}	
}
