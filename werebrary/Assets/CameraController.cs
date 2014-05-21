using UnityEngine;
using System.Collections;

/*
 * This class controls the camera in the scene.
 * The camera moves to random points around the edge of the map
 * and looks at random partons.
 * The look and move are on different timers.
 * 
 * 
 */
public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		target_obj = Random_patron ();
	}

	//Countdown until the camera retargets
	private float retarget_in = 2.0f;
	//Time since the camera last retargeted
	private float time_since_retarget = 0.0f;

	//time until next move
	private float move_in = 15.0f;
	private float time_since_move = 1000.0f;
	private Vector3 target_pos = new Vector3 (0, 0, 0);
	GameObject target_obj;
	void Update () {
		//if the tatget was destroyed, look at someone else.
		if(target_obj == null)
		{
			target_obj = Random_patron();
			target_pos = target_obj.transform.position;
		}

		//countdown to retarget
		time_since_retarget += Time.deltaTime;
		if (time_since_retarget > retarget_in ) {
			time_since_retarget = 0.0f;
			retarget_in = Random.Range(5.0f, 15.0f);

			target_obj = Random_patron ();
		}

		//smoothly look at target.
		var rot = Quaternion.LookRotation((target_obj.transform.position - transform.position).normalized);
		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime * 1);


		//countdown to move
		time_since_move += Time.deltaTime;
		if (time_since_move > move_in ) {
			time_since_move = 0.0f;
			move_in = Random.Range(15.0f, 35.0f);

			if(Random.Range(0, 100) < 50){
				if(Random.Range(0, 1) == 1){
					target_pos = new Vector3( 15, transform.position.y, Random.Range(-15, 30));
				} else {
					target_pos = new Vector3(-30, transform.position.y, Random.Range(-15, 30));
				}
			}
			else {
				if(Random.Range(0, 100) < 50){
					target_pos = new Vector3( Random.Range(-30, 15), transform.position.y, -15);
				} else {
					target_pos = new Vector3(Random.Range(-30, 15), transform.position.y, 30);
				}
			}

		}

		//ugly way to say move slowly to target location.
		if (Vector3.Distance (target_pos, transform.position) > 3) {

			
			float move_speed = 2.0f;

			if (transform.position.x < target_pos.x) {
				transform.position = new Vector3(transform.position.x + (move_speed * Time.deltaTime) ,transform.position.y, transform.position.z);
			} else {
				transform.position = new Vector3(transform.position.x - (move_speed * Time.deltaTime) ,transform.position.y, transform.position.z);
			}
			if (transform.position.z < target_pos.z) {
				transform.position = new Vector3(transform.position.x ,transform.position.y, transform.position.z+ (move_speed * Time.deltaTime));
			} else {
				transform.position = new Vector3(transform.position.x ,transform.position.y, transform.position.z- (move_speed * Time.deltaTime));
			}
		}
		
	}

	//chooses a random gameobject with a PatronController on it.
	GameObject Random_patron(){
		Object[] patrons = FindObjectsOfType<PatronController>();
		return ((PatronController)patrons[(int)Random.Range(0, patrons.Length)]).gameObject;
	}
}

