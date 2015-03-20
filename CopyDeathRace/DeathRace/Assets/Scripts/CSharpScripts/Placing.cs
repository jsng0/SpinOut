using UnityEngine;
using System.Collections;

public class Placing : MonoBehaviour {
	
	private GameObject player1;
	private GameObject player2;
	// Use this for initialization
	void Start () {
		player1 = GameObject.Find ("Player1");
		player2 = GameObject.Find ("Player2");
	}
	
	// Update is called once per frame
	void Update () {
		if(player1.GetComponent<Respawn>().getSpawnNum() > 
		   player2.GetComponent<Respawn>().getSpawnNum())
		{
			player1.GetComponent<CarLap> ().place = 1;
			player2.GetComponent<CarLap> ().place = 2;
		}
		if(player1.GetComponent<Respawn>().getSpawnNum() < 
		   player2.GetComponent<Respawn>().getSpawnNum())
		{
			player1.GetComponent<CarLap> ().place = 2;
			player2.GetComponent<CarLap> ().place = 1;
		}
	}
}