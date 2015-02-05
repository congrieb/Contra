using UnityEngine;
using System.Collections;

public class PE_Runner : PE_Obj {
	public float speed = 3f;

	protected int layerTimerCrouch = 0;
	protected int layerTimerClip = 0;
	private int layerTimerCrouchMax = 20;
	private int layerTimerClipMax = 15;


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

	override protected void ResolveCollisionWith(PE_Obj that) {
		switch (this.coll) {

		case PE_Collider.enemy:

			switch (that.coll) {

			case PE_Collider.platform: // collide with platform
				Vector3 thatP = that.transform.position;
				Vector3 newPos = transform.position;
				float boxSizeY = this.GetComponent<BoxCollider> ().size.y;
				newPos.y = thatP.y + (this.transform.lossyScale.y * .5f * boxSizeY) + (that.transform.lossyScale.y / 2);
				transform.position = newPos;
				vel.y = 0;

				break;

			case PE_Collider.guy: // collide with guy
				PE_Guy guy = that.GetComponent<PE_Guy> ();
				if(!guy.isDead)
					guy.death(false, that.transform.position);
				break;

			case PE_Collider.friendlyBullet: // collide with friendlyBullet
				PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
				Destroy(that.gameObject);
				PhysEngine.objs.Remove(this.GetComponent<PE_Obj>());
				Destroy(this.gameObject);
				break;
			}
			break;
		}
	}
}