using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)){
			Application.LoadLevel("_Scene_1");
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			Application.LoadLevel ("_Scene_0");
		}

	
	}
}
