using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {

	public float linearSpeed = 2.8f;
	public float blastRadius = 4.0f;
	public float explosionForce = 600.0f;
	public GameObject explosion;
	public float fallSpeed = 6.0f;
	
	Quaternion from;
	Quaternion to;
	
	void Awake () {
		transform.localScale = new Vector3(.3f,.3f,.3f);
		//transform.RotateAround(transform.position, Vector3.right, 15.0f);
		//transform.LookAt( transform.position + (Vector3.down + Ve ); 
		from = transform.rotation;
		//from.ToAngleAxis
		to.SetLookRotation(transform.forward+Vector3.down);
	}
	

	void Update () {	
		if( ! Physics.CheckSphere (transform.position , .5f ) )
		{
			transform.rotation = Quaternion.Slerp(from, to, (Time.deltaTime) * fallSpeed);
			transform.position += transform.forward * linearSpeed;
		}
		else 
		{
			Collider[] urDone4 = Physics.OverlapSphere( transform.position, blastRadius );
			foreach (Collider enemy in urDone4)
			{
				if( enemy && enemy.rigidbody ) 
				{
					enemy.rigidbody.AddExplosionForce(explosionForce, transform.position, blastRadius, 5.0f );
					PlayerWeaponController enemyHealth = enemy.GetComponentInParent <PlayerWeaponController> ();
					if(enemyHealth != null)
					{
						enemyHealth.TakeDamage ((int)(explosionForce*.05), Vector3.zero);	
					}
				}
				
			}
			Instantiate( explosion, transform.position, transform.rotation);
			gameObject.SetActive(false);	
			Destroy(gameObject);
		}
	}
}
