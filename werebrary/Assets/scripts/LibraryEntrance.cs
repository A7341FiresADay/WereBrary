using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//The location where patrons spawn.
//Also, the location where patrons leave the library and
//manages genetics
public class LibraryEntrance : MonoBehaviour {

	public GameObject PatronModel;

	//The number of patrons to start with.
	public int InitialPatrons;
	public int PatronsReturned;
	
	//a list of the genetics of each patron that made it back
	public List<genetic_object> returned_patrons;

	// Use this for initialization
	void Start () {
		returned_patrons = new List<genetic_object>();
		BookStore bookDisp = new BookStore();


		//spawn a parton for each "initalPatron".
		for (int i = 0; i < InitialPatrons; i++) {
			var pos = transform.position;
			pos.z += i * 1.5f;
			GameObject patron = (GameObject)Instantiate(PatronModel, pos, Quaternion.identity);
			PatronController pc = patron.AddComponent<PatronController>();
			//assign a unique book
			pc.BookToFind = bookDisp.GetBook();
		
			//seed the inital genetics.
			pc.seed_genetics(false, 
			              1000,
			              1000,
			              1000,
			              new Vector2(500, 2000),
			              7, 
			              4,
			              3);
			//pc.get_genetics();


			
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		PatronController[] patrons = FindObjectsOfType<PatronController>();
		for (int i = 0; i < patrons.Length; i++) 
		{
			GameObject patron = patrons[i].gameObject;
			PatronController patron_controller = patrons[i];

			//check if anyone has returned with their book.
			if(Vector3.Distance(patron.transform.position, transform.position) < 3 && patron_controller.got_book == true)
			{
				//if they have, add their genetics, destroy them, and spawn a new patron.
				returned_patrons.Add(patron_controller.get_genetics());
				Destroy(patron);
				spawn_new_patron();
			}

		}


	
	}

	void spawn_new_patron()
	{
		GameObject patron = (GameObject)Instantiate(PatronModel, transform.position, Quaternion.identity);
		PatronController pc = patron.AddComponent<PatronController>();
		
		BookStore bookDisp = new BookStore();
		pc.BookToFind = bookDisp.GetBook();

		genetic_object patron_obj = new genetic_object();
		patron_obj.was_caught = false;
		patron_obj.search_speed = 1000;
		patron_obj.conversation_speed = 1000;
		patron_obj.take_speed = 1000;
		patron_obj.next_talk_timespan = new Vector2(500, 2000);
		patron_obj.werewolf_flee_range = 7;
		patron_obj.patron_talk_range = 4;
		patron_obj.librarian_talk_range = 3;

		//use the genetic backlog to weight the new patron.
		for(int i = 0; i < returned_patrons.Count; i++)
		{
			//add in rolling average for each property from list, devalue for age and number of other values.
			if(!returned_patrons[i].was_caught)
			{
				patron_obj.search_speed 	    += returned_patrons[i].search_speed/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.conversation_speed   += returned_patrons[i].conversation_speed/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.take_speed 	        += returned_patrons[i].take_speed/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.next_talk_timespan.x += returned_patrons[i].next_talk_timespan.x/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.next_talk_timespan.y += returned_patrons[i].next_talk_timespan.y/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.werewolf_flee_range  += returned_patrons[i].werewolf_flee_range/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.patron_talk_range    += returned_patrons[i].patron_talk_range/(returned_patrons.Count*i) * Random.Range(-1, 1);
				patron_obj.librarian_talk_range += returned_patrons[i].librarian_talk_range/(returned_patrons.Count*i) * Random.Range(-1, 1);

			}

		}

		//pass the weighted genetics to the new patron.
		pc.seed_genetics(
			patron_obj.was_caught,
			patron_obj.search_speed,
			patron_obj.conversation_speed,
			patron_obj.take_speed,
			patron_obj.next_talk_timespan,
			patron_obj.werewolf_flee_range,
			patron_obj.patron_talk_range,
			patron_obj.librarian_talk_range);
	}




}












