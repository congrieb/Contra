using UnityEngine;
using System.Collections;

public class BossButton : PE_Obj {
	public int health = 5;


	// Use this for initialization
	void Start () {
	
	}

	override protected void ResolveCollisionWith(PE_Obj that){
		if (that.coll == PE_Collider.friendlyBullet) {
			PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
			Destroy(that.gameObject);
			health--;
		}



		}

	// Update is called once per frame
	void Update () {
		if (health < 0)
						Application.LoadLevel ("_Scene_0");
	}
}
