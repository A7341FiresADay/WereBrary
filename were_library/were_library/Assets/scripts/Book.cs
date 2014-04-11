using UnityEngine;
using System.Collections;

public class Book : MonoBehaviour {

	public string BookName;
	public float percentOwned = 100;

	void Start(){
		GetComponent<TextMesh>().fontSize = 15;
	}

	void Update(){
		GetComponent<TextMesh>().text = BookName;
		GetComponent<TextMesh>().color = new Color(1, 1, 1, percentOwned/100);

		transform.LookAt(GameObject.Find("Main Camera").transform);
		
		transform.Rotate(0, 180, 0);

	}

}
