using UnityEngine;
using System.Collections;

/// <summary>
/// 2013-05-14
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// helper class to release the parent of a ragdoll prop, and avoid rigidbody jitter
/// NOTE: this script was made obsolete by the more versatile and generic clsdrop
/// </summary>
public class clsragdollrelease : MonoBehaviour {
	void Start () {
		transform.parent = null;
	}
}
