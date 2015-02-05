using UnityEngine;
using System.Collections;

public enum PowerupType{
	rapidFire,
	machineGun,
	spreadGun,
	laser,
	flame
}

public class PE_Powerup : PE_Obj {
	public PowerupType powerupType;

	public Material machineColor;
	public Material spreadColor;
	public Material flameColor;
	public Material laserColor;
	public Material rapidColor;

	override protected void Start(){
		switch (this.powerupType) {
		case PowerupType.rapidFire:
			renderer.material = rapidColor;
			break;

		case PowerupType.machineGun:
			renderer.material = machineColor;
			break;
		case PowerupType.spreadGun:
			renderer.material = spreadColor;
			break;
		case PowerupType.laser:
			renderer.material = laserColor;
			break;
		case PowerupType.flame:
			renderer.material = flameColor;
			break;

		}
		base.Start ();
	}

	override protected void ResolveCollisionWith(PE_Obj that) {
		switch (that.coll) {
			case PE_Collider.platform: // collide with platform
				Vector3 thatP = that.transform.position;
				Vector3 delta = (pos1 - this.transform.lossyScale/2) - (thatP - that.transform.lossyScale/2);
				if (delta.y >= 0 && vel.y <= 0) { // Check coming from above and moving down
					Vector3 newPos = transform.position;
					newPos.y = thatP.y + (this.transform.lossyScale.y/2) + (that.transform.lossyScale.y/2);
					transform.position = newPos;
					vel.y = 0;
					vel.x = 0;
				}
				break;
		}
	}
}
