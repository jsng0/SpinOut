using UnityEngine;
using System.Collections;

/// <summary>
/// 2013-05-14
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// Base projectile class, not used in the demo scene
/// </summary>
public class clscannonball : MonoBehaviour {
	public bool vargamenabled = true;
	public clscannon varcannon = null;
	
	void OnCollisionEnter() {
		if (vargamenabled) {
			if (varcannon != null)
				varcannon.metresetactor();
		}
		vargamenabled = false;
	}
}
