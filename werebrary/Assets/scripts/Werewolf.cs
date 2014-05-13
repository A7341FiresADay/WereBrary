//------------------------------------------------------------------------------
//Class Librarian
//Behaviors:
//-Wanders by desk, likes to help people find books (hopefully)
//-sound alerts her to werewolves/people in danger, finds path to them.
//-forces werewolves away from villagers, if close enough will turn werewolf back to human
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Steering))]

public class Werewolf: MonoBehaviour
{
	//need some private instance variables for movement
	private CharacterController characterController;
	private Steering steering;
	public GameManager gameManager;
	private Vector3 steeringForce, moveDirection;
	
	private Gibberish gibContext;
	
	private float gravity = 200.0f;
	
	//for the state machine
	string[] inputs = {"Wandering", "Waiting", "Helping", "Chasing"};
	int nInputs;
	int currentState;
	string currentAction;
	float currentSpeed = 0.0f;
	
	//random crap I need
	GameObject target;
	Vector3 lastPos;
	//	Wander wanderObject;
	Point centerPoint;
	
	float wanderRandom = 0; //current position on projected circle
	public float wanderRate = 0.1f; //rate at which point on circle moves
	public int wanderRadius = 30; //radius of projected circle for wander
	public int wanderDistance = 10; //distance from character to projected circle

	//to determine what has the highest priority for actions
	bool seesLibrarian = false;
	bool seesVillager = false;
	
	float maxTetherX, maxTetherZ, minTetherX, minTetherZ;
	GameObject currentPlane;
	public Werewolf ()
	{
		//Debug.Log ("Makin a librarian");
		currentState = 0;
		nInputs = inputs.Length;
		centerPoint = new Point (0.0f, 0.0f);
		//Start ();
	}
	
	//different from constructor, might restart some Villagers. Otherwise call start along with new()
	public void Start()
	{
		//Debug.Log ("WHITE PEOPLE");
		//		gameManager = GameManager.Instance;
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		currentPlane = GameObject.Find ("Plane");
		
		characterController = gameObject.GetComponent<CharacterController> ();
		steering = gameObject.GetComponent<Steering> ();
		
		maxTetherX = 32.0f;
		minTetherX = -32.0f;
		maxTetherZ = 37.2f;
		minTetherZ = -26.8f;
		//currentPlane.collider.bounds.max.x
		//plane's position is (0, -0.5, 5.2)
		/*Debug.Log ("Plane: " + currentPlane.transform.position.ToString ());
		Debug.Log ("\n Max X: " + currentPlane.collider.bounds.max.x + "\nMax Z: " + currentPlane.collider.bounds.max.z 
		           + "\nMin X: " + currentPlane.collider.bounds.min.x + "\nMin Z: " + currentPlane.collider.bounds.min.z);
		Max X: 32
			Max Z: 37.20094
				Min X: -32
				Min Z: -26.79906*/
		//setGameManager();
		gibContext = new Gibberish ("Assets/scripts/LibrarianGibberish", Random.Range (2, 5));
		//Debug.Log ("Gibberish: " + gibContext.FinalReturnGibberish);
	}
	
	//properties
	public int NInputs { get { return nInputs; } }
	
	public void setGameManager (GameObject g) {	gameManager = g.GetComponent<GameManager> (); }
	
	//UPDATE
	public void FixedUpdate()
	{
		if (!seesLibrarian) {
			if(!Physics.Linecast(transform.position, gameManager.Librarian.transform.position)) {
				target = gameManager.Librarian;
				seesLibrarian = true;
			}
		}
		if (!seesLibrarian && !seesVillager) {
			float d = float.MaxValue;
			foreach (GameObject v in GameObject.FindGameObjectsWithTag("Patron")) {
				if(!Physics.Linecast(transform.position, v.transform.position)) {
					float newD = Vector3.Distance(transform.position,v.transform.position);
					if(newD < d) {
						d = newD;
						seesVillager = true;
						target = v;
					}
				}
			}
		}
		steeringForce = Vector3.zero;
		
		steeringForce += CalcSteeringForce().normalized;
		//Debug.Log("Vector from steeringForce(wander only): " + steeringForce.ToString());
		
		//if (gameManager != null)
		//	steeringForce += steering.Seek(new Vector3(0, 0, 0));
		//Logic to determine which state we should be in and what to send to MakeTrans()
		//Debug.Log ("Updating librarian");
		
		steeringForce += StayInBounds(100.0f, Vector3.zero).normalized;
		//Debug.Log("After stayinbounds called: " + steeringForce.ToString());
		
		//steeringForce += steering.Seek (Vector3.zero);
		//Debug.Log("Tethering! Vector: " + steeringForce.ToString());
		//Debug.DrawLine(this.transform.position, this.transform.position + (steeringForce * 5), Color.blue);
		
		ClampSteering ();
		
		ClampSteering ();
		
		moveDirection = transform.forward;
		moveDirection.y = 0;
		moveDirection *= currentSpeed;
		steeringForce.y = 0;
		moveDirection += steeringForce;
		
		//add the stayInBounds here when we know what bounds we are staying in
		
		//currentSpeed = moveDirection.magnitude;
		/*if (currentSpeed != moveDirection.magnitude) {
			moveDirection = moveDirection.normalized * currentSpeed;
		}*/
		//orient transform
		if (moveDirection != Vector3.zero)
		{	
			transform.forward = moveDirection;
		}
		
		// Apply gravity
		//moveDirection.y -= gravity;
		
		// the CharacterController moves us subject to physical constraints
		Debug.DrawLine (transform.position, transform.position + moveDirection);
		characterController.Move (Vector3.ClampMagnitude(moveDirection, 0.1f));
		
	}
	
	#region movement Methods
	private Vector3 StayInBounds ( float radius, Vector3 center)
	{
		Vector3 tempSteeringForce = Vector3.zero;
		bool nearEdge = false;
		
		if(transform.position.x > maxTetherX-10)
		{
			tempSteeringForce += steering.Flee(new Vector3(maxTetherX, 0,transform.position.z));
			//Debug.Log("Position.x (" + transform.position.x + ") > maxTetherX (" + maxTetherX);
			Debug.DrawLine(this.transform.position, this.transform.position + (tempSteeringForce * 5), Color.red);
			nearEdge = true;
		}
		
		else if(transform.position.x < minTetherX+10)
		{
			tempSteeringForce += steering.Flee(new Vector3(minTetherX, 0,transform.position.z));
			//Debug.Log("Position.x (" + transform.position.x + ") < minTetherX (" + minTetherX);
			Debug.DrawLine(this.transform.position, this.transform.position + tempSteeringForce, Color.red);
			nearEdge = true;
		}
		
		else if(transform.position.z > maxTetherZ-10)
		{
			tempSteeringForce += steering.Flee(new Vector3(transform.position.x,0,maxTetherZ));
			//Debug.Log("Position.z (" + transform.position.x + ") > maxTetherZ (" + maxTetherZ);
			Debug.DrawLine(this.transform.position, this.transform.position + tempSteeringForce, Color.red);
			nearEdge = true;
		}
		
		else if(transform.position.z < minTetherZ+10)
		{
			tempSteeringForce += steering.Flee(new Vector3(transform.position.x,0,minTetherZ));
			//Debug.Log("Position.z (" + transform.position.x + ") < maxTetherZ (" + maxTetherZ);
			Debug.DrawLine(this.transform.position, this.transform.position + tempSteeringForce, Color.red);
			nearEdge = true;
		}
		else {}
		
		if(nearEdge)
		{
			//Debug.Log("SteeringForce: " + steeringForce.ToString() + "gameManager: " + gameManager.ToString() + "\n");
			tempSteeringForce += steering.Seek(Vector3.zero);
			Debug.DrawLine(this.transform.position, this.transform.position + tempSteeringForce, Color.yellow);
			//Debug.DrawLine(this.transform.position, gameManager.transform.position);
			//Debug.Log("Nearing Edge blanket, seeking: " + gameManager.gameObject.transform.position.ToString());
		}
		
		tempSteeringForce.Normalize();
		//Debug.Log("SteeringForce from stay in bounds: " + tempSteeringForce.magnitude.ToString());
		//Debug.DrawLine(this.transform.position, this.transform.position + (tempSteeringForce * 5), Color.red);
		return tempSteeringForce;
	}
	
	private void ClampSteering ()
	{
		if (steeringForce.magnitude > steering.maxForce) {
			steeringForce.Normalize ();
			steeringForce *= steering.maxForce;
		}
	}
	
	private Vector3 CalcSteeringForce()
	{
		//Debug.DrawLine(this.transform.position, this.transform.position + (steering.Seek(Vector3.zero)), Color.blue);
		//return steering.Seek(Vector3.zero);
		Vector3 tempSteering = Vector3.zero;
		Vector3 boundsSteering = StayInBounds (100.0f, Vector3.zero);
		//if (boundsSteering == Vector3.zero) {
		switch (currentAction = chooseAction ()) {
		case "Waiting":
			currentSpeed = 0;
			break;
		case "Running":
			if(Physics.Linecast(transform.position, target.transform.position)) {
				seesLibrarian = false;
				target = null;
				break;
			}
			currentSpeed = steering.maxSpeed;
			tempSteering += steering.Flee(target);  //target should be the book in question, could randomly place them? Librarian is leader for the villager
			break;
		case "Chasing":
			if(target == null || Physics.Linecast(transform.position, target.transform.position)) {
				if(Vector3.Distance(transform.position, lastPos) < 1) {
					target = null;
					seesVillager = false;
					break;
				}
				tempSteering += steering.Seek(lastPos);
			} else {
				lastPos = target.transform.position;
				currentSpeed = steering.maxSpeed;
				tempSteering += steering.Seek(target);  //target HERE should be the villager in question
			}
			break;
		default:
			currentSpeed = steering.maxSpeed / 1.5f;
			tempSteering += wander ();
			//Debug.Log("Wandering! Vector: " +  tempSteering.ToString());
			Debug.DrawLine(this.transform.position, this.transform.position + (tempSteering * 5), Color.green);
			break;
		}
		//Debug.Log("TempSteering Magnitude while wandering: " + tempSteering.magnitude);
		
		//		} 
		/*else {
			//tempSteering = (tempSteering)/10;
			//tempSteering.magnitude = tempSteering.magnitude/10;
			tempSteering += boundsSteering;
			Debug.Log("Tethering! Vector: " + boundsSteering.ToString());
			Debug.DrawLine(this.transform.position, this.transform.position + (tempSteering * 5), Color.blue);
		}*/
		//Debug.Log("SteeringForce from calcsteeringforce before Normalization: " + steeringForce.magnitude.ToString());
		tempSteering.Normalize();
		//Debug.DrawLine(this.transform.position, this.transform.position + (tempSteering /* 5*/), Color.red);
		//Debug.Log("SteeringForce from calcsteeringforce Normalized: " + steeringForce.magnitude.ToString());
		return tempSteering;
	}
	
	public Vector3 wander() 
	{
		wanderRandom += Random.Range(-wanderRate, wanderRate); //move the point on the circle to a random point within the rate
		float wanderAngle = wanderRandom * (Mathf.PI * 2); //get angle of point on circle
		//Debug.DrawLine(
		return new Vector3((this.transform.forward.x * wanderDistance) +
		                   (wanderRadius * Mathf.Cos(wanderAngle)), 0,
		                   (this.transform.forward.z * wanderDistance) +
		                   (wanderRadius * Mathf.Sin(wanderAngle)));
		
		/*wanderRandom += Random.Range(-wanderRate, wanderRate); //move the point on the circle to a random point within the rate
		float wanderAngle = wanderRandom * (Mathf.PI * 2); //get angle of point on circle
		//Debug.DrawLine(
		return new Vector3(this.transform.position.x + (this.transform.forward.x * wanderDistance) +
		                   (wanderRandom * Mathf.Cos(wanderAngle)), 150,
		                   this.transform.position.z + (this.transform.forward.z * wanderDistance) +
		                   (wanderRandom * Mathf.Sin(wanderAngle))); //return vector of current position + forward vector * projected circle distance +*/
		//position of current point on project circle
	}
	
	#endregion
	
	#region State Machine Methods
	
	//returns the state necessary based on what is going on
	private string chooseAction()
	{
		if (seesLibrarian)										//is there a wolf attacking nearby?
			return "Running";
		if (seesVillager)
			return "Chasing";									//has someone asked you to help them find a book?									//are you checking something (waiting at the desk)
		return "Wandering";										//otherwise wander aimlessly
	}
	
	// Handling the FSM, mostly taken from the StMachDemo
	public void MakeTrans(int input)
	{
		int nextState = -1;			// Assume the worst (an error)
		
		switch (currentState)				// Get the next state with logic
		{
		case 0:						// Cascading if/elses
			if (input == 0)
				nextState = 1;
			else if (input == 2)
				nextState = 2;
			else
				nextState = currentState;
			break;
		case 1:
			switch (input)			// Switch within a switch
			{
			case 1:
				nextState = 0;
				break;
			case 2:
				nextState = 2;
				break;
			default:
				nextState = currentState;
				break;
			}
			break;
		case 2:						// The old nested ternary trick
			nextState = (input == 3) ? 0 : (input == 4) ? 3 : currentState;
			break;
		case 3:
			if (input == 5)			// Simple if/else actually okay for this one
				nextState = 0;
			else
				nextState = currentState;
			break;
		}
		
		if (nextState >= 0)			// Make sure the next state is legal
			currentState = nextState;		// and update currState if it is
		else
		{
			Debug.Log ("Bad state in FSMHardCode");
		}
		
		Debug.Log ("Input"+input + ": " + inputs[input]);	// Display input
	}
	
	// Driver method for calling action routines copied from Ghost class
	public void CallAction ()
	{
		switch (currentState)
		{
		case 0:
			s0Act ();
			break;
		case 1:
			s1Act ();
			break;
		case 2:
			s2Act ();
			break;
		case 3:
			s3Act ();
			break;
		default:
			Debug.Log ("Oops!  Bad state!");
			break;
		}
		return;
	}
	
	void s0Act(){}
	void s1Act(){}
	void s2Act(){}
	void s3Act(){}
	
	#endregion
}