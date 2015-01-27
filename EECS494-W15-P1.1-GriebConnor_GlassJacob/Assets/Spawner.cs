using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	public GameObject runnerPrefab;

	// Use this for initialization
	void Start() {
		spawn();
	}
	
	void spawn() {
		GameObject runner = (GameObject) Instantiate(runnerPrefab);
		runner.transform.position = this.transform.position;
		Destroy (this.gameObject);
	}
}
