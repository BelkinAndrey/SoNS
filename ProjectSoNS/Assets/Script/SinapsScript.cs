using UnityEngine;
using System.Collections;

public class SinapsScript : MonoBehaviour {

	public GameObject NeironTarget; //Целевой нейрон
	public float Force; //Сила синапса
	public int TypeSinaps; //Тип синапса

	public bool GoAction; //Индикатор активности

	private byte BStart;
	private byte BEnd;

	private bool Up = true;

	private byte BA = 255;
	private float BH = 0.6f;

	void Start () {
		
		gameObject.GetComponent<LineRenderer> ().SetPosition (0, gameObject.transform.position);
		
		if (NeironTarget != null) 
		{
			gameObject.GetComponent<LineRenderer> ().SetPosition (1, NeironTarget.transform.position);
		}
	}


	void FixedUpdate () //Анимация
	{
		if (GoAction) {
			BStart = 0;
			BA = 255;
			BH = 0.4f;
			GoAction = false;
		}

		if (BStart == 111) {
			BEnd = BStart;
			Up = true;
		}

		if (BEnd == 0) Up = false;

			if (BStart < 222) BStart++;
			if (Up) { if (BEnd > 0) BEnd--; }
			else { if (BEnd < 222) BEnd++; }

		if (BA > 109) BA--;
		else BH = 0.2f;

		gameObject.GetComponent<LineRenderer>().SetColors(new Color32 (221, 221, BStart, BA), new Color32 (221, 221, BEnd, BA));
		gameObject.GetComponent<LineRenderer> ().SetWidth (BH, BH);
	}
	
}
