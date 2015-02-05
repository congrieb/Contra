using UnityEngine;
using System.Collections;

public class PE_FlameBullet : PE_Bullet {
	public float xSpeed = 0;
	public float ySpeed = 0;
	public float ogTime = 0;

	void FixedUpdate () {
		vel.y = ySpeed + (30 * Mathf.Cos((Time.time - ogTime) * 10));
		vel.x = xSpeed + (30 * Mathf.Sin((Time.time - ogTime) * 10));
	}
}