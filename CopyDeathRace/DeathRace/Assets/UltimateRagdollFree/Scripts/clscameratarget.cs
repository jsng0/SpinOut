using UnityEngine;
using System.Collections;

/// <summary>
/// 2013-05-14
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// advanced class for camera scene management
/// 
/// this class is used as a scene manager and an initializer for the "Fixed timestep" parameter of the
/// EDIT=> PROJECT SETTINGS=> TIME menu, that determines the frequency of physic updates for the game
/// It is suggested that the value be kept consistent throughout development to avoid
/// unexpected physic behavior
/// </summary>
public class clscameratarget : MonoBehaviour {
	/// <summary>
	/// inspector slot for a target
	/// </summary>
	public Transform vargamtarget = null;
	//V4: removed after introduction of scene manager
	//private Vector3 varpositionfixer = new Vector3();
	//private Vector3 varrotationfixer = new Vector3();
	public clscameratargetdata[] vargamscenarios;
	/// <summary>
	/// tracking speed for the camera, roughly in radians per second
	/// </summary>
	public float vargamtrackingspeed = 0.3f;
	
	public int vargamcurrentscenario = -2;
	private const int cnsbuttonwidthmenu = 250;
	private const int cnsbuttonwidth = 100;
	private const string cnsdemostagename = "__URG_Demo";
	private const string cnsdemoresetname = "__URG_Empty_scene";
	private const string cnssoldier = "Soldier";
	private bool vardisplaymoreinfo = false;
	
	private GameObject varsoldier;
	void Awake() {
		//set the Fixed timestep to 100 calls
		Time.fixedDeltaTime = 0.01f;
		//set the minimum collision detection
		Physics.minPenetrationForPenalty = 0.01f;
		//set the collision matrix to comply with actor controller and missiles
		Physics.IgnoreLayerCollision(2,0);
		//make sure that rotations can reach a considerable velocity
		Physics.maxAngularVelocity = 20;
		
		metwatchscenario(0);
		//freeze time
		Time.timeScale = 0;
	}

	private void metwatchscenario(int varpscenario) {
		vargamtarget = null;
		vargamcurrentscenario = varpscenario;
		Time.timeScale = 1;
		if (vargamscenarios[varpscenario] == null || vargamscenarios[varpscenario].propviewport == null) {
			Debug.LogError("Scenario [" + varpscenario + "] is null. Please assign a scenario to the manager to proceed.");
			return;
		}
		transform.position = vargamscenarios[varpscenario].propviewport.position;
		transform.rotation = vargamscenarios[varpscenario].propviewport.rotation;
		varsmoothtargetposition = transform.position + transform.forward;
		if (vargamscenarios[varpscenario].proptarget != null) {
			vargamtarget = vargamscenarios[varpscenario].proptarget;
		}
		vardisplaymoreinfo = false;
	}
	
	private void metresetlevel() {
		Application.LoadLevel(cnsdemostagename);
	}
	
	void OnGUI() {
		GUI.skin.box.alignment = TextAnchor.UpperLeft;
		switch(vargamcurrentscenario) {
			case 1:
				GUILayout.Box(@"Simple ragdoll functionality.
The soldier game model was ragdolled into a prefab in editor mode. At the press of the 'Go ragdoll' button
below, the soldier will run until it starts to fall, which causes the ragdolled prefab to be spawned and
posed like the original gameobject, which is destroyed.
This approach uses two different game objects for game character and ragdoll, allowing easy differentiation
between the two. This way, the ragdoll can be spawned without controller nor other important scripts that
are instead needed on the character.
Free version instructions:
- In Edit mode, Start URG with Gameobject menu=> Create Other=> Ultimate ragdoll free
- Drag the desired game character into the source (topmost) slot
- Press create ragdoll
- Manually add colliders to the ragdoll parts");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- The soldier_ragdollified_controller gameobject moves thanks to the clsactorcontroller component, which
	uses Physics.Raycast to determine if it's falling beyond its allowed limit.
- If the soldier is falling, the gameobject uses the clsragdollhelperfree component to instantiate its
	ragdoll, with the Transform varragdoll = varhelper.metgoragdoll instruction. This instruction in turn
	calls the clsragdollifyfree component, which hosts the actual ragdoll prefab to instance.
- The metgoragdoll function poses the spawned ragdoll like its parent, adds velocity to the bodyparts
	to preserve the original momentum, and destroys the host.
- The weapon unparents when the ragdoll spawns, thanks to the clsdrop component.");
					GUILayout.Box(@"Setup information:
- Dragged the soldier model from the project into the scene, and created a non kinematic ragdoll for it
	using URG Developer edition.
- Added rigidbody, box collider and clsdrop components to the gun of the soldier (m4mb gameobject)
- Created a prefab (Soldier_ragdoll). Deleted the scene gameobject, and dragged the soldier model into
	the scene again, to add charactercontroller, clsragdollhelperfree, clsragdollifyfree and
	clsactorcontroller components to it. In particular, dragged the Soldier_ragdoll prefab into the
	clsragdollifyfree component slot.
- Named Solider_ragdollified_controller the soldier gameobject, and proceeded with the implementation
	above.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			case 2:
				GUILayout.Box(@"Basic ragdoll functionality.
The character was ragdolled in edit mode, with the 'Kinematic' checkbox flagged in URG! options.
Additionally, a trigger script was added to it. When the desired event occurs (in this case a simple time
wait), the collider script turns the character rigidbody components from 'Kinematic' to 'Physic driven',
and the character turns into a ragdoll.
This method requires more work than ragdoll instancing, but is necessary when ragdoll to animation transition
is needed afterwards.
Since game character and ragdoll are the same, this method is normally used when the two character states
share most of the logic. For example, if the ragdoll differs from the character because of the controller
alone, deactivating it via script is more maintenable than creating a separated ragdoll without it.
Free version instructions:
- Start URG with Gameobject menu=> Create Other=> Ultimate ragdoll free
- Drag the desired game character into the source (topmost) slot
- Expand options and make sure the 'Kinematic ragdoll' option is checked
- Press create ragdoll
- Manually add colliders to the ragdoll parts");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
The character simply transitions from its normal, game state into a ragdoll state, by means of a script.
- the clsshowcasehelper script receives its activation message from the showcase manager.
- after a little wait, animations are stopped, since otherwise the rigidbodies don't become physic.
- the clsurgutils.metgodriven call is made and the character is preserved, but becomes a ragdoll.");
					GUILayout.Box(@"Setup information:
- The character was simply dragged into the scene, and ragdolled with URG Developer version, with the
	'Kinematic' parameter checked.
- The clsshowcasehelper script was added to the Lerpz_kinematic gameobject, case 2 was added to the
	showcase manager clscameratarget, and case 5 was added to the clsshowcasehelper script.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			case 3:
				GUILayout.Box(@"Advanced ragdoll (Not available in free version):
The character was ragdolled in edit mode, with the 'URGent' checkbox flagged in URG! options.
Then, thanks to the urgent actuators and the urgent state manager, the collision event generated from the
weight that hits the character, issues a call that turns all its rigidbodies into physic driven in a single
line of code.
The URGent manager offers a convenient approach to manage ragdoll cases and implement different states based
on character data. For example, the URGent manager can greatly simplify ragdoll behavior when any ragdolled
character is killed, or needs be dismembered, or needs to transition from ragdoll back to animation, all
within of a single control script, that can serve any number of ragdoll templates.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- Thanks to the URGent actuators, all the ragdoll logic of this example resides in the clsurgentactuator class.
- The scripts uses the vargamcasemanager value of the vargamurgentsource attribute to determine which 'kind' of
	ragdoll is acting, and responds consequently thanks to a simple Switch command on the OnCollisionEnter event.
- In this particular case, actuators are used to call clsurgutils.metdriveurgent and transition the character
	into a ragdoll, but in more elaborate cases the user can choose to decrease character bodypart hitpoints,
	dismember, break individual limbs, etc.
- Lastly, the weapon is detached with a simple GetComponent call for clsdrop.");
					GUILayout.Box(@"Setup information:
- The soldier character was instanced and t-posed, and ragdolled with URG Developer with the 'Add URG scripts'
	and 'Kinematic ragdoll' options enabled.
- Rigidbody, Collider and clsdrop components are added to the soldier weapon gameobject. The rigidbody is setup
	as Kinematic.
- The soldier was positioned and posed for the scene.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
		case 4:
				GUILayout.Box(@"Advanced ragdoll to animation (Not available in free version):
This feature is very sought after, since a ragdoll can become animated again, but there's no way to create a fluid,
animation like transition without it.
Any standard or URGent ragdoll can receive the URG Animation states class. In edit mode, the ASM compiler creates
data structures that are used in play mode, to create ragdoll to animation or animation to animation transitions.
In this example, there's a simple script that checks if the character has fallen, and transitions to one 'rise'
animation, which is consequently played.
The transition call is extremely fast and requires only a line of code, and the quality of the final effect is just
limited by the quality and number of the landing animations (i.e. one when the character has fallen on its back,
or on its belly or side, etc.)");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- This functionality is very flexible and can be used to also perform animation to animation transitions,
	but in this particular case, it allows the ragdoll to play as an animated character again, after becoming
	a ragdoll.
- The character is setup to play the 'balance' animation automatically, so the clsshowcasehelper is used
	to simply time the call to the 'metAsm' function, which wraps the 'metAsmRoutine' function in a loop.
- Inside the 'metAsmRoutine' the animations are stopped, the character becomes ragdoll, and finally the
	transition is made with a single line call to metcrossfadetransitionanimation, which automatically takes
	care of restoring the animation ability of the ragdoll.");
					GUILayout.Box(@"Setup information:
- The character was dragged t-posed into the scene, and ragdolled as kinematic with URG Developer edition.
- The URG animation states was then used, dragging the character into its slot and clicking the 'Memorize'
	button to State all animations.
- The character was posed, and the clsshowcasehelper script was added to it with switch 6");
				}
				break;
			case 5:
				GUILayout.Box(@"Advanced dismemberable character (Not available in free version):
	The 'Big D' is a next-gen utility and URG exclusive that allows separation of any 'Transform',
along with its mesh triangles, from the main gameobject.
The compiled class is installed in edit mode with the Dismemberator utility
and is used afterwards in a call with a single line of code.
Separation can optionally instance cut triangles with an user defined
material, and parent and child separation gameobjects (for example particles) if so desired.
Additionally, this feature doesn't affect animations, so it becomes possible
to keep an animation running, and seamlessly detach any gameobject part.
NOTE: this feature is CPU intensive. To flawlessly perform multiple single frame cuts we recommend
choosing or producing an optimized 3d model (Consult Unity guidelines for more information regarding
number of bones and triangles in optimal game characters)");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- The scenario manager (this script), sends an activate message to the /__Scenery/_Bombspawn gameobject.
- The bombspawn gameobject activates thanks to the clsdismemberatorhelper script, spawns an explosion and
	looks for the /Zombie_D gameobject.
- The Zombie_D parts are processed by a random selection, to determine which part is going to be cut.
- Parts to cut receive the 'Transform varcurrentcut = clsurgutils.metdismemberpart' command, followed by a
	force push
- IMPORTANT: please keep the clsdismemberator component FOLDED in the inspector, to work around an unity bug
	that causes a slowdown with a skinned mesh renderer in a public script slot.");
					GUILayout.Box(@"Setup information:
- Dragged the zombie model from the project into the scene.
- Duplicated (CTRL D) the idle animation from the project into the /_models/_animations folder, and assigned
	it to the instanced zombie (this is a necessary step to allow animation after separation).
- Ran URG Developer edition on the model and created a kinematic ragdoll
- Ran URG Dismemberator on the model, which added the clsdismemberator component to the zombie.
	NOTE: please keep the clsdismemberator component FOLDED, otherwise it'll slow down the scene camera, due
		  to a bug with Unity's inspector.
- Created a prefab for the zombie, and proceeded with the implementation above.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			case 6:
				GUILayout.Box(@"Linear ragdoll for partial characters:
With certain unusual ragdolls (multiple limbs like spiders, or linear like ropes, fences, chains, etc.), the normal
ragdolling procedure might not complete properly. Thanks to the 'Fake Limbs' and 'Connect' URG! functions
it becomes possible to create separate linear ragdolls, and connect them with a single click, to make the
final gameobject perform as close to a 'full' ragdoll as possible.
Free version instructions:
- Create a ragdoll as per previous instructions
- Locate the non ragdolled part (for example a tail) by exploring the ragdoll parts
- Use the 3d editor to rotate the limb and point it upwards
- Drag the limb into URG source slot and press the 'Create ragdoll' button
    This will assign a 'spine' behavior to the part.
- Drag the parent part of the recently ragdolled part (for example the back) into the
    connection slot
- Press the 'Connect part' button. Repeat until all parts are ragdolled.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- Linear ragdolls can be any type of ragdoll (Instanced, Kinematic, URGent, etc.) so they can be created with
	any design in mind.
- The difference from normal ragdolls is that the user will use the 'Fake Limbs' feature to provide URG with 
	delimiters, that will shape the Colliders of the 'spine' of the ragdoll to be.
- Thanks to this behavior, it is just enough to select a different source object each time, and all 'extra'
	limbs can be ragdolled as a Spine, to eventually connect all of the ragdolled parts together, with the
	'Connect ragdoll' feature.");
					GUILayout.Box(@"Setup information:
- Tree, Rope and Weight models are dragged into the scene. Tree and Rope are created using the 'Fake Limbs'
	feature. Since their bones are linear, a 'Spine' setup is created for their ragdolling process.
- In particular, the Tree was created with an uniform flexibility of 1.6, while the Rope with an u.f. of 4.5
- The weight received a Collider and Rigidbody, and the three objects were setup in their Intended positions,
	in Cinematic mode (Kinematic flag unchecked on all rigidbodies)
- Using the 'Connect ragdoll' feature, the Weight was connected to the Rope, and then the Rope was connected
	to the Tree. Note that actual 3d world position is important during connection, since the Joints will pivot
	around their creation or connection center.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			case 7:
				GUILayout.Box(@"Dismember for arbitrary game models (Not available in free version):
Dismemberator functionality is handily available for any sort of SkinnedMeshRendered gameobject.
Tanks to the 'Big D', as long as an object has 3D bones and is properly skinned,it will be possible to
dynamically detach any of its parts in realtime.
In this example, the bike hosts the Big D class, and when it impacts the ground with a simple sphere trigger,
a basic routine iterates through the bike parts to disconnect them, issuing a single instruction call to the
dismemberator utility.
The main power of this feature is that the target gameobject can just be a single mesh, designed and rigged
part by part, so it's perfect to disassemble objects that are subject to animations (turrets, furniture, coffers,
vehicles, etc.).");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- The fulcrum of arbitrary separation is that for each separation target bone, there's a closed loop in the mesh,
	for example the bike wheels, or a turret cannon, or a table leg.
- The power of arbitrary separation is that there's no triangulation involved. As long as the 3D artist creates a
	bone for the particular game model component, Big D will be able to detach that bone, and all the triangles
	skinned to it.
- Functionality is identical to standard dismember, so separation can instance child and parent special effects,
	and compilation is the same as for regular skinned gameobjects.
- Mayhem potential of this feature is limitless. Even if an object is not animated, as long as its rigged and
	skinned, it can be separated part by part. Disassembling objects apart has suddenly become easy, with just basic
	3D gamemodel design.");
					GUILayout.Box(@"Setup information:
- The bike was rigged in Blender in a matter of minutes, assigning bones to each of the parts that we wanted to
	disassemble. These parts are closed loops, so that there's no need for triangulation when they are separated.
- Once imported, Big D was run on the model, and the clsbike and the clsshowcase helper scripts were added to assign
	the logic.
- Physical objects were added to the bike: a kinematic rigidbody and a collider for all bones, and a main collider
	and a non kinematic rigidbody for the bike root.
- The Lerpz gamemodel was ragdolled, and made non kinematic and non tangible with the Kinetifier utility (not included
	in the free version), and then posed and parented to the bike.
- When the scene runs, the bike uses wheel colliders to propel itself, and when its sphere trigger collides with the
	ground, it iterates through its bones and separates each part. Additionally, it detects the ground below with a
	raycast, and calls the metfalling routine to detach the passenger.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			case 8:
				GUILayout.Box(@"Animated game character part break (Not available in free version):
This is one often sought after feature, which can be easily achieved thanks to the URGent classes and manager. The
ragdoll is created by adding the URG entity scripts, and whenever needed, a simple function call is made and desired
limbs become fully physical, ignoring animation curves.
NOTE: this behavior relies on the 'Animate physics' feature of the skinned mesh renderer, and thus does not work with
Mecanim rigged characters automatically.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					metresetlevel();
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- Limbified ragdolls are URGent compiled ragdolls with the 'Animate physics' flag checked in their Skinned Mesh
	Renderer component.
- Part breaking is based on kinematic functionality of individual bodyparts, which are transformed thanks to the direct
	access allowed by the URGent class.
- A broken part can be repaired by a later call to the breaking routine, with a different parameter.
- This feature can also be used to simulate animation states. For example an animated flower can be broken, and it will
	fall as if dead when limbified. To later bring the flower back to animation. Transition is instantaneous, but result
	can be satisfying in many cases, and without the extra work involved in ASM preparation.");
					GUILayout.Box(@"Setup information:
- The 'Soldier_Urgent_controller' character was ragdolled normally, but checking 'Add URG entity scripts', and adding a
	controller and the clsactorcontroller to it afterwards. The '_Tooncannon' model was prepared with the specific
	clscannon script, and in particular the 'cannonball' gameobject was prefabbed with the 'missile' tag.
- The soldier character parts are using the 'stock' clsurgentactuator class, that simply runs the 'OnCollisionEnter'
	event, monitoring the '-3' case from a 'missile' tagged gameobject.
- When the cannonball hits any clsurgentactuator host, the collision event is triggered, and the part is driven with a
	call to the metdrivebodypart function.");
					if (vardisplaymoreinfo) {
						if (GUILayout.Button("Less Information",GUILayout.Width(cnsbuttonwidthmenu))) {
							vardisplaymoreinfo = false;
						}
					}
				}
				break;
			default:
				GUILayout.Box(@"Good day, and welcome to URG! demo scene and feature showcase.
Thanks for your interest in the U.R.G. Please press the buttons for a demonstration of the available functions.
For usage instructions, please see the reference file.");
				GUILayout.Box(@"Free version limitations:
- Only an emergency collider is created for each ragdoll. Physics elements (joints, rigidbodies, etc.) are fully instanced.
   > Complete versions create shaped, per bodypart colliders. This limitation can be overcome by manually creating colliders.
- Flexibility parameters are fixed, set for a cartoony effect.
   > Complete versions allow joint flexibility finetuning for ragdolls to perform realistically or like cloth, wood and anything
	    in between.
- Urgent Manager, Dismemberator and ASM compilers are not included, but there are scenarios featuring these utilities.
   > Urgent Manager is a centralized ragdoll event hub that greatly simplifies ragdoll project management.
   > Dismemberator is an utility for real time bone separation, wich can dynamically create uv textured cut patches, and allow
	    instantiation of cut gameobjects (particles, meshes, etc.).
   > ASM is an advanced utility that allows transition from ragdoll to animation, creating transition animations in real time.");
				GUILayout.Space(10);
				if (GUILayout.Button("Simple use: ragdoll prefab", GUILayout.Width(cnsbuttonwidthmenu))) {
					if (varsoldier == null) {
						varsoldier = GameObject.Find(cnssoldier);
					}
					metwatchscenario(1);
				}
				if (GUILayout.Button("Basic use: kinematic ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(2);
					GameObject.Find("/Lerpz_kinematic").SendMessage("metactivate");
				}
				if (GUILayout.Button("Advanced use: URGent ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(3);
				}
				if (GUILayout.Button("Advanced use: Animation States", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(4);
					GameObject.Find("/Alien_ASM").SendMessage("metactivate");
				}
				if (GUILayout.Button("Advanced use: Dismemberator", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(5);
					GameObject.Find("/__Scenery/_Bombspawn").SendMessage("metactivate");
				}
				if (GUILayout.Button("Special use: Linear ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(6);
				}
				if (GUILayout.Button("Special use: Arbitrary separation", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(7);
				}
				if (GUILayout.Button("Special use: Limb break", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(8);
					GameObject.Find("/__Scenery/_Tooncannon").SendMessage("metactivate");
					GameObject.Find("/Soldier_Urgent_controller").SendMessage("metactivate");
				}
				if (!vardisplaymoreinfo) {
					if (GUILayout.Button("More Information",GUILayout.Width(cnsbuttonwidthmenu))) {
						vardisplaymoreinfo = true;
					}
				}
				if (vardisplaymoreinfo == true) {
					GUILayout.Box(@"Implementation information:
- Demo scene requires at least a 1024x768 viewport.");
					GUILayout.Box(@"Setup information:
- This script is hosted in the Camera_Menu_Timestepmanager gameobject.");
				}
				break;
		}
	}
	
	private Vector3 varcurrenttargetposition;
	private Vector3 varsmoothtargetposition;
	void LateUpdate () {
		if (vargamtarget != null) {
			varcurrenttargetposition = vargamtarget.position;
			varsmoothtargetposition = Vector3.Lerp(varsmoothtargetposition, varcurrenttargetposition, Time.deltaTime * vargamtrackingspeed);
			transform.LookAt(varsmoothtargetposition);
		}
	}
}

[System.Serializable]
/// <summary>
/// viewport target data class
/// </summary>
public class clscameratargetdata {
	/// <summary>
	/// inspector 3d scene positions for camera snapshotting function
	/// </summary>
	public Transform propviewport;
	/// <summary>
	/// list of targets to combine with the scenarios array
	/// </summary>
	public Transform proptarget;
}