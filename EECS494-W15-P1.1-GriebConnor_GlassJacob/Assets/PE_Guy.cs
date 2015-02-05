using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FacingDir{
	upFaceLeft,
	upFaceRight,
	upRight,
	right,
	downRight,
	down,
	downLeft,
	left,
	upLeft,
}

public enum GunType{
	normal,
	machineGun,
	spreadGun,
	laser,
	flame
}

public class PE_Guy : PE_Obj {
	public FacingDir fD = FacingDir.right;
	public bool isDead = false;
	private int lives = 3;
	private float deathTime = 0f;
	private Vector3 deathPos;

	public bool isCrouching = false;
	private bool isInAir = false;
	private bool isClimbing = false;
	private bool didIFall = false;


	public float speed = 5f;

	private FacingDir lastDir = FacingDir.right;
	public float jumpSpeed = 20f;

	protected float layerTimerCrouch = 0;
	private float layerTimerCrouchMax = .25f;

	private float climbTimer = 0f;

	//How long between shots
	public float normFireRate = .1f;
	public float normBurstRate = 1f;
	private float fireRate = .1f;
	//How long between bursts
	private float burstRate = 1f;
	public GunType gunType = GunType.normal;
	private float nextBurst;
	private int numFired;
	private float nextFire;
	private bool bulletTime = false;

	private bool isInWater;
	private SpriteRenderer spriteRend;


	//Sprites
	public Sprite SliceLeft;
	public Sprite SliceRight;
	public Sprite SliceJump;
	public Sprite SliceCrouchRight;
	public Sprite SliceCrouchLeft;
	public Sprite SliceUpRight;
	public Sprite SliceUpLeft;
	public Sprite SliceDownLeft;
	public Sprite SliceDownRight;
	public Sprite SliceUpFaceRight;
	public Sprite SliceUpFaceLeft;






	// Prefab for bullets
	public GameObject bulletPrefab;

	override protected void Start(){
		spriteRend = this.GetComponent<SpriteRenderer> ();
		base.Start();
	}

	private void respawn(bool fell , Vector3 whereAt){

		print ("You fell");
		whereAt.y  = 300;
		if(didIFall)
			whereAt.x -= 20;
		acc.y = 0;
		vel.y = 0;
		this.transform.position = whereAt;
		isDead = false;
		gunType = GunType.normal;
		fireRate = normFireRate;
		burstRate = normBurstRate;
		
	}
	
	public void death(bool fell, Vector3 whereAt){
		print ("I'm hit");
		isDead = true;
		deathPos = whereAt;
		Vector3 temp = Vector3.zero;
		temp.y = 10000;
		didIFall = fell;
		this.transform.position = temp;
		deathTime = Time.time;
		lives--;
		GameObject scoreCount = GameObject.Find ("LivesCounter");
		string lifeText = "Lives: " + lives.ToString ();
		Text oldText = scoreCount.GetComponent<Text> ();
		oldText.text = lifeText;
	}

	void updateSprite(){

		if (isInAir == true)
						spriteRend.sprite = SliceJump;
				else if (isCrouching == true && vel.x == 0) {
						if (fD == FacingDir.right || fD == FacingDir.downRight || fD == FacingDir.upRight) {
								spriteRend.sprite = SliceCrouchRight;
						} else {
								spriteRend.sprite = SliceCrouchLeft;
						}
				} else {
						switch (fD) {
						case FacingDir.downLeft:
								spriteRend.sprite = SliceDownLeft;
								break;
						case FacingDir.upLeft:
								spriteRend.sprite = SliceUpLeft;
								break;
						case FacingDir.downRight:
								spriteRend.sprite = SliceDownRight;
								break;
						case FacingDir.upRight:
								spriteRend.sprite = SliceUpRight;
								break;
						case FacingDir.right:
								spriteRend.sprite = SliceRight;
								break;
						case FacingDir.left:
								spriteRend.sprite = SliceLeft;
								break;
						case FacingDir.upFaceLeft:
								spriteRend.sprite = SliceUpFaceLeft;
								break;
						case FacingDir.upFaceRight:
								spriteRend.sprite = SliceUpFaceRight;
								break;
						}
						BoxCollider box = this.GetComponent<BoxCollider> ();
						Vector3 newCenter = box.center;
						if (fD == FacingDir.downLeft || fD == FacingDir.left || fD == FacingDir.upLeft) {
								newCenter.x = .1f;
						} else {
								newCenter.x = -.1f;
						}
						box.center = newCenter;
				}
	}

	// Update is called once per frame
	void Update () {
		//Handle respawning
		if (isDead && ((deathTime + 1f) < Time.time)) {
				if(lives > -1)
						respawn (didIFall, deathPos);
		}

		if (lives < 0)
						Application.LoadLevel (Application.loadedLevel);

		//Reset Burst Fire
			if (Time.time > nextBurst)
						numFired = 0;

		updateSprite ();

		// Bullet-time
		if(Input.GetKeyDown(KeyCode.R)) {
			if( Application.loadedLevelName == "_Scene_0")
				return;
			if(!bulletTime){
				speed *= 2f;
				vel.x *= 2f;
				fireRate *= .5f;
				burstRate *= .5f;
				Time.timeScale = (1f/3f);
				bulletTime = true;
			} else {
				speed /= 2f;
				vel.x /= 2f;
				fireRate /= .5f;
				burstRate /= .5f;
				Time.timeScale = 1f;
				bulletTime = false;
			}
		}

		// Left Button
		if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			if(isCrouching){
				makeNotCrouch();
				fD = FacingDir.downLeft;
			} else {
				fD = FacingDir.left;
			}
			lastDir = FacingDir.left;
			vel.x += -speed;
		}
		if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) {
			vel.x += speed;
		}

		// Right Button
		if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
			if(isCrouching){
				makeNotCrouch();
				fD = FacingDir.downRight;
			} else {
				fD = FacingDir.right;
			}
			lastDir = FacingDir.right;
			vel.x += speed;
		}
		if(Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) {
			vel.x += -speed;
		}

		// Down Button
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			if(vel.x == 0 && vel.y == 0 && !isCrouching) {
				makeCrouch();
			} else if(vel.x == 0 && vel.y != 0) {
				fD = FacingDir.down;
			} else if(vel.x > 0) {
				fD = FacingDir.downRight;
			} else if(vel.x < 0) {
				fD = FacingDir.downLeft;
			}
		}

		if(Input.GetKeyDown(KeyCode.P)){
			Application.LoadLevel("_Scene_1");
		}
		if (Input.GetKeyDown (KeyCode.O)) {
						Application.LoadLevel ("_Scene_0");
		}

		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) {
			if(isCrouching) {
				makeNotCrouch();
			} else if(fD == FacingDir.downLeft) {
				fD = FacingDir.left;
				spriteRend.sprite = SliceLeft;
			} else if(fD == FacingDir.downRight) {
				spriteRend.sprite = SliceRight;
				fD = FacingDir.right;
			} else if(fD == FacingDir.down){
				fD = lastDir;
			}
		}

		// Up Button
		if (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.UpArrow)) {
			if(vel.x == 0){
				if(fD == FacingDir.left || fD == FacingDir.upLeft || fD == FacingDir.downLeft || fD == FacingDir.upFaceLeft){
					fD = FacingDir.upFaceLeft;
				} else  {
					fD = FacingDir.upFaceRight;	
				}
			} else if(vel.x > 0) {
				fD = FacingDir.upRight;
			} else if(vel.x < 0) {
				fD = FacingDir.upLeft;
			}
		}
		if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) {
			if(fD == FacingDir.upFaceLeft || fD == FacingDir.upLeft){
				fD = FacingDir.left;
			} else {
				fD = FacingDir.right;
			}
		}


		// Jump Button
		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Period)) {
			if(vel.y == 0 && !isCrouching && !isInWater){ // Jump
				vel.y += jumpSpeed;
				makeJump();
				isInAir = true;
			} else if(isCrouching) { // Drop below
				makeNotCrouch();
				// Fall through platforms
				Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), true);
				layerTimerCrouch = Time.time;
			}
		}

		// Shoot Button
		//Machine Gun fire
		if ((Input.GetKey(KeyCode.Z) || Input.GetKey (KeyCode.Comma)) && gunType == GunType.machineGun) {
			shootBullet();
		}
		//Normal  fire
		if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma)){
			shootBullet();
		}


	}

	void makeCrouch() {
	

		// Change fD
		switch (fD) {
		case FacingDir.downLeft:
			fD = FacingDir.left;
			break;
		case FacingDir.upLeft:
			fD = FacingDir.left;
			break;
		case FacingDir.downRight:
			fD = FacingDir.right;
			break;
		case FacingDir.upRight:
			fD = FacingDir.right;
			break;
		case FacingDir.down:
			fD = lastDir;
			break;
		}
		//Change position

		Vector3 newPos = transform.position;
		float oldHeight = SliceRight.bounds.size.y;
		float newHeight = SliceCrouchRight.bounds.size.y;
		float scaleChange = newHeight / oldHeight;
		print (scaleChange);
		newPos.y -= scaleChange * this.transform.lossyScale.y / 2;
		this.transform.position = newPos;
		print (newPos.y);

		BoxCollider box = this.GetComponent<BoxCollider> ();
		Vector3 boxBound = box.size;
		boxBound.y *= scaleChange;
		box.size = boxBound;
		isCrouching = true;
	}


	void makeJump() {
		// Make not crouching
		float oldHeight = SliceRight.bounds.size.y;
		float newHeight = SliceCrouchRight.bounds.size.y;
		float scaleChange = newHeight / oldHeight;
		
		BoxCollider box = this.GetComponent<BoxCollider> ();
		Vector3 boxBound = box.size;
		boxBound.y *= 1f * scaleChange;
		box.size = boxBound;

	}

	void makeNotJump() {
		// Make not crouching
		float oldHeight = SliceRight.bounds.size.y;
		float newHeight = SliceCrouchRight.bounds.size.y;
		float scaleChange = newHeight / oldHeight;
		
		BoxCollider box = this.GetComponent<BoxCollider> ();
		Vector3 boxBound = box.size;
		boxBound.y *= 1f / scaleChange;
		box.size = boxBound;
		
	}


	void makeNotCrouch() {
		// Make not crouching
		float oldHeight = SliceRight.bounds.size.y;
		float newHeight = SliceCrouchRight.bounds.size.y;
		float scaleChange = newHeight / oldHeight;

		Vector3 newPos = transform.position;
		newPos.y += scaleChange * this.transform.lossyScale.y /2;
		this.transform.position = newPos;

		BoxCollider box = this.GetComponent<BoxCollider> ();
		Vector3 boxBound = box.size;
		boxBound.y *= 1f/scaleChange;
		box.size = boxBound;

		isCrouching = false;
		fD = lastDir;
	}

	void shootBullet() {
		if (Time.time < nextFire)
			return;
		else if (numFired >= 4) {
			return;
		} else {
			if(numFired == 0) {
				nextBurst = Time.time + burstRate;
			}
			nextFire = Time.time + fireRate;
			numFired++;
		}
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = transform.position;
		float bulletSpeed = bullet.GetComponent<PE_Bullet>().speed;
		float fireAngle = 0; 
		switch (fD) {
		case FacingDir.upFaceLeft:
			fireAngle = Mathf.PI / 2;
			bullet.GetComponent<PE_Bullet>().vel.y = bulletSpeed;
			break;
		case FacingDir.upFaceRight:
			fireAngle = Mathf.PI / 2;
			bullet.GetComponent<PE_Bullet>().vel.y = bulletSpeed;
			break;
		case FacingDir.upRight:
			fireAngle = Mathf.PI / 4;
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.right:
			fireAngle = 0;
			bullet.GetComponent<PE_Bullet>().vel.x = bulletSpeed;
			break;
		case FacingDir.downRight:
			fireAngle = 7*Mathf.PI/4;
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(7*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(7*Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.down:
			fireAngle = 3 * Mathf.PI / 2;
			bullet.GetComponent<PE_Bullet>().vel.y = -bulletSpeed;
			break;
		case FacingDir.downLeft:
			fireAngle = 5*Mathf.PI/4;
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(5*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(5*Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.left:
			fireAngle = Mathf.PI;
			bullet.GetComponent<PE_Bullet>().vel.x = -bulletSpeed;
			break;
		case FacingDir.upLeft:
			fireAngle = 3*Mathf.PI/4;
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(3*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(3*Mathf.PI/4) * bulletSpeed;
			break;
		}
		if (gunType == GunType.spreadGun) {
			spreadFire(fireAngle);
		}
	}

	void spreadFire(float fireAngle){

		GameObject bullet1 = Instantiate (bulletPrefab) as GameObject;
		bullet1.transform.position = transform.position;
		GameObject bullet2 = Instantiate (bulletPrefab) as GameObject;
		bullet2.transform.position = transform.position;
		GameObject bullet3 = Instantiate (bulletPrefab) as GameObject;
		bullet3.transform.position = transform.position;
		GameObject bullet4 = Instantiate (bulletPrefab) as GameObject;
		bullet4.transform.position = transform.position;

		float spreadRate = Mathf.PI / 23;

		float bulletSpeed = bullet1.GetComponent<PE_Bullet>().speed;

		bullet1.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(fireAngle - (2 * spreadRate)) * bulletSpeed;
		bullet1.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(fireAngle - (2 * spreadRate)) * bulletSpeed;

		bullet2.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(fireAngle - (spreadRate)) * bulletSpeed;
		bullet2.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(fireAngle - (spreadRate)) * bulletSpeed;

		bullet3.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(fireAngle + (spreadRate)) * bulletSpeed;
		bullet3.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(fireAngle + (spreadRate)) * bulletSpeed;

		bullet4.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(fireAngle + (2 * spreadRate)) * bulletSpeed;
		bullet4.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(fireAngle + (2 * spreadRate)) * bulletSpeed;

	}
	
	void FixedUpdate(){
		if (Time.time >= (layerTimerCrouch + layerTimerCrouchMax)) {
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), false);
		}


	
	}

	private void RepositionToTop(PE_Obj that) {
		Vector3 thatP = that.transform.position;
		Vector3 newPos = transform.position;
		float boxSizeY = this.GetComponent<BoxCollider> ().size.y;
		newPos.y = thatP.y + (this.transform.lossyScale.y * .5f * boxSizeY) + (that.transform.lossyScale.y / 2);
		transform.position = newPos;
		vel.y = 0;
	}

	override protected void ResolveCollisionWith(PE_Obj that) {
			switch (that.coll) {
				case PE_Collider.platform:
					// In Progress
					Vector3 thatP = that.transform.position;
					Vector3 delta = (pos1 - this.transform.lossyScale / 2) - (thatP - that.transform.lossyScale / 2);
					if (isInWater) {
							if (!isClimbing) {
									climbTimer = Time.time;
									isClimbing = true;
							}
							Vector3 newPos = transform.position;

							//Take a quarter of a second to climb up ledge
							if (Time.time > climbTimer + .15f) {
									isInWater = false;
									isClimbing = false;
									RepositionToTop (that);
									break;
							}
							newPos = transform.position;
							if (transform.position.x > thatP.x) {
									newPos.x = thatP.x + (that.transform.lossyScale.x / 2);
							} else if (transform.position.x < thatP.x) {

									newPos.x = thatP.x - (that.transform.lossyScale.x / 2);
							}
							this.transform.position = newPos;
				
						} else if (delta.y >= 0) { // Top
								if (vel.y <= 0) {
										// Landed!
										if (isInAir) {
												isInAir = false;
												makeNotJump ();
										}
										if (fD == FacingDir.down)
												fD = lastDir;
										RepositionToTop (that);
								}
						}
						break;


				//TODO Change this to handle sprites properly
				case PE_Collider.water:
					//If you are in the water then you can no longer jump
					isInWater = true;
					if(isInAir){
						isInAir = false;
						makeNotJump();
					}

					RepositionToTop (that);
					break;

				case PE_Collider.enemyBullet:
					if(!isDead)
						death(false, transform.position);
			break;

				case PE_Collider.powerup:
					PowerupType pt = that.GetComponent<PE_Powerup>().powerupType;
					print("You got a powerup of type: " + pt);
					switch(pt){
						case PowerupType.rapidFire:
							fireRate *= .85f;
							burstRate *= .85f;
							break;
						case PowerupType.machineGun:
							gunType = GunType.machineGun;
							fireRate = .12f;
							burstRate = .15f;
							break;
						case PowerupType.spreadGun:
							gunType = GunType.spreadGun;
							fireRate = normFireRate;
							burstRate = normBurstRate;
							break;
						case PowerupType.laser:
							gunType = GunType.laser;
							fireRate = 1f;
							burstRate = 1f;
							break;
						case PowerupType.flame:
							gunType = GunType.flame;
							fireRate = .3f;
							burstRate = 1f;
							break;
					}
					// Remove powerup from game
					PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
					Destroy(that.gameObject);
					break;
				}
			

		}

}