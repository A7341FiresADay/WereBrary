using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * This is the hell class.
 */


//This holds all values that are used for the gentic algorithm.
public struct genetic_object
{
	public bool was_caught;
	public float search_speed;
	public float conversation_speed;
	public float take_speed;
	public Vector2 next_talk_timespan;
	public float werewolf_flee_range;
	public float patron_talk_range;
	public float librarian_talk_range;
}

//This is in charge of patron behavior trees, etc
public class PatronController : MonoBehaviour {

	public bool got_book;

	/* Genetic Variables */
	public bool was_caught; //if the patron was caught, this will be true when they return, making them invalid gentic matrial.
	public float search_speed;
	public float conversation_speed;
	public float take_speed;
	public Vector2 next_talk_timespan;
	public float werewolf_flee_range;
	public float patron_talk_range;
	public float librarian_talk_range;

	//If the librarian saves someone, they get generic genetics, and will not qualify for the gene pool.
	private bool overrode_default_genetics = false;

	//used to populate the genetic values.
	public void seed_genetics(bool _was_caught, 
	                          float _search_speed,
	                          float _conversation_speed,
	                          float _take_speed,
	                          Vector2 _next_talk_timespan,
	                          float _werewolf_flee_range, 
	                          float _patron_talk_range,
	                          float _librarian_talk_range)
	{
		was_caught = _was_caught;
		search_speed = _search_speed;
		conversation_speed = _conversation_speed;
		take_speed = _take_speed;
		next_talk_timespan = _next_talk_timespan;
		werewolf_flee_range = _werewolf_flee_range;
		patron_talk_range = _patron_talk_range;
		librarian_talk_range = _librarian_talk_range;

		overrode_default_genetics = true;
	}

	//passes out the genetics (used when the patron is finished to add them to pool.
	public genetic_object get_genetics()
	{
		genetic_object ret = new genetic_object();

		ret.was_caught = was_caught;
		ret.search_speed = search_speed;
		ret.conversation_speed = conversation_speed;
		ret.take_speed = take_speed;
		ret.next_talk_timespan = next_talk_timespan;
		ret.werewolf_flee_range = werewolf_flee_range;
		ret.patron_talk_range = patron_talk_range;
		ret.librarian_talk_range = librarian_talk_range;

		return ret;
	}



	public string BookToFind; // title of the desired book.
	public GameObject CarriedBook; //current book character is carrying.
	
	public float SearchTime; // Time until shelf is searched
	public float TakeTime; //Time unti book has been taken from shelf

	//resets the timers on conversations/takes/etc.
	public void ResetTimers(){
		SearchTime = search_speed;
		TakeTime = take_speed;
		ConversationTime = conversation_speed;
	}


	
	
	//toogled to prevent partron from talking to librarian forever
	public bool knows_all = false;
	public List<bookshelf> KnownSelves; // Shelves this patron knows the content of.
	public bookshelf TargetShelf; //shelf to try next.
	
	public float TimeToNextTalk; // Time until next conversation is allowed
	public float ConversationTime; // Time until convo is finsihed
	public string ConvoText; //gibberish to say in next conversation
	public GameObject ConversationTarget; //Fellow to talk to.


	//What the patron is currently doing
	PatronStates patronState;
	public PatronStates PatronState{
		get{
			return patronState;
		}
		set{
			if( value != patronState){
				ResetTimers();
				patronState = value;
			}
		}

	}

	//what the patron could be doing
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
		//weird, weird hotfix. Probably redundant. Magic.
		if(got_book != true){
			got_book = false;
		}

		//Gives default genetics if none were provided.
		if(!overrode_default_genetics)
		{
			
			seed_genetics(true, 
		                  1000,
		                  1000,
		                  1000,
		                  new Vector2(500, 2000),
		                  7, 
		                  4,
		                  3);

		}

		//BookToFind = GameObject.Find ("Shelves").GetComponent<BookselfStocker> ().AssignSeekerBook ();
		TimeToNextTalk = Random.Range(next_talk_timespan.x, next_talk_timespan.y);
		PatronState = PatronStates.wandering;

		//init known bookshelves (none)
		KnownSelves = new List<bookshelf> ();

		//choose a shelf to target
		target_random_shelf();

		//choose inital conversation text from the markov chain.
		ConvoText = random_talk();

		//load the colored bars for the UI.
		grey_texture = Resources.Load<Texture2D>("grey");
		red_texture = Resources.Load<Texture2D>("red");
		blue_texture = Resources.Load<Texture2D>("blue");
		yellow_texture = Resources.Load<Texture2D>("yellow");
		darkyellow_texture = Resources.Load<Texture2D>("musturd_yellow");


	}

	//generates a little bit of gibberish
	string random_talk(){
		
		 return new Gibberish ("Assets/scripts/PatronGibberish", Random.Range (2, 15)).FinalReturnGibberish;
	}

	//returns locaiton of a random bookshelf.
	Vector3 random_shelf_position(){
		TargetShelf = GameObject.FindGameObjectsWithTag ("Bookshelf")[(int)(Random.value * GameObject.FindGameObjectsWithTag ("Bookshelf").Length)].GetComponent<bookshelf>();
		return TargetShelf.gameObject.transform.position;
	}

	//sets the target location to a random bookshelf.
	void target_random_shelf(){
		GetComponent<NavMeshAgent>().SetDestination( random_shelf_position() );
	}

	//sets desitination to given Vector3.
	void target_obj(Vector3 target){
		GetComponent<NavMeshAgent>().SetDestination(target);
	}

	//Checks if target shelf location is known.
	//tsk caches the check so I'm not looking it up every update.
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

	//returns the target bookshelf, or a random shelf if it is unknown.
	//use target_shelf_known to check if it is a random shelf or the correct shelf.
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

	//returns the nearest partron
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

	//returns the nearest librarain
	GameObject nearest_librarian() {
		
		Object[] librarians = FindObjectsOfType<Librarian>();
		if(librarians.Length <= 0){return gameObject;}

		GameObject nl = ((Librarian)librarians[0]).gameObject;
		float dist = int.MaxValue;
		for (int i = 0; i < librarians.Length; i++) {
			GameObject librarian = ((Librarian)librarians[i]).gameObject;
			float temp_dist = Vector3.Distance(transform.position, librarian.transform.position);
			if(dist > temp_dist && librarian != gameObject){
				dist = temp_dist;
				nl = librarian;
			}
		}
		return nl;
	}

	//returns nearest werewolf
	GameObject nearest_werewolf() {

		Object[] werewolves = FindObjectsOfType<NavMeshWerewolf>();
		if(werewolves.Length <= 0){return gameObject;}

		GameObject nw = ((NavMeshWerewolf)werewolves[0]).gameObject;
		float dist = int.MaxValue;
		for (int i = 0; i < werewolves.Length; i++) {
			GameObject werewolf = ((NavMeshWerewolf)werewolves[i]).gameObject;
			float temp_dist = Vector3.Distance(transform.position, werewolf.transform.position);
			if(dist > temp_dist && werewolf != gameObject){
				dist = temp_dist;
				nw = werewolf;
			}
		}
		return nw;
	}


	// Update is called once per frame
	void Update () {

		//countdown until you can have another conversation. Some random jigger on the countdown.
		TimeToNextTalk -= Random.Range(0, 100) * Time.deltaTime;

		//assume you're just wandering
		var new_patron_state = PatronStates.wandering;
		if(target_shelf_known () ){ //if you know where to go, go there
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

		//if there is a librarian nearby, and you're ready to talk to them, do so
		GameObject nl = nearest_librarian ();
		if (nl != gameObject && Vector3.Distance (nl.transform.position, transform.position) < librarian_talk_range && !knows_all) {
			new_patron_state = PatronStates.askingLibrarian;
			ConversationTarget = nl;
		}

		//If you've got the book...
		if(CarriedBook != null){


			if(CarriedBook.GetComponent<Book>().BookName == BookToFind){ // skedaddle!
				new_patron_state = PatronStates.leavingWithBook;
			}
		}

		//If you're ready and able to share your knowledge with another patron, have that talk.
		GameObject np = nearest_patron ();
		if (Vector3.Distance (np.transform.position, transform.position) < patron_talk_range && TimeToNextTalk <= 0 && np != gameObject && CarriedBook == null) {
			new_patron_state = PatronStates.askingPatron;
			ConversationTarget = np;
		}

		//finally, if you ever see a werewolf, drop everything and RUN!
		GameObject nw = nearest_werewolf();
		if (nw != gameObject && !Physics.Linecast(nw.transform.position, transform.position) && Vector3.Distance (nw.transform.position, transform.position) < werewolf_flee_range) {
			new_patron_state = PatronStates.fleeingWerewolf;
		}
	
		//Update your state.
		PatronState = new_patron_state;



		//do the behavior for the chosen state
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

	//go somewhere random
	void wander(){
		GetComponent<NavMeshAgent> ().Resume ();
		transform.position = GetComponent<NavMeshAgent>().nextPosition;

		//GameObject.FindGameObjectsWithTag ("Bookshelf");
	}

	//countdown to knowing the name of a book
	void searchSelf(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 2) {
			SearchTime -= Time.deltaTime * 200;
		}
		if (SearchTime <= 0) {
			KnownSelves.Add(TargetShelf);
		}

	}

	//countdown to taking a book
	void takeBook(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, TargetShelf.transform.position) < 2) {
			TakeTime -= Time.deltaTime * 200;


		}
		got_book = true;
		if (TakeTime <= 50) {
			got_book = true;

			CarriedBook = TargetShelf.book;
			TargetShelf.book = null;
		}
	}

	//Coundown to exchange knowledge.
	void askPatron(){
		GetComponent<NavMeshAgent> ().Stop ();
		if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < patron_talk_range) {
			ConversationTime -= Time.deltaTime * 200;
			// Debug.Log( Gibberish.StringFromPool("Assets/scripts/PatronGibberish") ); <-- gibberish
		}
		if (ConversationTime <= 0) {
			ResetTimers();
			
			ConvoText = random_talk();
			KnownSelves.AddRange( ConversationTarget.GetComponent<PatronController>().KnownSelves );
			TimeToNextTalk = Random.Range(next_talk_timespan.x, next_talk_timespan.y);
		}
	}

	//coundown to ask librairan for books
	void askLibrarian(){

		GetComponent<NavMeshAgent> ().Stop ();
		if (!nearest_librarian().GetComponent<Librarian>().CheckingSomething)
			nearest_librarian ().GetComponent<Librarian> ().startWait (); /*CheckingSomething = true;*/
		if (Vector3.Distance (transform.position, ConversationTarget.transform.position) < librarian_talk_range) {
			ConversationTime -= 250 * Time.deltaTime;
		}
		if(Random.Range(0, 100) < 1){ConvoText = random_talk();}


		if (ConversationTime <= 0) {
			ResetTimers();

			//make sure to get knowledge of 10 random book, plus your desired book.
			int[] others = new int[10];
			int max = GameObject.Find("Shelves").transform.childCount;
			for(int i = 0; i < others.Length; i++){
				others[i] = (int)Random.Range(0, max);
			}
			for (int j = 0; j < max; j++) 
			{
				GameObject shelf = GameObject.Find ("Shelves").transform.GetChild (j).gameObject;
				bookshelf s = shelf.GetComponent<bookshelf> ();

				//If some books are already taken, the shelf will be null. You cannot learn about it.
				if(s.book == null){ continue; }

				//if it's your shelf, take knowledge
				if(s.book.GetComponent<Book>().BookName == BookToFind){
					KnownSelves.Add (s);
				}
				//if it's one of the other 10 random shelves, take knowledge
				for(int i = 0; i < others.Length; i++){
					if(others[i] == j && !KnownSelves.Contains(s)){
						KnownSelves.Add (s);
					}
				}
			}

			knows_all = true;
			//1- Take knowledge of desired book. 
			//2- Take knowledge of any other book

		}

	}

	//move away from werewolf
	void fleeWerewolf(){
		GameObject nw = nearest_werewolf();

		if (nw != null) 
		{
			Vector3 flee_target = 10 * (transform.position - nw.transform.position) - transform.position;
			flee_target.y = transform.position.y;
			GetComponent<NavMeshAgent> ().SetDestination (flee_target);
		}


	}

	//move to exit/enternace
	void leaveWithBook(){
		
		GetComponent<NavMeshAgent> ().Resume ();
		target_obj(GameObject.Find("Enterance").transform.position);
		transform.position = GetComponent<NavMeshAgent>().nextPosition;
	}

	//textures for bars over heads
	public Texture grey_texture;
	public Texture red_texture;
	public Texture blue_texture;
	public Texture yellow_texture;
	public Texture darkyellow_texture;
	void OnGUI(){

		//get a rectange that is positioned and scaled to look like it is in the 3D environment and not hovering over on the UI layer
		var p = Camera.main.WorldToScreenPoint(transform.position);
		var d = p.z/10;//shrink the width/height based on distance.
		Rect ui_pos = new Rect(p.x - 250/d/2, Screen.height - p.y - 40/d, 250/d, 40/d);

		//draw different bars based on activity type.
		switch (PatronState) 
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

				ui_pos.y -= 25.0f;
				ui_pos.height += 20.0f;
				GUI.skin.textField.wordWrap = true;
				GUI.DrawTexture(ui_pos, grey_texture);
				GUI.TextField(ui_pos,ConvoText );
			break;
			case(PatronStates.askingLibrarian):
				GUI.DrawTexture(ui_pos, grey_texture);
				ui_pos.width =  ConversationTime/4/d;
				GUI.DrawTexture(ui_pos, darkyellow_texture);

			
				ui_pos.y -= 40.0f/d;
				ui_pos.width = 250/d;
				
				ui_pos.y -= 25.0f;
				ui_pos.height += 20.0f;
				GUI.skin.textField.wordWrap = true;
				GUI.DrawTexture(ui_pos, grey_texture);
				GUI.TextField(ui_pos,ConvoText );
			break;
			case(PatronStates.fleeingWerewolf):

			break;
			case(PatronStates.leavingWithBook):

			break;
			default:						 

			break;
		}

	}
}

