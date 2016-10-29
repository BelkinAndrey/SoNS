using UnityEngine;
using System.Collections;

public class BlokEnterScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera.main.GetComponent<StartSceneScript>().EntryOject = gameObject;
	}
	

}
