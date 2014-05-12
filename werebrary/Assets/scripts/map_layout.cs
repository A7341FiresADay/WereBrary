using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class map_layout : MonoBehaviour {

	public List<GameObject> posts;
	public GameObject post_prefab;
	float grid_width = 25;
	float grid_size = 10;


	// Use this for initialization
	void Awake () {
		posts = new List<GameObject> ();
		for (int i = 0; i < grid_width; i++) {
			for(int j = 0; j < grid_width; j++){
				GameObject new_post = (GameObject)Instantiate(post_prefab, 
				                                              new Vector3(grid_size*i, 0, grid_size*j), 
				                                              Quaternion.identity);
				new_post.transform.parent = transform;
				posts.Add(new_post);
			}
		}

		foreach (GameObject node in posts) {
			foreach(GameObject edge_to in all_neighbors(node)){
				node.GetComponent<node>().add_edge(edge_to, 1);
			}
		}

		
	}

	List<GameObject> all_neighbors(GameObject self){
		var neighbors = new List<GameObject> ();
		foreach (GameObject node in posts) {
			if(Vector3.Distance(self.transform.position, node.transform.position) <= grid_size){
				neighbors.Add(node);
			}
		}

		return neighbors;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject node in posts) {
			foreach(edge e in node.GetComponent<node>().edges){
				//e.weight
				Debug.DrawLine(node.transform.position, e.to.transform.position, new Color(100,0,0, 0.1f));
			}
		}
	}
}
