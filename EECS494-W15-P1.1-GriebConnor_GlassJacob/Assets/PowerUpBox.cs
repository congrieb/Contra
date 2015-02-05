using UnityEngine;
using System.Collections;

public class PowerUpBox : PE_Obj {

	private int health = 2;

	public GameObject powerUpPrefab;




	// Update is called once per frame
	void Update () {
		if(health <= 0){
			// Instantiate Powerup
			GameObject powerupGO = Instantiate(powerUpPrefab) as GameObject;
			powerupGO.transform.position = this.transform.position;
		
			// Kill Self
			PhysEngine.objs.Remove(this.GetComponent<PE_Obj>());
			Destroy(this.gameObject);
		}
	}

	protected override void ResolveCollisionWith (PE_Obj that)
	{
		if (that.coll == PE_Collider.friendlyBullet) {
			// Kill bullet
			PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
			Destroy(that.gameObject);
			health--;
		}
	}

}
