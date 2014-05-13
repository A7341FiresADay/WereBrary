using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshWerewolf : MonoBehaviour {
	
	public string BookToFind; // title of the desired book.
	public GameObject CarriedBook; //current book character is carrying.
	public GameObject chasedPatron;
	public float SearchTime; // Time until shelf is searched
	public float TakeTime; //Time unti book has been taken from shelf
	public void ResetTimers(){
		SearchTime = 1000;
		TakeTime = 1000;
		ConversationTime = 1000;
		
	}
	
	
	
	
	
	public List<bookshelf> KnownSelves; // Shelves this patron knows the content of.
	public bookshelf TargetShelf; //shelf to try next.
	
	public float TimeToNextTalk; // Time until next conversation is allowed
	public float ConversationTime; // Time until convo is finsihed
	public string ConvoText; //gibberish to say in next conversation
	public GameObject ConversationTarget; //Fellow to talk to.
	
	
	
	WerewolfStates werewolfState;
	public WerewolfStates WerewolfState{
		get{
			return werewolfState;
		}
		set{
			if( value != werewolfState){
				ResetTimers();
				werewolfState = value;
			}
		}
		
	}
	public enum WerewolfStates{
		wandering,
		chasingPatron,
		fleeingLibrarian
	}
	
	
	// Use this for initialization
	void Start () {
		//BookToFind = GameObject.Find ("Shelves").GetComponent<BookselfStocker> ().AssignSeekerBook ();
		TimeToNextTalk = Random.Range(500, 2000);
		WerewolfState = WerewolfStates.wandering;
		
		KnownSelves = new List<bookshelf> ();
		
		target_random_shelf();
		
		ConvoText = Gibberish.StringFromPool("Assets/scripts/PatronGibberish");
		
		grey_texture = Resources.Load<Texture2D>("grey");
		red_texture = Resources.Load<Texture2D>("red");
		blue_texture = Resources.Load<Texture2D>("blue");
		yellow_texture = Resources.Load<Texture2D>("yellow");
		darkyellow_texture = Resources.Load<Texture2D>("musturd_yellow");
		
		
	}
	
	Vector3 random_shelf_position(){
		TargetShelf = GameObject.FindGameObjectsWithTag ("Bookshelf")[(int)(Random.value * GameObject.FindGameObjectsWithTag ("Bookshelf").Length)].GetComponent<bookshelf>();
		return TargetShelf.gameObject.transform.position;
	}
	
	void target_random_shelf(){
		GetComponent<NavMeshAgent>().SetDestination( random_shelf_position() );
	}

	void target_patron() {
		float d = float.MaxValue;
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Patron")) {
			float newD = Vector3.Distance(transform.position,p.transform.position);
			if(newD < d) {
				d = newD;
				chasedPatron = p;
			}
				}
	}
	
	void target_obj(Vector3 target){
		
		GetComponent<NavMeshAgent>().SetDestination(target);
	}
	
	bool tsk = false;
	bool target_shelf_known(){
		if (tsk) 
		{
			return true;
		}
		foreach(bookshelf shelf in KnownSelves){
			if(shelf.book){
				if(shelf.book.GetComponent<Book>().BookName == BookToFind){
					tsk = true;
					return true;
				}
			}
		}
		
		return false;
	}
	Vector3 target_book_shelf(){
		foreach(bookshelf shelf in KnownSelves){
			if(shelf.book != null){
				if(shelf.book.GetComponent<Book>().BookName == BookToFind){
					TargetShelf = shelf;
					return shelf.transform.position;
				}
			}
		}
		return random_shelf_position();
	}
	
	GameObject nearest_patron() {
		Object[] patrons = FindObjectsOfType<PatronController>();
		GameObject np = ((PatronController)patrons[0]).gameObject;
		float dist = int.MaxValue;
		for (int i = 0; i < patrons.Length; i++) {
			GameObject patron = ((PatronController)patrons[i]).gameObject;
			float temp_dist = Vector3.Distance(transform.position, patron.transform.position);
			if(dist > temp_dist && patron != gameObject){
				dist = temp_dist;
				np = patron;
			}
		}
		return np;
		
	}
	bool gtg = false;
	bool known = false;
	// Update is called once per frame
	void Update () {
		
		/*
		if (gtg && !known)
		{
			known = true;
			Debug.Log("it is known");
			//TELL EACH PATRON WHERE THEIR BOOK IS SO THEY WILL GO FOR IT FIRST!
			for (int j = 0; j < GameObject.Find("Shelves").transform.childCount; j++) 
			{
				GameObject shelf = GameObject.Find ("Shelves").transform.GetChild (j).gameObject;
				bookshelf s = shelf.GetComponent<bookshelf> ();
				if (s.book.GetComponent<Book> ().BookName == BookToFind) 
				{
					KnownSelves.Add (s);
				}
			}
		}
		gtg = true;
		//FOR DEBUG ONLY
		*/
		
		
		TimeToNextTalk -= Random.Range(0, 100) * Time.deltaTime;
		var new_patron_state = WerewolfStates.wandering;

		GameObject fleedLibrarian = null;
		foreach(GameObject l in GameObject.FindGameObjectsWithTag("Librarian")) {
			if(!Physics.Linecast(transform.position, l.transform.position)) {
				fleedLibrarian = l;
			}
		}
		if (fleedLibrarian != null) {
			Vector3 flee_target = (transform.position - fleedLibrarian.transform.position) - transform.position;
			target_obj(flee_target);
				}
		else {
			target_obj(nearest_patron().transform.position);
				}
		/*if(target_shelf_known () ){ //if you know where to go, go there
			target_obj ( target_book_shelf() );
		} else {
			if(KnownSelves.Contains(TargetShelf)){ //if you don't have a plan and you don't know where to go, go ANYWHERE
				target_random_shelf();
			}
		}
		
		//Debug.Log(TargetShelf);
		if(Vector3.Distance(TargetShelf.gameObject.transform.position, transform.position) < 2){ // If you're at your target
			new_patron_state = PatronStates.searchingSelf; // search the book
			
			if( target_shelf_known() ){ //or take the book, if you're sure this is the one you want
				new_patron_state = PatronStates.takingBook;
			}
		}
		
		if(CarriedBook != null){
			Debug.Log("exists");
			Debug.Log(CarriedBook.GetComponent<Book>().BookName);
			if(CarriedBook.GetComponent<Book>().BookName == BookToFind){ //if you have the book, skedaddle
				new_patron_state = PatronStates.leavingWithBook;
			}
		}
		
		GameObject np = nearest_patron ();
		if (Vector3.Distance (np.transform.position, transform.position) < 4 && TimeToNextTalk <= 0 && np != gameObject && CarriedBook == null) {
			new_patron_state = PatronStates.askingPatron;
			ConversationTarget = np;
		}*/
		
		WerewolfState = new_patron_state;
		
		
		
		
		switch (WerewolfState) 
		{
		case(WerewolfStates.wandering):		wander(); 		 break; //<- do
		//case(PatronStates.searchingSelf):	searchSelf(); 	 break; //<- do
		//case(PatronStates.takingBook):		takeBook(); 	 break; //<- do
		//case(PatronStates.askingPatron):	askPatron(); 	 break; //<- NO
		//case(PatronStates.askingLibrarian):	askLibrarian();  break; //<- NO
		//case(PatronStates.fleeingWerewolf):	fleeWerewolf();	 break; //<- NO
		//case(PatronStates.leavingWithBook): leaveWithBook(); break; //<- do
		default:							wander(); 		 break; //<- do
		}
		
	}
	
	
	void wander(){
		GetComponent<NavMeshAgent> ().Resume ();
		transform.position = GetComponent<NavMeshAgent>().nextPosition;
		
		//GameObject.FindGameObjectsWithTag ("Bookshelf");
	}
	
	
	void searchSelf(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 2) {
			SearchTime -= Time.deltaTime * 200;
		}
		if (SearchTime <= 0) {
			KnownSelves.Add(TargetShelf);
		}
		
	}
	
	void takeBook(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 2) {
			TakeTime -= Time.deltaTime * 200;
			
			
		}
		if (TakeTime <= 0) {
			CarriedBook = TargetShelf.book;
			TargetShelf.book = null;
		}
	}
	
	
	void askPatron(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < 4) {
			ConversationTime -= Time.deltaTime * 200;
			// Debug.Log( Gibberish.StringFromPool("Assets/scripts/PatronGibberish") ); <-- gibberish
		}
		if (ConversationTime <= 0) {
			ResetTimers();
			
			ConvoText = Gibberish.StringFromPool("Assets/scripts/PatronGibberish");
			KnownSelves.AddRange( ConversationTarget.GetComponent<PatronController>().KnownSelves );
			TimeToNextTalk = Random.Range(500, 2000);
		}
	}
	
	void askLibrarian(){
		
		if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < 2) {
			ConversationTime -= Time.deltaTime;
		}
		if (ConversationTime <= 0) {
			ResetTimers();
			//1- Take knowledge of desired book. 
			//2- Take knowledge of any other book
			
		}
		
	}
	
	void fleeLibrarian(){

	}
	
	void leaveWithBook(){
		
		GetComponent<NavMeshAgent> ().Resume ();
		target_obj(GameObject.Find("Enterance").transform.position);
		transform.position = GetComponent<NavMeshAgent>().nextPosition;
	}
	
	public Texture grey_texture;
	public Texture red_texture;
	public Texture blue_texture;
	public Texture yellow_texture;
	public Texture darkyellow_texture;
	void OnGUI(){
		var p = Camera.main.WorldToScreenPoint(transform.position);
		var d = p.z/10;//Vector3.Distance(Camera.main.transform.position, transform.position) / 10;
		Rect ui_pos = new Rect(p.x - 250/d/2, Screen.height - p.y - 40/d, 250/d, 40/d);
		
		/*switch (PatronState) 
		{
		case(PatronStates.wandering):
			
			break;
		case(PatronStates.searchingSelf):
			
			GUI.DrawTexture(ui_pos, grey_texture);
			ui_pos.width =  SearchTime/4/d;
			GUI.DrawTexture(ui_pos, red_texture);
			break;
		case(PatronStates.takingBook): 
			GUI.DrawTexture(ui_pos, grey_texture);
			ui_pos.width =  TakeTime/4/d;
			GUI.DrawTexture(ui_pos, blue_texture);
			break;
		case(PatronStates.askingPatron):
			
			
			GUI.DrawTexture(ui_pos, grey_texture);
			ui_pos.width =  ConversationTime/4/d;
			GUI.DrawTexture(ui_pos, yellow_texture);
			
			ui_pos.y -= 40.0f/d;
			ui_pos.width = 250/d;
			
			GUI.Box(ui_pos,ConvoText  );
			break;
		case(PatronStates.askingLibrarian):
			GUI.DrawTexture(ui_pos, grey_texture);
			ui_pos.width =  ConversationTime/4/d;
			GUI.DrawTexture(ui_pos, darkyellow_texture);
			break;
		case(PatronStates.fleeingWerewolf):
			
			break;
		case(PatronStates.leavingWithBook):
			
			break;
		default:						 
			
			break;
		}*/
		
	}
}

