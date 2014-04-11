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
	

	// set in editor to promote reusability.
	public int numberOfvillagers;
	public int numberOfWerewolves;
	public Object villagerPrefab;
	public Object werewolfPrefab;
	public Object obstaclePrefab;
	public Object followerPrefab;

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


	// array of obstacles with accessor
	private  GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}

		
	//Set stage for game, creating characters and the simple GUI implemented.
	public void Start ()
	{
		instance = this;

		obstacles = GameObject.FindGameObjectsWithTag ("Bookshelf");

	}
	
	public void Update( )
	{
		//calcCentroid( );//find average position of each flocker 
//		calcFlockDirection( );//find average "forward" for each flocker
		//calcDistances( );
	}
	
	/*private void calcFlockDirection ()
	{
		
		flockDirection = new Vector3();
		
		// calculate the average heading of the flock
		// use transform.
		for(int i = 0; i < villagers.Count; i++)
		{	
			flockDirection = flockDirection + villagers[i].transform.forward; 
		}
		
	}*/
	
	/*public void createNewVillager()
	{
		Villager villager;
		Follow follower;
		
		villagers.Add ((GameObject)Instantiate (villagerPrefab, 
				new Vector3 (371 + UnityEngine.Random.Range(0,10), 5, 365), Quaternion.identity));
			//grab a component reference
			villager = villagers[villagers.Count-1].GetComponent<Villager> ();
			villagers[villagers.Count-1].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
			//set values in the Vehicle script
			villager.Index = villagers.Count-1;
		
			VillageFollowers.Add((GameObject)Instantiate(followerPrefab, 
				new Vector3(371 + UnityEngine.Random.Range(0,10), 5, 365), Quaternion.identity));
			
			//Create a follower for the minimap
			follower = VillageFollowers[VillageFollowers.Count-1].GetComponent<Follow> ();
			follower.ToFollow = villagers[villagers.Count-1];
			VillageFollowers[VillageFollowers.Count-1].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
			villager.Follower = follower;
		
		
	}*/
	
	/*void calcDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfvillagers; i++)
		{
			for( int j = i+1; j < numberOfvillagers; j++)
			{
				dist = Vector3.Distance(villagers[i].transform.position, villagers[j].transform.position);
				distances[i, j] = dist;
				distances[j, i] = dist;
			}
		}
	}*/
	
	/*public float getDistance(int i, int j)
	{
		return distances[i, j];
	}*/
		
	/*private void calcCentroid ()
	{
		// calculate the current centroid of the flock
		// use transform.position
		
		centroid = new Vector3();
		
		for(int i = 0; i < villagers.Count; i++)
		{
			centroid = centroid + villagers[i].transform.position;	
		}
		
		centroid = centroid / villagers.Count;
		
		centroidContainer.transform.position = new Vector3(100,100,100);
	}*/
	

}