using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatronController : MonoBehaviour {

	public string BookToFind;
	public GameObject CarriedBook;
	
	public float SearchTime;
	public float TakeTime;
	public float ConversationTime;
	public void ResetTimers(){
		SearchTime = 1000;
		TakeTime = 1000;
		ConversationTime = 1000;
		
	}
	
	public Vector3 WanderTarget;
	public bookshelf TargetShelf;
	public GameObject ConversationTarget;


	PatronStates patronState;
	public PatronStates PatronState{
		get{
			return patronState;
		}
		set{
			ResetTimers();
			patronState = value;
		}

	}
	public enum PatronStates{
		wandering,
		searchingSelf,
		takingBook,
		askingPatron,
		askingLibrarian,
		fleeingWerewolf,
		leavingWithBook
	}





	// Use this for initialization
	void Start () {
		//BookToFind = GameObject.Find ("Shelves").GetComponent<BookselfStocker> ().AssignSeekerBook ();

		PatronState = PatronStates.wandering;

		KnownSelves = new List<bookshelf> ();

		
		GetComponent<NavMeshAgent>().SetDestination(GameObject.FindGameObjectsWithTag ("Bookshelf")[(int)(Random.value * GameObject.FindGameObjectsWithTag ("Bookshelf").Length)].transform.position);
	}
	
	// Update is called once per frame
	void Update () {

		switch (PatronState) 
		{
			case(PatronStates.wandering):		wander(); 		 break;
			case(PatronStates.searchingSelf):	searchSelf(); 	 break;
			case(PatronStates.takingBook):		takeBook(); 	 break;
			case(PatronStates.askingPatron):	askPatron(); 	 break;
			case(PatronStates.askingLibrarian):	askLibrarian();  break;
			case(PatronStates.fleeingWerewolf):	fleeWerewolf();	 break;
			case(PatronStates.leavingWithBook): leaveWithBook(); break;
			default:							wander(); 		 break;
		}

	}





	public List<bookshelf> KnownSelves;


	void wander(){

		transform.position = GetComponent<NavMeshAgent>().nextPosition;

		//GameObject.FindGameObjectsWithTag ("Bookshelf");
	}


	void searchSelf(){
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 4) {
			SearchTime -= Time.deltaTime;
		}
		if (SearchTime <= 0) {
			KnownSelves.Add(TargetShelf);
		}

	}

	void takeBook(){
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 4) {
			TakeTime -= Time.deltaTime;
		}
		if (TakeTime <= 0) {
			CarriedBook = TargetShelf.book;
			TargetShelf.book = null;
		}
	}


	void askPatron(){
		/*if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < 4) {
			ConversationTime -= Time.deltaTime;
		}
		if (ConversationTime <= 0) {
			ResetTimers();
			//1- Take knowledge of desired book. 
			List<bookshelf> TargetKnownShelves = ConversationTarget.GetComponent<PatronController>().KnownSelves;
			if(!KnownSelves.Exists(shelf => shelf.getComponent<bookshelf>().books.contains(BookToFind))){ //If you don't know about your own book

				if(TargetKnownShelves.Exists(shelf => shelf.getComponent<bookshelf>().books.contains(BookToFind)) ){//And they do know about it (must change to check what is on known shelves)

				} else {
					KnownSelves.Add(TargetKnownShelves[Random.value * TargetKnownShelves.Count]);
				}
			} else {
				KnownSelves.Add(TargetKnownShelves[Random.value * TargetKnownShelves.Count]);
			}

			//2- Take knowledge of any other book
			
		}*/
}

	void askLibrarian(){

		if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < 4) {
			ConversationTime -= Time.deltaTime;
		}
		if (ConversationTime <= 0) {
			ResetTimers();
			//1- Take knowledge of desired book. 
			//2- Take knowledge of any other book

		}

	}

	void fleeWerewolf(){

	}

	void leaveWithBook(){

	}
}


