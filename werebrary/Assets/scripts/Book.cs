using UnityEngine;
using System.Collections;

//Represents a "Book".
//Probably should have just used a string
public class Book : MonoBehaviour {

	public string BookName;
	public float percentOwned = 100;

	void Start(){
		GetComponent<TextMesh>().fontSize = 50;
		transform.localScale = transform.localScale * 0.0005f;
	}

	void Update(){
		GetComponent<TextMesh>().text = BookName;
		//set the last parameter to a bigger alpha to show book names over shelves.
		GetComponent<TextMesh>().color = new Color(1, 1, 1, 0/100);

		transform.LookAt(GameObject.Find("Main Camera").transform);
		
		transform.Rotate(0, 180, 0);

	}

}
