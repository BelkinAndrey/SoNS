using UnityEngine;
using System.Collections;

public class LockPic : MonoBehaviour {
	

	void Update () {
		transform.LookAt (Camera.main.transform);
	}
}
