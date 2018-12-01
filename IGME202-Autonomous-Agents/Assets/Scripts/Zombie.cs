using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle {

	HumanManager humanManager;
	ZombieManager zombieManager;
	
    public override Vector3 CalcSteeringForces()
    {
		humanManager = GameObject.Find("HumanManager").GetComponent<HumanManager>();
		zombieManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
		List<GameObject> lst = humanManager.HumanList;
		List<GameObject> zombies = zombieManager.ZombieList;


		Vector3 UltimateForce = Vector3.zero;
		if (lst.Count > 0)
		{
			UltimateForce += Seek(ClosestHuman());
		}
		else
		{
			UltimateForce += Wander();
		}

		foreach (GameObject item in zombies)
		{
			UltimateForce += Separation(item);
		}

		Debug.DrawLine(vehiclePosition, vehiclePosition + transform.forward, Color.yellow);
		UltimateForce = UltimateForce * maxSpeed;

		return UltimateForce;
    }


	float CalcDistance(GameObject obj)
	{
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	GameObject ClosestHuman() {
		humanManager = GameObject.Find("HumanManager").GetComponent<HumanManager>();
		List<GameObject> lst = humanManager.HumanList;

		GameObject a = null;
		if (lst.Count != 0)
		{
			
			List<float> distances = new List<float>();

			//create a list of distances 
			for (int i = 0; i < lst.Count; i++)
			{
				distances.Add(CalcDistance(lst[i]));
			}


			//find minimum distance from list of distances
			for (int i = 0; i < distances.Count; i++)
			{
				//float dist = CalcDistance(lst[i]);
				int min_idx = i;
				for (int j = 1; j < distances.Count; j++)
				{
					if (distances[j] < distances[min_idx])
					{
						min_idx = j;
					}
				}
				a = lst[min_idx];
			}
		}
		
		return a;
	}

	public override void OnRenderObject()
	{
		humanManager = GameObject.Find("HumanManager").GetComponent<HumanManager>();

		if (debuglines)
		{

			DebugForward.SetPass(0);

			// FORWARD LINE
			GL.Begin(GL.LINES);
			// Begin to draw lines
			GL.Vertex(vehiclePosition);
			// First endpoint of this line
			GL.Vertex(vehiclePosition + transform.forward);
			// Second endpoint of this line
			GL.End();
			//Finish drawing the line

			// RIGHT LINE
			// Set another material to draw this second line in a different color
			DebugRight.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(vehiclePosition);
			GL.Vertex(vehiclePosition + transform.right);
			GL.End();

			// FUTURE POSITIONS
			DebugFuture.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(vehiclePosition);
			GL.Vertex(vehiclePosition + velocity);
			GL.End();

			// LINE TO ZOMBIE

			if (humanManager.HumanList.Count > 0)
			{
				DebugChase.SetPass(0);
				GL.Begin(GL.LINES);
				GL.Vertex(vehiclePosition);
				GL.Vertex(ClosestHuman().transform.position);
				GL.End();
			}
			
		}
	}
}
