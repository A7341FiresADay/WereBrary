using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class agent : MonoBehaviour {

	public A_Star AStar;

	List<GameObject> nodes{
		get
		{
			return GameObject.Find ("map").GetComponent<map_layout> ().posts;
		}
	}

	// Use this for initialization
	void Start () 
	{
		AStar = new A_Star ();
		choose_target ();
	}
	
	// Update is called once per frame
	void Update () {	
		Vector3 stt =  AStar.short_term_target ( gameObject ) ;
		transform.position = Vector3.MoveTowards (transform.position, stt, 0.5f);

		Debug.DrawLine (transform.position, AStar.Target.transform.position, Color.blue);
		Debug.DrawLine (transform.position, stt, Color.gray);


	}

	public void choose_target(){
		AStar.Target = nodes [(int)Random.Range (0, nodes.Count - 1)];
		
	}


}










