using UnityEngine;
using System.Collections;

public class BridgeWatcher : MonoBehaviour {
	public GameObject bridge1;
	public GameObject bridge2;
	public GameObject bridge3;
	public GameObject bridge4;

	private bool notYetPassed = true;

	void FixedUpdate () {
		if(guyHasPassed() && notYetPassed) {
			notYetPassed = false;
			print("Booyah");
			Invoke("killBridge1", .5f);
			Invoke("killBridge2", 1.3f);
			Invoke("killBridge3", 2.1f);
			Invoke("killBridge4", 2.9f);
		}
	}

	void killBridge1(){
		print("Booyah");
		PhysEngine.objs.Remove(bridge1.GetComponent<PE_Obj>());
		Destroy(bridge1);
	}

	void killBridge2(){
		print("Booyah");
		PhysEngine.objs.Remove(bridge2.GetComponent<PE_Obj>());
		Destroy(bridge2);
	}

	void killBridge3(){
		print("Booyah");
		PhysEngine.objs.Remove(bridge3.GetComponent<PE_Obj>());
		Destroy(bridge3);
	}

	void killBridge4(){
		print("Booyah");
		PhysEngine.objs.Remove(bridge4.GetComponent<PE_Obj>());
		Destroy(bridge4);
		Destroy(this.gameObject);
	}
	
	bool guyHasPassed(){
		float guyPos = Camera.main.camera.transform.position.x;
		float myPos = this.transform.position.x;
		if (guyPos >= myPos)
			return true;
		else
			return false;
	}
	
}

