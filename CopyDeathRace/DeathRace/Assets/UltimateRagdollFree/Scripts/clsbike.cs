using UnityEngine;
using System.Collections;
using clsurgutils = U_r_g_utils.clsurgutils;

/// <summary>
/// 2013-04-23
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// Extended class to drive the demo scene bike
/// NOTE: the dismemberation flow starts with collision detection
/// </summary>
public class clsbike : MonoBehaviour {
	/// <summary>
	/// maximum torque
	/// </summary>
	public float vargammotormax = 500;
	/// <summary>
	/// simple car with a single shift, will accelerate with maximum allowed torque and reach this value, to stop accelerating further
	/// </summary>
	public float vargamspeedmax = 15;
	/// <summary>
	/// maximum antitorque
	/// </summary>
	public float vargambrakemax = 150;
	/// <summary>
	/// car main rigidbody reference
	/// </summary>
	public Rigidbody vargamchassisbody = null;

	private int varwheelscount = 0;
	private WheelCollider[] varwheels;

	private const float cnsspring = 2500;
	private const float cnsdamper = 200;
	private const float cnssuspension = 0.5f;

	void Start () {
		if (vargamchassisbody == null) {
			Debug.LogError("The bike needs a rigidbody to function.");
			enabled = false;
		}
		varwheels = GetComponentsInChildren<WheelCollider>();
		varwheelscount = varwheels.Length;
		for (int varwheelcounter = 0; varwheelcounter < varwheelscount; varwheelcounter++) {
			JointSpring varspring = new JointSpring();
			varspring.spring = cnsspring;
			varspring.damper = cnsdamper;
			varspring.targetPosition = 0;
			varwheels[varwheelcounter].suspensionSpring = varspring;
			varwheels[varwheelcounter].suspensionDistance = cnssuspension;
			
			//full throttle
			varpower = 1;
			varbrake = 0;
		}
		vargamchassisbody.centerOfMass = new Vector3(0, -0.05f, 0);
		varspeedmax = vargamspeedmax * vargamspeedmax;
		varD = transform.root.GetComponentInChildren<clsdismemberator>();
		if (varD != null) {
			varbones = varD.vargamskinnedmeshrenderer.bones;
		}
	}

	private float varpower, varsteering, varbrake, varspeedmax;
	private bool varpassenger = true;
	private Transform vargampassenger = null;
	void FixedUpdate() {
		Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
		if (varpassenger) {
			if (!Physics.Raycast(transform.position, Vector3.down,1.0f)) {
				vargampassenger = transform.Find("Lerpz_kinematic");
				if (vargampassenger != null) {
					Invoke("metfalling",0.3f);
				}
				varpassenger = false;
			}
		}

		//toggle pedal when maximum speed is reached
		if (vargamchassisbody.velocity.sqrMagnitude > varspeedmax) {
			varpower = 0;
		}

		if (varwheels[0] != null) {
			varwheels[0].motorTorque = vargammotormax * varpower;
			varwheels[0].brakeTorque = vargambrakemax * varbrake;
		}
		if (varwheels[1] != null) {
			varwheels[1].motorTorque = vargammotormax * varpower;
			varwheels[1].brakeTorque = vargambrakemax * varbrake;
		}
		
	}
	
	private clsdismemberator varD;
	private Transform[] varbones;
	void OnTriggerEnter(Collider varpother) {
		//crash!
		if (varpother.tag == "terrain") {
			collider.enabled = false;

			if (varD != null) {
				//determine the number of parts to break, based on our speed on trigger
				int varparts = varD.vargamboneindexes.Count;
				float varspeedratio = vargamchassisbody.velocity.sqrMagnitude / (vargamspeedmax * vargamspeedmax);
				int varpartstobreak =  (int)(varparts * varspeedratio);
				int varbrokenparts = 0;
				for (int varbreakcounter = 0; varbreakcounter < varbones.Length; varbreakcounter++) {
					float varbreakchance = Random.Range(0, 0.99f);
					if (varbreakchance > (1- varspeedratio) && varbones[varbreakcounter].collider != null) {
						clsurgutils.metdismember(varbones[varbreakcounter],null,varD);
						varbrokenparts++;
					} 
					if (varbrokenparts > varpartstobreak) {
						break;
					}
				}
			}
			
		}
	}
	
	private void metfalling() {
		vargampassenger.parent = null;
		clsurgutils.metgotangible(vargampassenger, true);
		clsurgutils.metgodriven(vargampassenger, rigidbody.velocity);
	}
}