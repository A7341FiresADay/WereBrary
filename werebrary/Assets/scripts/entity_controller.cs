using UnityEngine;
using System.Collections;

public class entity_controller : MonoBehaviour {

	public GameObject entity;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < 1; i++){
			GameObject ent = (GameObject)Instantiate(entity, new Vector3(3,0,3), Quaternion.identity);
			ent.transform.parent = transform;
		}

	}

}
