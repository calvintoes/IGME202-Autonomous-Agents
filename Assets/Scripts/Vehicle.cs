﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// In-class progress on 10/30
// Changed script for behaviors that AA (Autonomous Agents) will display
// Added: max speed, Seek methods, applying seek force
// Removed: Calling forces like wind or gravity

abstract public class Vehicle : MonoBehaviour
{
	// Vectors necessary for force-based movement
	protected Vector3 vehiclePosition;
	public Vector3 acceleration;
	public Vector3 direction;
	public Vector3 velocity;

	// Floats
	public float mass;
	public float maxSpeed = 8f;
	public float radius;
	public float safeDistance;
	private float randAngle;
	public float wanderRadius;
	private float angleRotation;

	//Bools
	protected bool inBounds = true;
	protected bool debuglines = false;

	//Materials
	public Material DebugForward;
	public Material DebugRight;
	public Material DebugFuture;
	public Material DebugChase;

	// Use this for initialization
	void Start()
	{
		vehiclePosition = transform.position;
	}

	// Update is called once per frame
	void Update()
	{

		Vector3 UltimateForce = CalcSteeringForces();
		UltimateForce += Boundaries();
		ApplyForce(UltimateForce);

		velocity += acceleration * Time.deltaTime;
		vehiclePosition += velocity * Time.deltaTime;
		direction = velocity.normalized;
		acceleration = Vector3.zero;
		transform.position = vehiclePosition;

		transform.LookAt(vehiclePosition + velocity.normalized);

		if (Input.GetKeyDown(KeyCode.D))
		{

			if (!debuglines)
			{
				debuglines = true;
				Debug.Log("debug: true");
			}
			else
			{
				debuglines = false;
				Debug.Log("debug: false");
			}
			
		}
	}

	public abstract void OnRenderObject();
	public abstract Vector3 CalcSteeringForces();

	// ApplyForce
	// Receive an incoming force, divide by mass, and apply to the cumulative accel vector
	public void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	// ApplyForce
	// Receive an incoming force, divide by mass, and apply to the cumulative accel vector
	public void ApplyGravityForce(Vector3 force)
	{
		acceleration += force;
	}

	// SEEK METHOD
	// All Vehicles have the knowledge of how to seek
	// They just may not be calling it all the time
	/// <summary>
	/// Seek
	/// </summary>
	/// <param name="targetPosition">Vector3 position of desired target</param>
	/// <returns>Steering force calculated to seek the desired target</returns>
	public Vector3 Seek(Vector3 targetPosition)
	{
		// Step 1: Find DV (desired velocity)
		// TargetPos - CurrentPos
		Vector3 desiredVelocity = (targetPosition + (velocity * 3)) - vehiclePosition;

		// Step 2: Scale vel to max speed
		// desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		// Step 3:  Calculate seeking steering force
		Vector3 seekingForce = desiredVelocity - velocity;

		// Step 4: Return force
		return seekingForce;
	}

	/// <summary>
	/// Overloaded Seek
	/// </summary>
	/// <param name="target">GameObject of the target</param>
	/// <returns>Steering force calculated to seek the desired target</returns>
	public Vector3 Seek(GameObject target)
	{
		return Seek(target.transform.position);
	}

	/// <summary>
	/// Flee
	/// </summary>
	/// <param name="targetPosition">Position of desired </param>
	/// <returns></returns>
	public Vector3 Flee(Vector3 targetPosition) {

		Vector3 desiredVelocity = vehiclePosition - (targetPosition + (velocity * 5));

		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		Vector3 fleeingForce = desiredVelocity - velocity;

		return fleeingForce;
	}

	/// <summary>
	/// Overloaded Flee
	/// </summary>
	/// <param name="target"></param>
	/// <returns>Fleeing force calculated to Flee the desired target</returns>
	public Vector3 Flee(GameObject target) {
		return Flee(target.transform.position);
	}

	/// <summary>
	/// Friction 
	/// </summary>
	/// <param name="coeff"></param>
	public void ApplyFriction(float coeff)
	{
		Vector3 friction = velocity * -1;
		friction.Normalize();
		friction = friction * coeff;
		acceleration += friction;
	}

	/// <summary>
	/// Keeps the sprite in bounds
	/// </summary>
	Vector3 Boundaries()
	{
		//Camera cam = Camera.main;
		//float height = cam.orthographicSize * 2f;
		//float width = height * cam.aspect;

		//GameObject floor = GameObject.Find("Plane");
		Vector3 steer = Vector3.zero;
		if (vehiclePosition.x > 20 || vehiclePosition.x < -20 || vehiclePosition.z > 20 || vehiclePosition.z < -20)
		{
			inBounds = false;
			steer = Seek(Vector3.zero);
		}
		else
		{
			inBounds = true;
		}

		return steer * 2f;

	}


	/// <summary>
	/// Keep bodies from intersecting with each other
	/// </summary>
	/// <returns>Steering force</returns>
	public Vector3 Separation(GameObject gameObject) {
		Vector3 steeringForce = Vector3.zero;

		//Find neighbors
		//Calculate a steering vector away from each neighbor
		//use weights that are inversely proportional (1/distance)
		//Sum up all the forces
		if (gameObject.transform.position != vehiclePosition)
		{
			if (Vector3.Distance(gameObject.transform.position, vehiclePosition) < 3f)
			{
				steeringForce = Flee(gameObject.transform.position);
			}
		}
	
		return steeringForce * 3f;

	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="obstacle"></param>
	/// <returns></returns>
	public Vector3 ObstacleAvoidance(Obstacle obstacle)
	{

		Vector3 VectToCenter = obstacle.transform.position - transform.position;
		float dotRight = Vector3.Dot(VectToCenter, transform.right);
		float dotForward = Vector3.Dot(VectToCenter, transform.forward);
		float radiiSum = obstacle.radius + radius;

		//Ignore anything behind char - exit and return zero
		if (dotForward < 0)
		{
			return Vector3.zero;
		}

		//Ignore anything oustide of "safe radius" - exit then return zero
		if (VectToCenter.magnitude > safeDistance)
		{
			return Vector3.zero;
		}

		//Test for Non-Intersection
		if (Mathf.Abs(dotRight) > radiiSum)
		{
			return Vector3.zero;
		}

		//Steer away from obstacle
		Vector3 desiredVelocity;
		if (dotRight < 0)
		{
			desiredVelocity = transform.right * maxSpeed;
		}
		else
		{
			desiredVelocity = -transform.right * maxSpeed;
		}
		Debug.DrawLine(transform.position, obstacle.transform.position, Color.red);
		Debug.DrawLine(transform.position, transform.position + (desiredVelocity * maxSpeed), Color.yellow);

		return (desiredVelocity - velocity);
	}

	public Vector3 Wander() {
		//Create circle in front
		Vector3 circleSpot = transform.position + (transform.forward * 4);
		//Pick random angle
		randAngle += Random.Range(-20f, 20f);

		//Find coordinates to that position
		float x = circleSpot.x + Mathf.Cos(randAngle * Mathf.Deg2Rad) * wanderRadius;
		float z = circleSpot.z + Mathf.Cos(randAngle * Mathf.Deg2Rad) * wanderRadius ;
		Vector3 wanderPos = new Vector3(x,transform.position.y,z);

		Debug.DrawLine(transform.position,wanderPos,Color.blue);
		return Seek(wanderPos);
	}
}
