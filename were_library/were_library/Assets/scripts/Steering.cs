using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class Steering : MonoBehaviour
{
	
	//movement variables - exposed in inspector panel
	public float maxSpeed = 50.0f;
	//maximum speed of vehicle
	public float maxForce = 15.0f;
	// maximimum force allowed
	
	//movement variables - updated by this component
	private float speed = 0.0f;
	//current speed of vehicle
	private Vector3 velocity;
	//change in position per second
	
	public Vector3 Velocity {
		get { return velocity; }
		set { velocity = value;}
	}
		
	public float Speed {
		get { return speed; }
		set { speed = Mathf.Clamp (value, 0, maxSpeed); }
	}
	
	public void Start ()
	{
		Velocity = Vector3.zero;
	}
	
	public Vector3 Pursuit (Vector3 fwd)
	{
		// find dv, the desired velocity
		// sets the vector to the future position
		fwd = fwd * 1.1f;
		Vector3 dv = fwd - transform.position;
		dv.y = 0; //only steer in the x/z plane
		dv = dv.normalized * maxSpeed;//scale by maxSpeed
		dv -= transform.forward * speed;//subtract velocity to get vector in that direction
		return dv;  
	}

	
	public Vector3 Seek (Vector3 pos)
	{
		/*
		float maxSpeed
		float maxForce
		float speed
		Vector3 velocity;
	*/
		return ((pos - transform.position).normalized * maxSpeed) - velocity;

	}
	
	public Vector3 Arrival (Vector3 pos)
	{
		// find dv, the desired velocity
		Vector3 dv = pos - transform.position;
		//calculate distance between pos and position of seeker
		float dist = Vector3.Distance(pos,transform.position);
		dv.y = 0; //only steer in the x/z plane
		//if close slow down, else full speed seek
		

		dv = (dv.normalized * (maxSpeed));//scale by maxSpeed
		if (dist < 5) {
						dv *= 1 / dist;
				}
		
		
		dv -= transform.forward * speed;//subtract velocity to get vector in that direction
		return dv;
	}
	
	// same as seek pos above, but parameter is game object
	public Vector3 Seek (GameObject gO)
	{
		return Seek(gO.transform.position);
	}
	
	public Vector3 Evasion (Vector3 fwd)
	{
		fwd = fwd * 2; // sets a future vector for level bobbing.
		Vector3 dv = transform.position - fwd;
		dv.y = 0; //only steer in the x/z plane
		dv = dv.normalized * maxSpeed;//scale by maxSpeed
		dv -= transform.forward * speed;//subtract velocity to get vector in that direction
		return dv;  
		
	}

	public Vector3 Flee (Vector3 pos)
	{

		return -Seek(pos);
	}
	
	public Vector3 Flee (GameObject go)
	{
		return Flee (go.transform.position);
	}

	public Vector3 AlignTo (Vector3 direction)
	{
		// useful for aligning with flock direction
		Vector3 dv = direction.normalized;
		return dv * maxSpeed - transform.forward * speed;
		
	}

	//Assumtions:
	// we can access radius of obstacle
	// we have CharacterController component
	public Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{
		Vector3 dv = Vector3.zero;
		//compute a vector from charactor to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;
		float dist = vecToCenter.magnitude;
		
		//return zero vector if too far to worry about
		if (dist > safeDistance + obst.GetComponent<Dimensions> ().Radius + GetComponent<Dimensions> ().Radius)
			return dv;
		
		//return zero vector if behind us
		if (Vector3.Dot (vecToCenter, transform.forward) < 0)
			return dv;
		
		//return zero vector if we can pass safely
		float rightDotVTC = Vector3.Dot (vecToCenter, transform.right);
		if (Mathf.Abs (rightDotVTC) > obst.GetComponent<Dimensions> ().Radius + GetComponent<Dimensions> ().Radius)
			return dv;
		
		//obstacle on right so we steer to left
		if (rightDotVTC > 0)
			dv = transform.right * -maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv = transform.right * maxSpeed * safeDistance / dist;
		
		//stay in x/z plane
		dv.y = 0;
		
		//compute the force
		dv -= transform.forward * speed;
		return dv;
	}
}