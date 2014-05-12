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
	private GameManager gameManager;
	private Vector3 steeringForce, moveDirection;
	
	private float gravity = 200.0f;
	
	//for the state machine
	string[] inputs = {"Wandering", "Waiting", "Running", "Chasing"};
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
	public int wanderRadius = 50; //radius of projected circle for wander
	public int wanderDistance = 50; //distance from character to projected circle
	
	//to determine what has the highest priority for actions
	bool seesLibrarian = false;
	bool seesVillager = false;
	
	public Werewolf ()
	{
		//Debug.Log ("Makin a librarian, bitch");
		currentState = 0;
		nInputs = inputs.Length;
		centerPoint = new Point (0.0f, 0.0f);
		Start ();
	}
	
	//different from constructor, might restart some Villagers. Otherwise call start along with new()
	public void Start()
	{
		//Debug.Log ("WHITE PEOPLE");
		gameManager = GameManager.Instance;
		characterController = gameObject.GetComponent<CharacterController> ();
		steering = gameObject.GetComponent<Steering> ();
		//		wanderObject = new Wander ();
	}
	
	//properties
	public int NInputs { get { return nInputs; } }
	
	public void setGameManager (GameObject g) {	gameManager = g.GetComponent<GameManager> (); }
	
	//UPDATE
	public void Update()
	{
		if (!seesLibrarian) {
			if(!Physics.Linecast(transform.position, gameManager.Librarian.transform.position)) {
				target = gameManager.Librarian;
				seesLibrarian = true;
			}
				}
		if (!seesLibrarian && !seesVillager) {
			float d = float.MaxValue;
						foreach (GameObject v in gameManager.villagers) {
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
		steeringForce += CalcSteeringForce(); //--> add this later
		
		//Logic to determine which state we should be in and what to send to MakeTrans()
		Debug.Log ("Updating librarian");
		
		
		ClampSteering ();
		
		moveDirection = transform.forward;
		moveDirection.y = 0;
		moveDirection *= currentSpeed;
		steeringForce.y = 0;
		moveDirection += steeringForce * Time.deltaTime;
		
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
		moveDirection.y -= gravity;
		
		// the CharacterController moves us subject to physical constraints
		characterController.Move (moveDirection * Time.deltaTime);
		
	}
	
	#region movement Methods
	private Vector3 StayInBounds ( float radius, Vector3 center)
	{
		
		steeringForce = Vector3.zero;
		
		if(transform.position.x > 750)
		{
			steeringForce += steering.Flee(new Vector3(800,0,transform.position.z));
		}
		
		if(transform.position.x < 200)
		{
			steeringForce += steering.Flee(new Vector3(150,0,transform.position.z));
		}
		
		if(transform.position.z > 715)
		{
			steeringForce += steering.Flee(new Vector3(transform.position.x,0,765));
		}
		
		if(transform.position.z < 205)
		{
			steeringForce += steering.Flee(new Vector3(transform.position.x,0,155));
		}
		
		if(transform.position.x > 750 || transform.position.x < 200 || 
		   transform.position.z > 715 || transform.position.z < 205)
		{
			Debug.Log("SteeringForce: " + steeringForce.ToString() + "gameManager: " + gameManager + "\n");
			steeringForce += steering.Seek(gameManager.gameObject);
		}
		
		return steeringForce;
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
		Vector3 tempSteering = Vector3.zero;
		
		switch (currentAction = chooseAction ()) 
		{
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
			if(Physics.Linecast(transform.position, target.transform.position)) {
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
			currentSpeed = steering.maxSpeed/1.5f;
			tempSteering += wander();
			break;
		}
		
		tempSteering += StayInBounds (100.0f, Vector3.zero);
		
		return tempSteering;
	}
	
	public Vector3 wander() 
	{
		wanderRandom += Random.Range(-wanderRate, wanderRate); //move the point on the circle to a random point within the rate
		float wanderAngle = wanderRandom * (Mathf.PI * 2); //get angle of point on circle
		return new Vector3(this.transform.position.x + (this.transform.forward.x * wanderDistance) +
		                   (wanderRandom * Mathf.Cos(wanderAngle)), 150,
		                   this.transform.position.y + (this.transform.forward.y * wanderDistance) +
		                   (wanderRandom * Mathf.Sin(wanderAngle))); //return vector of current position + forward vector * projected circle distance +
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
			return "Chasing";									//has someone asked you to help them find a book?								//are you checking something (waiting at the desk)
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
