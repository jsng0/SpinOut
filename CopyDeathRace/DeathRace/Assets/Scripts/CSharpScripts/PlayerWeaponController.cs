using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class PlayerWeaponController : MonoBehaviour {
	RawImage ammoPic;
	RawImage rocketPic;
	RawImage shieldPic;
	RawImage laserPic;
	RawImage boosterPic;
	RawImage tntPic;
	//VolumeManager //sounds;
	
	public GameObject rocket;
	public GameObject tntBomb;
	public GameObject explosion;
	public Texture2D crosshair;
	public int startingHealth = 100;
	Slider healthSlider;
	public int currentHealth;
	
	
	GameObject booster1;
	GameObject booster2;	
	Camera myCam;
	
	Text weaponCounter;
	Text defenseCounter;
	Crosshair target;
	//Using int for efficiency, much faster than string comparison every frame
	//itemHeld[0] -> Weaponheld      itemHeld[1] -> Defenseheld
	//0 for no item, 1->machinegun/healthpack, 2->laser/shield, 3->rocketlauncher/gravityfield
	int[] itemsHeld = { 0, 0 };
	int sizeX = 32;
	int sizeY = 32; 
	int xDraw = 0;
	int yDraw = 0;
	
	bool fireWeapon;
	bool fireDefense;
	
	public int playerNumber = 1;
	YellowCar myCar; 
	
	int numWeapon = 500;
	int numDefense = 500;
	float range = 50f;
	float effectsDisplayTime = 0.2f;
	int damagePerShot = 8;
	public float timeBetweenBullets = 0.15f;
	public float timeBetweenLasers = 0.05f;
	float timeBetweenBoosts = .05f;
	
	
	
	float timer;	//For weapon timing
	float timer2;	//For the defense timing not to interfere with weapons
	Ray shootRay;
	RaycastHit shootHit;
	int shootableMask;
	public ParticleSystem gunParticles;
	LineRenderer MgunLine;

	Light MgunLight;
	

	void Awake ()
	{
		//sounds = GameObject.Find("Music Manager").GetComponent<VolumeManager>();
		Screen.showCursor = false;

		
		myCar = gameObject.GetComponent<YellowCar>();
		//target = (GameObject.FindGameObjectWithTag("Aiming")).GetComponent<Crosshair>();
		Random.seed = (int)(Time.deltaTime*10000)%50;		
		xDraw = Screen.width/2;
		
		if( playerNumber == 1 )
		{
			weaponCounter = GameObject.Find("WeaponCounter").GetComponent<Text>();
			defenseCounter = GameObject.Find("DefenseCounter").GetComponent<Text>();
			ammoPic = GameObject.Find("AmmoPicture").GetComponent<RawImage>();
			shieldPic = GameObject.Find("HealthPicture").GetComponent<RawImage>();
			laserPic = GameObject.Find("LaserPicture").GetComponent<RawImage>();
			rocketPic = GameObject.Find("RocketPicture").GetComponent<RawImage>();
			boosterPic = GameObject.Find("BoosterPicture").GetComponent<RawImage>();
			tntPic = GameObject.Find("TNTPicture").GetComponent<RawImage>();
			healthSlider = GameObject.Find("UserHealthSlider").GetComponent<Slider>();
			
			weaponCounter.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
			defenseCounter.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
			ammoPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
			shieldPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
			laserPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
			rocketPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f); 
			boosterPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f); 
			tntPic.transform.localPosition += new Vector3(0.0f, Screen.height/2, 0.0f);
	
			yDraw = Screen.height*3/4;
			myCam = GameObject.Find("PlayerOneCamera").GetComponent<Camera>();
			booster1 = transform.FindChild("RocketFire11").gameObject;
			booster2 = transform.FindChild("RocketFire12").gameObject;
		}
		else 
		{
			weaponCounter = GameObject.Find("WeaponCounter2").GetComponent<Text>();
			defenseCounter = GameObject.Find("DefenseCounter2").GetComponent<Text>();
			ammoPic = GameObject.Find("AmmoPicture2").GetComponent<RawImage>();
			shieldPic = GameObject.Find("HealthPicture2").GetComponent<RawImage>();
			laserPic = GameObject.Find("LaserPicture2").GetComponent<RawImage>();
			rocketPic = GameObject.Find("RocketPicture2").GetComponent<RawImage>();
			boosterPic = GameObject.Find("BoosterPicture2").GetComponent<RawImage>();
			tntPic = GameObject.Find("TNTPicture2").GetComponent<RawImage>();
			healthSlider = GameObject.Find("UserHealthSlider2").GetComponent<Slider>();
			
			healthSlider.transform.localPosition -= new Vector3(0.0f, Screen.height/2, 0.0f);
			yDraw = Screen.height/4;
			myCam = GameObject.Find("PlayerTwoCamera").GetComponent<Camera>();
			booster1 = transform.FindChild("RocketFire21").gameObject;
			booster2 = transform.FindChild("RocketFire22").gameObject;
		}
		
		//Grab and disable boosters
		//booster1 = GameObject.Find("RocketFire03");
		//booster2 = GameObject.Find("RocketFire02");

		
		
		booster1.SetActive(false);
		booster2.SetActive(false);
		
		ammoPic.gameObject.SetActive(false);
		shieldPic.gameObject.SetActive(false);
		boosterPic.gameObject.SetActive(false);
		laserPic.gameObject.SetActive(false);
		rocketPic.gameObject.SetActive(false);
		tntPic.gameObject.SetActive(false);
		weaponCounter.gameObject.SetActive(false);
		defenseCounter.gameObject.SetActive(false);
		
		shootableMask = LayerMask.GetMask ("Player");
		MgunLine = GetComponent <LineRenderer> ();

		MgunLight = GetComponent<Light> ();
		healthSlider.minValue = 0;
		healthSlider.maxValue = 100;
		currentHealth = startingHealth;
	}
	

	

	
	
	void Update ()
	{
		var inputDevice = (InputManager.Devices.Count > playerNumber) ? InputManager.Devices[playerNumber] : null;
		var inputDevice2 = inputDevice;
		if (inputDevice == null)
		{
			return;
		}
		else
		{
			if( inputDevice.Action1 ) {		/*AndyRespawn ar = gameObject.GetComponent<AndyRespawn>();
											ar.respawn();	*/}//A
			if( inputDevice.Action2 ) {			}//B
			if( inputDevice.Action3 ) {			}//X
			if( inputDevice.Action4 ) {			}//Y
			
			if( inputDevice.RightBumper ) {	myCar.set_controller_speed(false, true );}
			else if( inputDevice.RightTrigger.Value > .5 ) {myCar.set_controller_speed(true, false );}
			else { myCar.set_controller_speed(false, false ); }
			
			fireWeapon = inputDevice2.LeftTrigger.IsPressed;
			fireDefense = inputDevice2.LeftBumper.IsPressed;
			
			if( inputDevice.Direction.X > .1f ) {myCar.set_controller_turn( false, true);}
			else if( inputDevice.Direction.X < -.1f ) {myCar.set_controller_turn( true, false);}
			else {myCar.set_controller_turn( false, false);}
			
			if( inputDevice.RightStickX.Value > .1 ) {xDraw+=3;}
			else if( inputDevice.RightStickX.Value < -.1 ) {xDraw-=3;}
			
			if( inputDevice.RightStickY.Value > .1 ) {yDraw+=2;}
			else if( inputDevice.RightStickY.Value < -.1 ) {yDraw-=2;}

		}
		AimCheck(playerNumber);
		timer += Time.deltaTime;
		timer2 += Time.deltaTime;
		//fireWeapon = Input.GetButton ("Fire1");
		//fireDefense = Input.GetButton ("Fire2");
		//myCar.set_controller_speed( );
		//myCar.set_controller_turn( );
		//print ( (Input.GetAxis(p_LAX) > 0.1f) );
		
		if( itemsHeld[0] != 0 || itemsHeld[1] != 0 )
		{
			if( fireWeapon && timer >= timeBetweenBullets*3 && itemsHeld[0] == 3 )
				ShootLauncher ();
			else if( fireWeapon && timer >= timeBetweenLasers && itemsHeld[0] == 2 )
				ShootLaser ();
			else if( fireWeapon && timer >= timeBetweenBullets 
			   && Time.timeScale != 0 && itemsHeld[0] == 1)
				ShootMgun ();
			if( fireDefense && itemsHeld[1] == 1 )
				UseHPItem();	
			else if( fireDefense && itemsHeld[1] == 2 && timer2 >= timeBetweenBoosts )
				UseBooster();	
			else if( fireDefense && itemsHeld[1] == 3  )
				UseTNT();	
			
		}
		if(timer >= timeBetweenBullets * effectsDisplayTime && ( itemsHeld[0] == 1 || itemsHeld[0] == 0) ){	//Disable to toggle line to look like gunfire
			MgunLine.enabled = false;	
			MgunLight.enabled = false;
		}
		if(timer >= timeBetweenLasers * effectsDisplayTime && ( itemsHeld[0] == 2 || itemsHeld[0] == 0) ) { 	//Disable to toggle line to look like gunfire {
			MgunLine.enabled = false;	
			MgunLight.enabled = false;
		}
		if(timer2 >= timeBetweenBoosts*5 ) {
			booster1.SetActive(false);
			booster2.SetActive(false);
		}
		//fireDefense = false;
		//fireWeapon = false;
	}
	
	void AimCheck( int pnum )
	{
		
		if ( xDraw < 0 ) xDraw = 0;
		if ( xDraw > Screen.width ) xDraw = Screen.width;
		if( pnum == 1 )
		{
			if ( yDraw < Screen.height/2 ) yDraw = Screen.height/2;
			if ( yDraw > Screen.height ) yDraw = Screen.height;
		}
		else 
		{
			if ( yDraw < 0 ) yDraw = 0;
			if ( yDraw > Screen.height/2 ) yDraw = Screen.height/2;
		}
	}
	
	void UseHPItem ()
	{
		numDefense = 0;
		defenseCounter.text = numDefense.ToString();
		
		PlayerHealth playerHealth = gameObject.GetComponentInParent<PlayerHealth>();
		playerHealth.HealCar(20);
		itemsHeld[1] = 0;
		shieldPic.gameObject.SetActive(false);
		defenseCounter.gameObject.SetActive(false);
	}
	
	Vector3 getFireDir() 
	{	
		LayerMask levelMask = LayerMask.GetMask("Grass");
		LayerMask roadMask = LayerMask.GetMask("Road");
		LayerMask playerMask = LayerMask.GetMask("Player");
		
		Ray camRay = myCam.ScreenPointToRay (new Vector3(xDraw,yDraw,0.0f));//target.getAim(playerNumber));
		RaycastHit levelHit, carHit, roadHit;
		Vector3 fireDir;
		if(Physics.Raycast (camRay, out carHit, 100f, playerMask))
		{
			return carHit.point;
		}
		if(Physics.Raycast (camRay, out levelHit, 100f, levelMask))
		{
			return levelHit.point;
		}		
		if(Physics.Raycast (camRay, out roadHit, 100f, roadMask))
		{
			return roadHit.point;
		}
		Ray mouseXY = myCam.ScreenPointToRay (new Vector3(xDraw,yDraw,0.0f));
		return GetGunOrigin() + mouseXY.direction*100;
	}
	
	void UseBooster ()
	{
		if ( DecAndCheck('d') ) return;
		
		defenseCounter.text = numDefense.ToString();
		booster1.SetActive(true);
		booster2.SetActive(true);
		myCar.speed_boost();
	}
	
	
	void ShootMgun ()
	{
		//sounds.PlayWeaponAudio("gun");
		if( DecAndCheck('w') ) return;
		MgunLight.enabled = true; //Enable line rendered, looks like bullet fire path

		MgunLine.enabled = true;
		
		shootRay.origin = transform.position + Vector3.up/2;		//Set origin to be from where gun fires
		shootRay.direction = getFireDir()-shootRay.origin;//transform.forward;		//Away from the user car in Z direction
		
		MgunLine.SetPosition (0, GetGunOrigin()); 
		
		if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
		{
			PlayerWeaponController enemyHealth = shootHit.collider.GetComponentInParent <PlayerWeaponController> ();
			if(enemyHealth != null)
			{
				if( enemyHealth.playerNumber != playerNumber )
					enemyHealth.TakeDamage (damagePerShot, shootHit.point);
			}
			MgunLine.SetPosition (1, getFireDir());
		}
		else
		{
			MgunLine.SetPosition (1, getFireDir());
		}
	}
	
	void ShootLauncher () 
	{
		//sounds.PlayWeaponAudio("rocket");
		if ( DecAndCheck('w') ) return;
		Vector3 fire = getFireDir();
		Vector3 firedir = fire - GetGunOrigin();	
		Quaternion rotate = transform.rotation;
		rotate.SetLookRotation(firedir,Vector3.up);
		Instantiate (rocket, GetGunOrigin()+Vector3.up*2, rotate);
	}
	
	Vector3 GetGunOrigin() 
	{
		return transform.position + Vector3.up*2 + Vector3.forward;
	}
	

	
	void UseTNT()
	{		
		numDefense = 0;
		defenseCounter.text = numDefense.ToString();
		itemsHeld[1] = 0;
		
		Vector3 tntPos = transform.position - transform.forward*5;
		RotateVectorY( tntPos, transform.rotation.eulerAngles.y ); 
		Instantiate( tntBomb, tntPos, transform.rotation);
		
		
		tntPic.gameObject.SetActive(false);
		defenseCounter.gameObject.SetActive(false);
	}
	
	void ShootLaser ()
	{
		if( DecAndCheck('w') ) return;
		//sounds.PlayWeaponAudio("laser");
//		float acuteTriangleAngle = 60.0f;
		
		Vector3 pos = GetGunOrigin();
		Ray firedir = new Ray(pos, getFireDir()-pos);
		float nx, ny, nz;
		MgunLine.SetPosition(0, pos);
		
		for( int i = 1; i < 15; i++ )
		{
			nx = firedir.GetPoint(i).x + 2 * Random.insideUnitCircle.x;
			ny = firedir.GetPoint(i).y + 1 * Random.insideUnitCircle.y;
			nz = firedir.GetPoint(i).z;

			Vector3 refFrame = new Vector3(nx,ny,nz);
			
			MgunLine.SetPosition(i, refFrame);
			Collider[] urDone4 = Physics.OverlapSphere( refFrame, 0.1f );
			foreach (Collider enemy in urDone4)
			{
				if( enemy && enemy.rigidbody ) 
				{
					PlayerWeaponController enemyHealth = enemy.GetComponentInParent <PlayerWeaponController> ();
					if(enemyHealth != null)
					{
						if( enemyHealth.playerNumber != playerNumber )
							enemyHealth.TakeDamage (3, refFrame);	
					}
				}
			}
			
		}
		MgunLine.enabled = true;
		MgunLight.enabled = true;
	}
	
	Vector3 RotateVectorY(Vector3 oldDirection, float angle)   
	{
		float newX = Mathf.Sin(angle*Mathf.Deg2Rad) * (oldDirection.z) + Mathf.Sin(angle*Mathf.Deg2Rad) * (oldDirection.x);   
		float newY = oldDirection.y;    		
		float newZ = Mathf.Cos(angle*Mathf.Deg2Rad) * (oldDirection.z) - Mathf.Cos(angle*Mathf.Deg2Rad) * (oldDirection.y);        
		return new Vector3(newX, newY, newZ);   
	}
	
	
	void OnTriggerEnter(Collider collider)
	{
		if( collider.gameObject.tag == "Weapon" )
		{
			
			MgunLine.enabled = false;	
			MgunLight.enabled = false;
			int randWeapon = Mathf.Abs(((int)(Random.insideUnitCircle.x*1000)))%3;//Mathf.Abs((int)(Random.insideUnitCircle.x*100)) % 3;
			if( randWeapon == 0 )
			{
				itemsHeld[0] = 1;
				numWeapon = 50;
				SetUpMgun();
				
				ammoPic.gameObject.SetActive(true);
				laserPic.gameObject.SetActive(false);
				rocketPic.gameObject.SetActive(false);
				
			}
			else if ( randWeapon == 1 )
			{
				itemsHeld[0] = 2;
				numWeapon = 100;
				SetUpLaser();
				
				laserPic.gameObject.SetActive(true);
				ammoPic.gameObject.SetActive(false);
				rocketPic.gameObject.SetActive(false);
			}			
			else if ( randWeapon == 2 )
			{
				itemsHeld[0] = 3;
				numWeapon = 10;
				rocketPic.gameObject.SetActive(true);
				ammoPic.gameObject.SetActive(false);
				laserPic.gameObject.SetActive(false);
			}
			weaponCounter.text = numWeapon.ToString();
			weaponCounter.gameObject.SetActive(true);
			collider.gameObject.SetActive(false);
			Destroy(collider.gameObject);
		}
		if( collider.gameObject.tag == "Defense" )
		{
			int randWeapon = Mathf.Abs(((int)(Random.insideUnitCircle.x*1000)))%3;//Mathf.Abs((int)(Random.insideUnitCircle.x*100)) % 3;
			
			if( randWeapon == 0 ){
				itemsHeld[1] = 1;
				numDefense = 1;
				shieldPic.gameObject.SetActive(true);
				boosterPic.gameObject.SetActive(false);
				tntPic.gameObject.SetActive(false);
			}
			else if ( randWeapon == 1 ) {
				itemsHeld[1] = 2;
				numDefense = 50;
				boosterPic.gameObject.SetActive(true);
				shieldPic.gameObject.SetActive(false);
				tntPic.gameObject.SetActive(false);
			}
			else if ( randWeapon == 2 ) {
				itemsHeld[1] = 3;
				numDefense = 1;
				boosterPic.gameObject.SetActive(false);
				shieldPic.gameObject.SetActive(false);
				tntPic.gameObject.SetActive(true);
			}
			
			defenseCounter.text = numDefense.ToString();
			defenseCounter.gameObject.SetActive(true);
			collider.gameObject.SetActive(false);
			Destroy(collider.gameObject);	
		}
		if( collider.gameObject.tag == "TNTBomb" ) 
		{
			PlayerWeaponController playerHealth = gameObject.GetComponentInParent<PlayerWeaponController>();
			playerHealth.TakeDamage( 60, collider.transform.position );
			rigidbody.AddExplosionForce(10000.0f, collider.transform.position, 6.0f, 5.0f );
			Instantiate( explosion, collider.transform.position, collider.transform.rotation);
			
			collider.gameObject.SetActive(false);
			Destroy(collider.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if( collision.gameObject.tag == "Player" ) 
		{
			PlayerWeaponController enemyHealth = collision.collider.GetComponent<PlayerWeaponController> ();
			enemyHealth.TakeDamage( (int)(myCar.get_speed()*2), collision.collider.transform.position );
			
			PlayerWeaponController playerHealth = gameObject.GetComponent<PlayerWeaponController>();
			playerHealth.TakeDamage( 10, collision.transform.position );
		}

	}
	
	bool DecAndCheck(char c) 
	{
		if( c == 'w' ) 
		{
			numWeapon--;
			weaponCounter.text = numWeapon.ToString();
			if( numWeapon <= 0 ) {
				itemsHeld[0] = 0;
				laserPic.gameObject.SetActive(false);
				rocketPic.gameObject.SetActive(false);
				ammoPic.gameObject.SetActive(false);
				weaponCounter.gameObject.SetActive(false);
				return true;
			}
			timer = 0.0f;
		}
		else if ( c == 'd' )
		{		
			numDefense--;
			defenseCounter.text = numDefense.ToString();
			if( numDefense <= 0 ) {
				itemsHeld[1] = 0;
				boosterPic.gameObject.SetActive(false);
				shieldPic.gameObject.SetActive(false);
				defenseCounter.gameObject.SetActive(false);
			}
			timer2 = 0;
		}
		return false;
	}

	void SetUpMgun()
	{
		MgunLine.SetVertexCount(2);
		MgunLine.material = new Material(Shader.Find("Particles/Additive"));
		MgunLine.SetWidth(.1f,.1f);
		MgunLine.SetColors( new Color( 1.0f, 1.0f, 1.0f ), new Color( 1.0f, 1.0f, 1.0f) );
	}
	void SetUpLaser () 
	{
		MgunLine.material = new Material(Shader.Find("Particles/Additive"));
		MgunLine.SetColors(new Color(1.0f,0.0f,1.0f), new Color(0.0f,1.0f,1.0f));
		MgunLine.SetWidth(0.4F, 0.2F);
		MgunLine.SetVertexCount(15);
	}
	
	public void TakeDamage (int amount, Vector3 pointContact)
	{		
		currentHealth -= amount;
		healthSlider.value = currentHealth;
		gunParticles.transform.position = pointContact;
		gunParticles.Play();
		
		if( currentHealth <= 0 )
		{
			Death ();
		}
	}
	
	public void HealCar ( int amount ) {
		currentHealth += 20;
		if( currentHealth > 100 )  currentHealth = 100;
		healthSlider.value = currentHealth;
		
	}
	
	
	void Death ()
	{
		rigidbody.AddExplosionForce(3000.0f, transform.position, 100.0f, 20.0f );
		Instantiate( explosion, transform.position, transform.rotation);
		
		currentHealth = 100;
		healthSlider.value = currentHealth;
	
	}
	
	void OnGUI() 
	{
		GUI.DrawTexture ( new Rect(xDraw-sizeX/2,Screen.height-(yDraw+sizeY/2), sizeX , sizeY ),
		                 crosshair, ScaleMode.ScaleToFit, true, 1.0f );

	} 
}