using UnityEngine;
using System.Collections;

public enum TurretType{
	man,
	grey,
	red
}

public class Turret : MonoBehaviour {

	public PE_Guy guy;
	public GameObject bulletPrefab;
	public TurretType type;
	int fireTimer = 0;


	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate () {

		Vector3 diff = this.transform.position - guy.transform.position;
		float fireAngle = Mathf.Atan (diff.y / diff.x);
		if (guy.transform.position.x <= this.transform.position.x)
						fireAngle += Mathf.PI;
		fireAngle =  Mathf.Floor(fireAngle / (Mathf.PI / 6)) * (Mathf.PI/6);
		if ((fireAngle <= (Mathf.PI / 2) || fireAngle >= (3 * Mathf.PI / 2)) && type == TurretType.red)
						return;
		print (fireAngle);
		if (fireTimer < 60)
						fireTimer++;
		else {
				GameObject bullet = Instantiate (bulletPrefab) as GameObject;
				bullet.transform.position = transform.position;
				float bulletSpeed = bullet.GetComponent<PE_Bullet> ().speed;

				bullet.GetComponent<PE_Bullet> ().vel.x = Mathf.Cos (fireAngle) * bulletSpeed;
				bullet.GetComponent<PE_Bullet> ().vel.y = Mathf.Sin (fireAngle) * bulletSpeed;
				fireTimer = 0;
		}
	}
}
