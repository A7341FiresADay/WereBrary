
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_Star {

	List<GameObject> nodes {get{
		return GameObject.Find ("map").GetComponent<map_layout> ().posts;
	}}

	public GameObject Target;

	public A_Star(){ }



	public Vector3 short_term_target(GameObject _agent) //switch to storing later
	{
		GameObject agent_node = nearist_node_to(nodes, _agent);
		List<GameObject> path = a_star(agent_node, Target);
		
		foreach(GameObject p in path){
			Debug.DrawLine(p.transform.position,p.transform.position + Vector3.up*100, Color.red );
		}
		if (path.Count > 1) 
		{
			return path [1].transform.position;
		} 
		else 
		{
			return _agent.transform.position;
		}
	}
	


	//---------------------------private--------------------------------------


	
	private List<GameObject> a_star(GameObject start, GameObject goal)
	{
		List<GameObject> closedset = new List<GameObject> ();
		List<GameObject> openset = new List<GameObject>();
		openset.Add (start);
		Dictionary<GameObject, float> g_score = new Dictionary<GameObject, float> ();
		Dictionary<GameObject, float> f_score = new Dictionary<GameObject, float> ();
		g_score [start] = 0;
		f_score [start] = get_heuristic_cost (start, goal);

		
		Dictionary<GameObject, GameObject> came_from = new Dictionary<GameObject, GameObject>();

		while (openset.Count > 0) 
		{

			GameObject current = lowest_f_score(openset, f_score);
			
			if(current == goal){
				return reconstruct_path(came_from, goal);
			}
			
			openset.Remove(current);
			closedset.Add(current);
			
			foreach(GameObject neighbor in neighbors(current))
			{
				if(closedset.Contains(neighbor))
				{
					continue;
				}

				float tenative_score = get_score(g_score, current) + 
								Vector3.Distance(neighbor.transform.position, current.transform.position);
				
				if( !openset.Contains(neighbor) || tenative_score < get_score(g_score, neighbor)) //if it's not on the open list, or it is, but this path is shorter...
				{
					came_from[neighbor] = current;
					g_score[neighbor] = tenative_score;
					f_score[neighbor] = g_score[neighbor] + get_heuristic_cost(neighbor, goal);
					if(!openset.Contains(neighbor))
					{
						openset.Add(neighbor);
					}
				}
			}

		}

		List<GameObject> to_return = new List<GameObject> ();
		to_return.Add (goal);
		return to_return;
	}

	private GameObject lowest_f_score(List<GameObject> set, Dictionary<GameObject, float> f_score) //SHOULD INCLUDE F (THIS IS JUST G SCORE
	{
		GameObject to_return = set [0];
		
		foreach(GameObject n in set){
			float score_1 = float.MaxValue;
			f_score.TryGetValue(n, out score_1);
			float score_2 = float.MaxValue;
			f_score.TryGetValue(to_return, out score_2);

			if(score_1 < score_2){
				to_return = n;
			}
		}
		
		return to_return;
	}
	
	private List<GameObject> neighbors(GameObject node)
	{
		List<GameObject> out_neighbors = new List<GameObject>();
		foreach (edge edge in node.GetComponent<node>().edges) {
			out_neighbors.Add( edge.to );
		}
		return out_neighbors;
	}
	
	private float get_score(Dictionary<GameObject, float> score, GameObject key){
		float out_val = 0;
		if( score.TryGetValue(key, out out_val) ){
			return out_val;
		}
		return 0;
		
	}

	private GameObject nearist_node_to(List<GameObject> these_nodes, GameObject close_to){
		float min_dist = float.MaxValue;
		GameObject nearist = (these_nodes.Count >= 1) ? these_nodes[0] : new GameObject();
		foreach (GameObject a_node in these_nodes) {
			if(a_node != null){
				float max_dist = Vector3.Distance(a_node.transform.position, close_to.transform.position);
				if(max_dist <= min_dist){
					min_dist = max_dist;
					nearist = a_node;
				}
			}
		}
		return nearist;
	}

	
	private List<GameObject> reconstruct_path(Dictionary<GameObject, GameObject> came_from, GameObject current_node){
		
		if (came_from.ContainsKey (current_node)) {
			List<GameObject> to_return = reconstruct_path(came_from, came_from[current_node]);
			to_return.Add(current_node);
			return to_return;
		} else {
			List<GameObject> to_return = new List<GameObject> ();
			to_return.Add (current_node);
			return to_return;
		}
	}


	private float get_heuristic_cost(GameObject n, GameObject t){
		return Vector3.Distance(n.transform.position, t.transform.position);
	}

}

