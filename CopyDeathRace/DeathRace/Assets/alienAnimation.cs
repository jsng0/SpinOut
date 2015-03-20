using UnityEngine;
using System.Collections;

public class alienAnimation : MonoBehaviour {

	PlayerWeaponController weaponScript;

	// Use this for initialization
	void Start () {
		weaponScript = gameObject.GetComponent<PlayerWeaponController>();
	}
	
	// Update is called once per frame
	void Update () {
//		if (weaponScript.weaponCounter == ' ')
//			animation.Play ("Idle");
//		if (weaponScript.weaponCounter == 'w')
//			animation.Play("FingerFire");
	}
}
