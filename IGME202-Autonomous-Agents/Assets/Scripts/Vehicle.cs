	using System.Collections;
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
		Vector3 desiredVelocity = (targetPosition + velocity) - vehiclePosition;

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

		Vector3 desiredVelocity = vehiclePosition - (targetPosition + velocity);

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

		return steer * 3f;

	}


	/// <summary>
	/// Keep bodies from intersecting with each other
	/// </summary>
	/// <returns>Steering force</returns>
	public Vector3 Separation(GameObject gameObject) {

		//Find neighbors
		//Calculate a steering vector away from each neighbor
		//use weights that are inversely proportional (1/distance)
		//Sum up all the forces
		float distance = Vector3.Distance(gameObject.transform.position, vehiclePosition);

		
		if ( distance < 8f)
		{
			Debug.Log("separate");
			Debug.DrawLine(vehiclePosition, vehiclePosition + Flee(gameObject), Color.red);

			Vector3 desiredVelocity = vehiclePosition - (gameObject.transform.position);

			desiredVelocity.Normalize();
			desiredVelocity = desiredVelocity * maxSpeed; ;

			Vector3 fleeingForce = desiredVelocity - velocity;
			fleeingForce = fleeingForce * (1/distance);

			return fleeingForce;
			//return Flee(gameObject);
		}
			
		
	
		return Vector3.zero;

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="obstacle"></param>
	/// <returns></returns>
	//public Vector3 ObstacleAvoidance(Obstacle obstacle)
	//{

	//	Vector3 VectToCenter = obstacle.transform.position - transform.position;
	//	float dotRight = Vector3.Dot(VectToCenter, transform.right);
	//	float dotForward = Vector3.Dot(VectToCenter, transform.forward);
	//	float radiiSum = obstacle.radius + radius;


	//	//Ignore anything behind char - exit and return zero
	//	if (dotForward < 0)
	//	{
	//		return Vector3.zero;
	//	}

	//	//Ignore anything oustide of "safe radius" - exit then return zero
	//	if (VectToCenter.magnitude > safeDistance)
	//	{
	//		return Vector3.zero;
	//	}

	//	//Test for Non-Intersection
	//	if (Mathf.Abs(dotRight) > radiiSum)
	//	{
	//		return Vector3.zero;
	//	}

	//	//Steer away from obstacle
	//	Vector3 desiredVelocity;
	//	if (dotRight < 0)
	//	{
	//		Debug.Log("avoid right");
	//		desiredVelocity = transform.right * maxSpeed;
	//	}
	//	else
	//	{
	//		Debug.Log("avoid left");
	//		desiredVelocity = -transform.right * maxSpeed;
	//	}
	//	Debug.DrawLine(transform.position, obstacle.transform.position, Color.magenta);
	//	Debug.DrawLine(transform.position, transform.position + (desiredVelocity * maxSpeed), Color.yellow);

	//	return (desiredVelocity - velocity) * 3f;
	//}

	protected Vector3 ObstacleAvoidance(GameObject obstacle)
	{
		// Info needed for obstacle avoidance
		Vector3 vecToCenter = obstacle.transform.position - vehiclePosition;
		float dotForward = Vector3.Dot(vecToCenter, transform.forward);
		float dotRight = Vector3.Dot(vecToCenter, transform.right);
		float radiiSum = obstacle.GetComponent<Obstacle>().radius + radius;

		// Step 1: Are there objects in front of me?  
		// If obstacle is behind, ignore, no need to steer - exit method
		// Compare dot forward < 0
		if (dotForward < 0)
		{
			return Vector3.zero;
		}

		// Step 2: Are the obstacles close enough to me?  
		// Do they fit within my "safe" distance
		// If the distance > safe, exit method
		if (vecToCenter.magnitude > safeDistance)
		{
			return Vector3.zero;
		}

		// Step 3:  Check radii sum against distance on one axis
		// Check dot right, 
		// If dot right is > radii sum, exit method
		if (radiiSum < Mathf.Abs(dotRight))
		{
			return Vector3.zero;
		}

		// NOW WE HAVE TO STEER!  
		// The only way to get to this code is if the obstacle is in my path
		// Determine if obstacle is to my left or right
		// Desired velocity in opposite direction * max speed
		Vector3 desiredVelocity;

		if (dotRight < 0)        // Left
		{
			desiredVelocity = transform.right * maxSpeed;
		}
		else                    // Right
		{
			desiredVelocity = -transform.right * maxSpeed;
		}

		// Debug line to obstacle
		// Helpful to see which obstacle(s) a vehicle is attempting to maneuver around
		Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);

		// Return steering force
		Vector3 steeringForce = desiredVelocity - velocity;
		return steeringForce;
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
