using UnityEngine;
using System.Collections;

public class AreaGizma : MonoBehaviour {

	public bool ball;

	public GameObject BoxPX;
	public GameObject BoxMX;
	public GameObject BoxPY;
	public GameObject BoxMY;
	public GameObject BoxPZ;
	public GameObject BoxMZ;

	public GameObject ChildBall;
	public GameObject ChildBox;

	public float Radiys = 10f;
	public float S = 10f;
	public float H = 10f;
	public float L = 10f;

	public void ActionX (float X){
		if (!BoxPX.GetComponent<BoxGizmaX>().ActionMouve) BoxPX.transform.localPosition = new Vector3(X, 0, 0);
		if (!BoxMX.GetComponent<BoxGizmaX>().ActionMouve) BoxMX.transform.localPosition = new Vector3(-X, 0, 0);
		if (ball) ActionBall(X);
		else {
			ChildBox.transform.localScale = new Vector3(2*X, ChildBox.transform.localScale.y, ChildBox.transform.localScale.z);
			S = X;
		}
	}

	public void ActionY (float Y){
		if (!BoxPY.GetComponent<BoxGizmaY>().ActionMouve) BoxPY.transform.localPosition = new Vector3(0, Y, 0);
		if (!BoxMY.GetComponent<BoxGizmaY>().ActionMouve) BoxMY.transform.localPosition = new Vector3(0, -Y, 0);
		if (ball) ActionBall(Y);
		else{
			ChildBox.transform.localScale = new Vector3(ChildBox.transform.localScale.x, 2*Y, ChildBox.transform.localScale.z);
			H = Y;
		}
	}

	public void ActionZ (float Z){
		if (!BoxPZ.GetComponent<BoxGizmaZ>().ActionMouve) BoxPZ.transform.localPosition = new Vector3(0, 0, Z);
		if (!BoxMZ.GetComponent<BoxGizmaZ>().ActionMouve) BoxMZ.transform.localPosition = new Vector3(0, 0, -Z);
		if (ball) ActionBall(Z);
		else {
			ChildBox.transform.localScale = new Vector3(ChildBox.transform.localScale.x, ChildBox.transform.localScale.y, 2*Z);
			L = Z;
		}
	}

	public void ActionBall (float R){
		if (!BoxPX.GetComponent<BoxGizmaX>().ActionMouve) BoxPX.transform.localPosition = new Vector3(R, 0, 0);
		if (!BoxMX.GetComponent<BoxGizmaX>().ActionMouve) BoxMX.transform.localPosition = new Vector3(-R, 0, 0);
		if (!BoxPY.GetComponent<BoxGizmaY>().ActionMouve) BoxPY.transform.localPosition = new Vector3(0, R, 0);
		if (!BoxMY.GetComponent<BoxGizmaY>().ActionMouve) BoxMY.transform.localPosition = new Vector3(0, -R, 0);
		if (!BoxPZ.GetComponent<BoxGizmaZ>().ActionMouve) BoxPZ.transform.localPosition = new Vector3(0, 0, R);
		if (!BoxMZ.GetComponent<BoxGizmaZ>().ActionMouve) BoxMZ.transform.localPosition = new Vector3(0, 0, -R);

		ChildBall.transform.localScale = new Vector3(2*R, 2*R, 2*R);
		Radiys = R;
	}

	public void BallOnOff (bool B){
		ball = B;
		if (B) {
			ChildBall.SetActive(true);
			ChildBox.SetActive(false);
			ActionBall(Radiys);
		}
		else {
			ChildBall.SetActive(false);
			ChildBox.SetActive(true);
			ActionX(S);
			ActionY(H);
			ActionZ(L);
		}
	}
}
