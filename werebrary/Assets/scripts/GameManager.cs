using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;

[System.Serializable]
public class GameManager : MonoBehaviour
{
	// weight parameters are set in editor and used by all villagers 
	// if they are initialized here, the editor will override settings	 
	// weights used to arbitrate btweeen concurrent steering forces 
	public float alignmentWt;
	public float separationWt;
	public float cohesionWt;
	public float avoidWt;
	public float inBoundsWt;

	// these distances modify the respective steering behaviors
	public float avoidDist;
	public float separationDist;

	public Librarian librarian;
	public GameObject[] villagers;

	public Librarian currLibrarian { get { return librarian; } }
	//values used by all villagers that are calculated by controller on update
	private Vector3 flockDirection;
	private Vector3 centroid;
	
	//accessors
	private static GameManager instance;
	public static GameManager Instance { get { return instance; } }


	public Vector3 FlockDirection {
		get { return flockDirection; }
	}
	
	public Vector3 Centroid { get { return centroid; } }
	public GameObject centroidContainer;

	public GameObject plane;// = GameObject.FindGameObjectWithTag("Plane");

	public GameObject Plane { 
		get { 
			return plane; 
		}
	}

	// array of obstacles with accessor
	private  GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}

		
	//Set stage for game, creating characters and the simple GUI implemented.
	public void Start ()
	{
		instance = this;

		obstacles = GameObject.FindGameObjectsWithTag ("Bookshelf");

		//librarianText = librarian.GetComponentInChildren<TextMesh> ();

	}
	
	public void Update( )
	{
	
	}
}