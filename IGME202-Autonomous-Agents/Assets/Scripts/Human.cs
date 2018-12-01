using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle {

	public float HumanMaxSpeed = 5f;
	ZombieManager zombieManager;
	HumanManager humanManager;

	
	////public Material material4;
	//public Material material5;

	GameObject zombie;

	public override Vector3 CalcSteeringForces() {
		zombieManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
		humanManager = GameObject.Find("HumanManager").GetComponent<HumanManager>();
		List<GameObject> humans = humanManager.HumanList;
		List<GameObject> zLst = zombieManager.ZombieList;
		Vector3 UltimateForce = Vector3.zero;
	
		for (int i = 0; i < zLst.Count; i++)
		{
			float dist = CalcDistance(zLst[i]);

			if (inBounds)
			{
				UltimateForce += Wander();
				
				if (dist < 5f)
				{
					UltimateForce += Flee(zLst[i]);
				}
			}
		}

		foreach (GameObject h in humans)
		{
			UltimateForce += Separation(h);
		}

		Debug.DrawLine(vehiclePosition, vehiclePosition + transform.forward, Color.green);
		UltimateForce = UltimateForce * HumanMaxSpeed;
		return UltimateForce;
    }

   float CalcDistance(GameObject obj) {
        return Vector3.Distance(obj.transform.position, transform.position);
    }

	public override void OnRenderObject()
	{
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

		}
	}
}
