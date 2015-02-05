using UnityEngine;
using System.Collections;

public class PE_FlameBullet : PE_Obj {
	float xSpeed = 10;
	float ySpeed = 0;
	float ogTime = 0;

	void FixedUpdate () {
		vel.y = ySpeed + (30 * Mathf.Cos((Time.time - ogTime) * 5));
		vel.x = xSpeed + (30 * Mathf.Sin((Time.time - ogTime) * 5));
	}
}