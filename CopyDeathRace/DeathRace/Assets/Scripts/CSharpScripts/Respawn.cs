using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour {

	public GameObject[] spawnpoint;
	private int spawnpoint_num;
	private YellowCar car;
	public GameObject center;
	// Use this for initialization
	void Start () {
		car = gameObject.GetComponent<YellowCar> ();	
	}

	public int getSpawnNum()
	{
		return spawnpoint_num;
	}
	public void setSpawnNum(int set)
	{
		spawnpoint_num = set;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "spawnpoint0" && spawnpoint_num == 6)
		{
			spawnpoint_num = 0;
		}
		if(other.gameObject.name == "spawnpoint1" && spawnpoint_num == 0)
		{
			spawnpoint_num = 1;
		}
		if(other.gameObject.name == "spawnpoint2" && spawnpoint_num == 1)
		{
			spawnpoint_num = 2;
		}
		if(other.gameObject.name == "spawnpoint3" && spawnpoint_num == 2)
		{
			spawnpoint_num = 3;
		}
		if(other.gameObject.name == "spawnpoint4" && spawnpoint_num == 3)
		{
			spawnpoint_num = 4;
		}
		if(other.gameObject.name == "spawnpoint5" && spawnpoint_num == 4)
		{
			spawnpoint_num = 5;
		}
		if(other.gameObject.name == "spawnpoint6" && spawnpoint_num == 5)
		{
			spawnpoint_num = 6;
		}
	}
/*
	public void respawn()
	{
		gameObject.SetActive (false);
		car.vel = 0;
		transform.position = new Vector3(spawnpoint[spawnpoint_num].transform.position.x,10,
		                                 spawnpoint[spawnpoint_num].transform.position.z);
		transform.rotation = spawnpoint[spawnpoint_num].transform.rotation;
		//Debug.Log("Respawn Complete");
		gameObject.SetActive (true);
		gameObject.GetComponent<PlayerHealth> ().HealCar(100);
		
	}*/
	// Update is called once per frame
	void Update () {
		//Debug.LogError(Vector3.Distance(transform.position,center.transform.position) > 500);
		/*if (gameObject.GetComponent<PlayerHealth> ().currentHealth == 0 ||
		    Vector3.Distance(transform.position,center.transform.position) > 500)
		{
			//respawn ();
		}*/
	}
}
