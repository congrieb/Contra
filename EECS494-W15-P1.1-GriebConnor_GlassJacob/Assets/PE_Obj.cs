using UnityEngine;
using System.Collections;


public class PE_Obj : MonoBehaviour {
	public bool			still = false;
	public PE_Collider	coll = PE_Collider.platform;
	public PE_GravType	grav = PE_GravType.constant;
	
	public Vector3		acc = Vector3.zero;
	
	public Vector3		vel = Vector3.zero;
	public Vector3		vel0 = Vector3.zero;
	
	public Vector3		pos0 = Vector3.zero;
	public Vector3		pos1 = Vector3.zero;

	protected virtual void Start() {
		if (PhysEngine.objs.IndexOf(this) == -1) {
			PhysEngine.objs.Add(this);
		}
	}
	
	void OnTriggerEnter(Collider other) {
		// Ignore collisions of still objects (moving handles collision)
		if (still) return;

		// 
		PE_Obj otherPEO = other.GetComponent<PE_Obj>();
		if (otherPEO == null) return;
		
		ResolveCollisionWith(otherPEO);
	}
	
	void OnTriggerStay(Collider other) {
		OnTriggerEnter(other);
	}
	
	protected virtual void ResolveCollisionWith (PE_Obj that){
	
	}
}
