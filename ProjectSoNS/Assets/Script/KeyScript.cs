using UnityEngine;
using System.Collections;

public class KeyScript : MonoBehaviour {

	public KeyCode Code;   


	void Update () {

		if (Input.GetKey(Code)) gameObject.GetComponent<OutScript>().EnableLetter = true;
		else gameObject.GetComponent<OutScript>().EnableLetter = false;
	
	}
}
