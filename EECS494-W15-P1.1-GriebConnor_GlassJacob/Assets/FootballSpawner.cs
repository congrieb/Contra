﻿using UnityEngine;
using System.Collections;

public class FootballSpawner : MonoBehaviour {
	public GameObject footballPrefab;
	
	void FixedUpdate () {
		if (justLeftScreen ()) {
			spawn ();
			Destroy(this.gameObject);
		}
	}
	
	void spawn() {
		GameObject footballGO = (GameObject) Instantiate(footballPrefab);
		footballGO.transform.position = this.transform.position;
	}
	
	bool justLeftScreen(){
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float screenLeft = Camera.main.camera.transform.position.x - horzExtent;

		float myPos = this.transform.position.x + this.transform.lossyScale.x;
		
		if (myPos < screenLeft)
			return true;
		else
			return false;
	}

}

