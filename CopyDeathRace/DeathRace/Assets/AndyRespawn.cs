using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AndyRespawn : MonoBehaviour {

	public GameObject[] spawn_points;
	public GameObject theHUD;

	public float max_res_time = 5f;

	public float res_time = 5f;

	private GameObject target_spawn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		res_time += Time.deltaTime;
		if( res_time < max_res_time )
			keep_waiting();
		else if( Input.GetKey("r"))
			respawn();
		else clear_screen();

	
	}

	public void clear_screen()
	{
		UnityEngine.UI.Text[] s = theHUD.GetComponentsInChildren<Text>();
		s[0].text = "";
	}

	public void keep_waiting()
	{

		transform.position = target_spawn.transform.position;
		transform.rotation = target_spawn.transform.rotation;
		GameObject g = this.gameObject;
		YellowCar yc =  g.GetComponent("YellowCar") as YellowCar;
		yc.vel = 0;
		transform.rigidbody.angularVelocity = Vector3.zero;
		transform.rigidbody.velocity = Vector3.zero;
		
		UnityEngine.UI.Text[] s = theHUD.GetComponentsInChildren<Text>();
		s[0].text = (int)(max_res_time - res_time + 1) + "";
	}

	public void respawn()
	{
		float max_dist = 0;
		int best_choice = 0;
		
		for( int i  = 0; i < spawn_points.Length; ++i)
		{
			//get farthest point
			float dist = Vector3.Distance( transform.position, spawn_points[i].transform.position);
			if(dist > max_dist)
			{
				max_dist = dist;
				best_choice = i;
			}
		}
		

		//get random choice
		//best_choice = Random.Range(0, spawn_points.Length);

		target_spawn = spawn_points[best_choice];
		transform.position = target_spawn.transform.position;
		transform.rotation = target_spawn.transform.rotation;
		transform.rigidbody.angularVelocity = Vector3.zero;
		transform.rigidbody.velocity = Vector3.zero;
		GameObject g = this.gameObject;
		YellowCar yc =  g.GetComponent("YellowCar") as YellowCar;
		yc.vel = 0;
		res_time = 0f;

		UnityEngine.UI.Text[] s = theHUD.GetComponentsInChildren<Text>();
		s[0].text = (int)(max_res_time - res_time + 1) + "";



	}

}
