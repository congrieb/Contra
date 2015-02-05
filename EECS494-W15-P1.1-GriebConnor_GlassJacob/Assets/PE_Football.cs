using UnityEngine;
using System.Collections;


public class PE_Football : PE_Obj {
	public GameObject powerupPrefab;
	public PowerupType powerupType;

	void FixedUpdate () {
		vel.y = 25 * Mathf.Sin(Time.time * 4);
	}

	override protected void ResolveCollisionWith(PE_Obj that) {
		switch (that.coll) {
			case PE_Collider.friendlyBullet: // collide with friendlyBullet
				// Kill bullet
				PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
				Destroy(that.gameObject);

				// Instantiate Powerup
				GameObject powerupGO = (GameObject) Instantiate(powerupPrefab);
				powerupGO.transform.position = this.transform.position;
				powerupGO.GetComponent<PE_Powerup>().powerupType = this.powerupType;

				// Kill Self
				PhysEngine.objs.Remove(this.GetComponent<PE_Obj>());
				Destroy(this.gameObject);
				break;
			}
	}
}
