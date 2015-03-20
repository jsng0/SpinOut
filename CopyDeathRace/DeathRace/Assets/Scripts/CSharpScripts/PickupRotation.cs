using UnityEngine;
using System.Collections;

public class PickupRotation : MonoBehaviour {

	public float rotateSpeed = 45.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate( Vector3.up, rotateSpeed * Time.deltaTime );
	}
}
