using UnityEngine;
using System.Collections;

public class LibraryEntrance : MonoBehaviour {

	public GameObject PatronModel;

	public int InitialPatrons;
	public int PatronsReturned;


	// Use this for initialization
	void Start () {
		BookStore bookDisp = new BookStore();


		for (int i = 0; i < InitialPatrons; i++) {
			var pos = transform.position;
			pos.z += i * 1.5f;
			GameObject patron = (GameObject)Instantiate(PatronModel, pos, Quaternion.identity);
			PatronController pc = patron.AddComponent<PatronController>();
			pc.BookToFind = bookDisp.GetBook();
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
