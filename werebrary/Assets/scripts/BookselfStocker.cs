using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Holds a book.
//Once, held multiple books. Now, holds a single book. Could almost be a string.
public class BookselfStocker : MonoBehaviour {

	public GameObject Book;

	// Use this for initialization
 	void Start () {
		BookStore bookDisp = new BookStore();


		GameObject[] bookshelfObjects = GameObject.FindGameObjectsWithTag ("Bookshelf");

		for(int i = 0; i < bookshelfObjects.Length; i++){


			GameObject currentShelfObject = bookshelfObjects[i];
			bookshelf currentShelf = currentShelfObject.AddComponent<bookshelf>();

			//position book over shelf?
			currentShelf.book = (GameObject)Instantiate(Book, Vector3.zero, Quaternion.identity);
			currentShelf.book.transform.parent = currentShelfObject.transform;
			currentShelf.book.transform.position = currentShelfObject.transform.position;
			currentShelf.book.transform.position = new Vector3(currentShelf.book.transform.position.x, 
			                                                     currentShelf.book.transform.position.y + 2.0f, 
			                                                     currentShelf.book.transform.position.z);

			currentShelf.book.GetComponent<Book>().BookName = bookDisp.GetBook();

		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}




}
