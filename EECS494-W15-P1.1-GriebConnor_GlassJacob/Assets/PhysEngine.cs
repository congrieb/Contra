using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PE_GravType {
	none,
	constant,
	planetary
}

public enum PE_Collider {
	friendlyBullet,
	enemyBullet,
	platform,
	guy,
	enemy,
	water
}

public class PhysEngine : MonoBehaviour {
	static public List<PE_Obj>	objs;

	public Vector3		gravity = new Vector3(0,-20f,0);

	
	// Use this for initialization
	void Awake() {
		objs = new List<PE_Obj>();
	}


	bool OffScreen(PE_Obj po){
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float left = this.transform.position.x - horzExtent;
		float right = this.transform.position.x + horzExtent;
		float top = this.transform.position.y + vertExtent;
		float bottom = this.transform.position.y - vertExtent;

		float objLeft = po.transform.position.x - po.transform.lossyScale.x;
		float objRight = po.transform.position.x + po.transform.lossyScale.x;
		float objBottom = po.transform.position.y - po.transform.lossyScale.y;
		float objTop = po.transform.position.y + po.transform.lossyScale.y;

		if (objLeft > right || objRight < left || objTop < bottom || objBottom > top)
						return true;
				else
						return false;

	}

	bool OnScreen(PE_Obj po){
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float left = this.transform.position.x - horzExtent;
		float right = this.transform.position.x + horzExtent;
		float top = this.transform.position.y + vertExtent;
		float bottom = this.transform.position.y - vertExtent;
		
		float objLeft = po.transform.position.x - po.transform.lossyScale.x;
		float objRight = po.transform.position.x + po.transform.lossyScale.x;
		float objBottom = po.transform.position.y - po.transform.lossyScale.y;
		float objTop = po.transform.position.y + po.transform.lossyScale.y;
		
		if (objRight < right && objLeft > left && objBottom > bottom && objTop < top)
			return true;
		else
			return false;

	}
	
	void FixedUpdate () {
		// Handle the timestep for each object
		float dt = Time.fixedDeltaTime;

		foreach (PE_Obj po in objs) {
			//Adjust Camera if guy has moved pass mid way point of the screen
			if(OffScreen(po) && po.coll == PE_Collider.friendlyBullet){
				objs.Remove(po);
				Destroy(po.gameObject);
				break;
			}
			if( po.coll == PE_Collider.guy && po.transform.position.x > this.transform.position.x){
				Vector3 newPos = this.transform.position;
				newPos.x = po.transform.position.x;
				this.transform.position = newPos;
			}
			TimeStep(po, dt);
		}
		
		// Resolve collisions
		
		
		// Finalize positions
		foreach (PE_Obj po in objs) {
			po.transform.position = po.pos1;
		}
		
	}
	
	
	public void TimeStep(PE_Obj po, float dt) {
		if (po.still) {
			po.pos0 = po.pos1 = po.transform.position;
			return;
		}
		
		// Velocity
		po.vel0 = po.vel;
		Vector3 tAcc = po.acc;
		switch (po.grav) {
		case PE_GravType.constant:
			tAcc += gravity;
			break;
		case PE_GravType.none:
			break;
		}
		po.vel += tAcc * dt;

		//Determine if po is to the left of screen
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float leftEdge = (this.transform.position.x - (horzExtent)) + 1.5f;
		if (po.pos1.x <= leftEdge && po.vel.x < 0) {

			switch (po.coll) {
				case PE_Collider.guy:
						print ("Go wings");
						po.pos1 = po.pos0 = po.transform.position;
						po.pos1 += po.vel * dt;

						po.pos0.x = po.pos1.x = po.transform.position.x;
						
						return;
						break;
				}
		}
		// Position
		po.pos1 = po.pos0 = po.transform.position;
		po.pos1 += po.vel * dt;
		
	}
}
