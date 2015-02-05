using UnityEngine;
using System.Collections;

public enum TurretType{
	man,
	grey,
	red,
	boss
}

public class Turret : PE_Obj {

	public PE_Guy guy;
	public GameObject bulletPrefab;
	public TurretType type;
	private int fireTimer = 0;
	private int health;


	// Use this for initialization
	override protected void Start () {
		if (type == TurretType.man)
						health = 1;
		else if (type == TurretType.boss)
						health = 8;
		else
						health = 5;
		base.Start ();

		guy = FindObjectOfType(typeof(PE_Guy)) as PE_Guy;

	
	}

	void FixedUpdate () {
		if (health <= 0) {
				PhysEngine.objs.Remove (this.GetComponent<PE_Obj> ());
				Destroy (this.gameObject);
		}
		Vector3 diff = this.transform.position - guy.transform.position;
		if (Mathf.Abs(diff.y) > 50)
						return;
		float fireAngle = Mathf.Atan (diff.y / diff.x);
		if (guy.transform.position.x <= this.transform.position.x)
						fireAngle += Mathf.PI;
		float fireAngleCeil =  Mathf.Ceil(fireAngle / (Mathf.PI / 6)) * (Mathf.PI/6); 
		float fireAngleFloor =  Mathf.Floor(fireAngle / (Mathf.PI / 6)) * (Mathf.PI/6);
		float ceilDiff = Mathf.Abs (fireAngle - fireAngleCeil);
		float floorDiff = Mathf.Abs (fireAngle - fireAngleFloor);
		if (ceilDiff < floorDiff)
				fireAngle = fireAngleCeil;
		else
				fireAngle = fireAngleFloor;

		if ((fireAngle <= (Mathf.PI / 2) || fireAngle >= (3 * Mathf.PI / 2)) && type == TurretType.red)
						return;
		if (fireTimer < 60)
						fireTimer++;
		else {
				GameObject bullet = Instantiate (bulletPrefab) as GameObject;
				Vector3 bulPos = transform.position;
				bulPos.y += 1;
				bullet.transform.position = bulPos;
				float bulletSpeed = bullet.GetComponent<PE_Bullet> ().speed;

				bullet.GetComponent<PE_Bullet> ().vel.x = Mathf.Cos (fireAngle) * bulletSpeed;
				bullet.GetComponent<PE_Bullet> ().vel.y = Mathf.Sin (fireAngle) * bulletSpeed;
				fireTimer = 0;
		}
	}

	override protected void ResolveCollisionWith(PE_Obj that){
		if (that.coll == PE_Collider.friendlyBullet) {
			PhysEngine.objs.Remove(that.GetComponent<PE_Obj>());
			Destroy(that.gameObject);
			health--;
		}
	}
}
