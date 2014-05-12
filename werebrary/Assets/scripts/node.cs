using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class node : MonoBehaviour {

	public List<edge> edges;

	// Use this for initialization
	void Start () {
	}
	public void add_edge(GameObject to, float weight)
	{
		if (edges == null) {
			edges = new List<edge> ();
		}
		edge e = new edge ();
		e.weight = weight;
		e.to = to;
		edges.Add (e);
	}





}


public struct edge {
	public GameObject to;
	public float weight;

}