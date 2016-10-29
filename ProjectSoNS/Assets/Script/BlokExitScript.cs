using UnityEngine;
using System.Collections;

public class BlokExitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera.main.GetComponent<StartSceneScript>().OutputOject = gameObject;
	}

}
