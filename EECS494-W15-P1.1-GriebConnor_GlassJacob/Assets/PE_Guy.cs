using UnityEngine;
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

public class PE_Guy : PE_Obj {
	public FacingDir fD = FacingDir.right;

	public bool isCrouching = false;
	private bool isInAir = false;
	private bool isClimbing = false;

	public float speed = 5f;

	private FacingDir lastDir = FacingDir.right;
	public float jumpSpeed = 20f;
	private Vector3 ogScale;
	private float tuckHeightRatio = .5f;
	
	protected int layerTimerCrouch = 0;
	protected int layerTimerClip = 0;
	private int layerTimerCrouchMax = 20;
	private int layerTimerClipMax = 15;

	private float climbTimer = 0f;

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
		ogScale = transform.lossyScale;
		spriteRend = this.GetComponent<SpriteRenderer> ();
		base.Start();
	}

	void updateSprite(){

		if (isInAir == true)
						spriteRend.sprite = SliceJump;
		else if (isCrouching == true && vel.x == 0) {
			if(fD == FacingDir.right || fD == FacingDir.downRight || fD == FacingDir.upRight){
				spriteRend.sprite = SliceCrouchRight;
			}
			else{
				spriteRend.sprite = SliceCrouchLeft;
			}
		}
		else {
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
		}

	}

	// Update is called once per frame
	void Update () {
		//print (fD);

		updateSprite ();

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
				layerTimerCrouch = 1;
			}
		}

		// Shoot Button
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
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = transform.position;
		float bulletSpeed = bullet.GetComponent<PE_Bullet>().speed;
		switch (fD) {
		case FacingDir.upFaceLeft:
			bullet.GetComponent<PE_Bullet>().vel.y = bulletSpeed;
			break;
		case FacingDir.upFaceRight:
			bullet.GetComponent<PE_Bullet>().vel.y = bulletSpeed;
			break;
		case FacingDir.upRight:
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.right:
			bullet.GetComponent<PE_Bullet>().vel.x = bulletSpeed;
			break;
		case FacingDir.downRight:
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(7*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(7*Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.down:
			bullet.GetComponent<PE_Bullet>().vel.y = -bulletSpeed;
			break;
		case FacingDir.downLeft:
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(5*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(5*Mathf.PI/4) * bulletSpeed;
			break;
		case FacingDir.left:
			bullet.GetComponent<PE_Bullet>().vel.x = -bulletSpeed;
			break;
		case FacingDir.upLeft:
			bullet.GetComponent<PE_Bullet>().vel.x = Mathf.Cos(3*Mathf.PI/4) * bulletSpeed;
			bullet.GetComponent<PE_Bullet>().vel.y = Mathf.Sin(3*Mathf.PI/4) * bulletSpeed;
			break;
		}
	}
	
	void FixedUpdate(){
		if (layerTimerCrouch >= layerTimerCrouchMax) {
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), false);
		} else if (layerTimerCrouch > 0){
			layerTimerCrouch++;
		}

		if (layerTimerClip >= layerTimerClipMax) {
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), false);
		} else if (layerTimerClip > 0){
			layerTimerClip++;
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
		switch (this.coll) {
		case PE_Collider.guy:
			switch (that.coll) {
			case PE_Collider.platform:
				
				// In Progress
				Vector3 thatP = that.transform.position;
				Vector3 delta = (pos1 - this.transform.lossyScale/2) - (thatP - that.transform.lossyScale/2);
				if(isInWater){
					if(!isClimbing){
						climbTimer = Time.time;
						isClimbing = true;
					}
					Vector3 newPos = transform.position;

					//Take a quarter of a second to climb up ledge
					if(Time.time > climbTimer + .15f){
						isInWater = false;
						isClimbing = false;
						RepositionToTop(that);
						break;
					}
					newPos = transform.position;
					if(transform.position.x > thatP.x){
						newPos.x = thatP.x + (that.transform.lossyScale.x/2);
					}
					else if(transform.position.x < thatP.x){

						newPos.x = thatP.x - (that.transform.lossyScale.x/2);
					}
					this.transform.position = newPos;
				
				}

				else if (delta.y >= 0) { // Top
					
					if(vel.y <= 0){
						float rightEdgeD = (pos0.x - this.transform.lossyScale.x/2) - (that.transform.position.x + that.transform.lossyScale.x/2);
						float leftEdgeD = (pos0.x + this.transform.lossyScale.x/2) - (that.transform.position.x - that.transform.lossyScale.x/2);
						if(rightEdgeD > 0f) {
							Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), true);
							layerTimerClip = 1;
						} else if(leftEdgeD < 0f) {
							Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), true);
							layerTimerClip = 1;
						} else {
							// Landed!
							if(isInAir){
								isInAir = false;
								makeNotJump();
							}
							if(fD == FacingDir.down)
								fD = lastDir;
							RepositionToTop(that);
						}
					}
				}
				
				break;
				//TODO Change this to handle sprites properly
			case PE_Collider.water:

				//If you are in the water then you can no longer jump
				isInWater = true;
				isInAir = false;

				RepositionToTop(that);
				break;
			}
			
			break;
		}
	}
}