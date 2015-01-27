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
				Vector3 delta = (pos1 - this.transform.lossyScale/2) - (thatP - that.transform.lossyScale/2);
				if (delta.y >= 0 && vel.y <= 0) { // Check coming from above and moving down
					float rightEdgeD = (pos0.x - this.transform.lossyScale.x/2) - (that.transform.position.x + that.transform.lossyScale.x/2);
					float leftEdgeD = (pos0.x + this.transform.lossyScale.x/2) - (that.transform.position.x - that.transform.lossyScale.x/2);
					if(rightEdgeD > 0f) { // Check coming from right
						Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), true);
						layerTimerClip = 1;
					} else if(leftEdgeD < 0f) { // Check coming from left
						Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Character"), LayerMask.NameToLayer("Platforms"), true);
						layerTimerClip = 1;
					} else { // Otherwise, land!
						Vector3 newPos = transform.position;
						newPos.y = thatP.y + (this.transform.lossyScale.y/2) + (that.transform.lossyScale.y/2);
						transform.position = newPos;
						vel.y = 0;
					}
				}
				break;

			case PE_Collider.guy: // collide with guy
				PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
				Destroy(that.gameObject);
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