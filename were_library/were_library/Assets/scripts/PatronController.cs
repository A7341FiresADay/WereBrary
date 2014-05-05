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

		target_random_shelf();
	}

	Vector3 random_shelf_position(){
		TargetShelf = GameObject.FindGameObjectsWithTag ("Bookshelf")[(int)(Random.value * GameObject.FindGameObjectsWithTag ("Bookshelf").Length)].GetComponent<bookshelf>();
		return TargetShelf.gameObject.transform.position;
	}

	void target_random_shelf(){
		GetComponent<NavMeshAgent>().SetDestination( random_shelf_position() );
	}

	void target_obj(Vector3 target){
		
		GetComponent<NavMeshAgent>().SetDestination(target);
	}

	bool target_shelf_known(){
		foreach(bookshelf shelf in KnownSelves){
			if(shelf.book.name == BookToFind){
				return true;
			}
		}

		return false;
	}
	Vector3 target_book_shelf(){
		foreach(bookshelf shelf in KnownSelves){
			if(shelf.book.name == BookToFind){
				TargetShelf = shelf;
				return shelf.transform.position;
			}
		}
		return random_shelf_position();
	}

	// Update is called once per frame
	void Update () {

		PatronState = PatronStates.wandering;

		if(target_shelf_known () ){ //if you know where to go, go there
			target_obj ( target_book_shelf() );
		} else {
			if(KnownSelves.Contains(TargetShelf)){ //if you don't have a plan and you don't know where to go, go ANYWHERE
				target_random_shelf();
			}
		}

		Debug.Log(TargetShelf);
		if(Vector3.Distance(TargetShelf.gameObject.transform.position, 
		                    transform.position) < 4){ // If you're at your target
			PatronState = PatronStates.searchingSelf; // search the book

			if( target_shelf_known() ){ //or take the book, if you're sure this is the one you want
				PatronState = PatronStates.takingBook;
			}
		}

		if(CarriedBook){
			
			if(CarriedBook.GetComponent<Book>().name == BookToFind){ //if you have the book, skedaddle
				PatronState = PatronStates.leavingWithBook;
			}

		}






		switch (PatronState) 
		{
			case(PatronStates.wandering):		wander(); 		 break; //<- do
			case(PatronStates.searchingSelf):	searchSelf(); 	 break; //<- do
			case(PatronStates.takingBook):		takeBook(); 	 break; //<- do
			case(PatronStates.askingPatron):	askPatron(); 	 break; //<- NO
			case(PatronStates.askingLibrarian):	askLibrarian();  break; //<- NO
			case(PatronStates.fleeingWerewolf):	fleeWerewolf();	 break; //<- NO
			case(PatronStates.leavingWithBook): leaveWithBook(); break; //<- do
			default:							wander(); 		 break; //<- do
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
		target_obj(GameObject.Find("Enterance").transform.position);
		transform.position = GetComponent<NavMeshAgent>().nextPosition;
	}
}

