using UnityEngine;
using System.Collections;

public enum EnemyType {
	runner,
	turretMan,
	redTurret,
	greyTurret
}

public class Spawner : MonoBehaviour {
	public GameObject runnerPrefab;
	public GameObject turretManPrefab;
	public GameObject redTurretPrefab;
	public GameObject greyTurretPrefab;

	public EnemyType enemyType = EnemyType.runner;
	public bool multiSpawner = false;
	public float spacingTime = 2f;

	private bool onlyOnce = false;

	void FixedUpdate () {
		if (!onlyOnce && justOffScreen()) {
			onlyOnce = true;
			if (multiSpawner) {
				InvokeRepeating("spawn", 0, spacingTime);		
			} else {
				spawn();
				Destroy(this.gameObject);
			}
		}
		if (onScreen()) {
			Destroy(this.gameObject);
		}
	}

	void spawn() {
		switch (enemyType) {
		case EnemyType.runner:
			GameObject runnerGO = (GameObject) Instantiate(runnerPrefab);
			runnerGO.transform.position = this.transform.position;
			break;
		case EnemyType.turretMan:
			GameObject turretManGO = (GameObject) Instantiate(turretManPrefab);
			turretManGO.transform.position = this.transform.position;
			break;
		case EnemyType.redTurret:
			GameObject redTurretGO = (GameObject) Instantiate(redTurretPrefab);
			redTurretGO.transform.position = this.transform.position;
			break;
		case EnemyType.greyTurret:
			GameObject greyTurretGO = (GameObject) Instantiate(greyTurretPrefab);
			greyTurretGO.transform.position = this.transform.position;
			break;
		}
	}

	bool onScreen(){
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float screenLeft = Camera.main.camera.transform.position.x - horzExtent;
		float screenRight = Camera.main.camera.transform.position.x + horzExtent;
		
		float myPos = this.transform.position.x + this.transform.lossyScale.x;
		
		if (myPos < screenRight && myPos > screenLeft)
			return true;
		else
			return false;
	}

	bool justOffScreen(){
		float vertExtent = Camera.main.camera.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		float screenRight = Camera.main.camera.transform.position.x + horzExtent;
		
		float myPos = this.transform.position.x + this.transform.lossyScale.x;

		if (myPos < (screenRight + 20f))
			return true;
		else
			return false;
	}
}

