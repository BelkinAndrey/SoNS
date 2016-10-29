using UnityEngine;
using System.Collections;

public class LineNum : MonoBehaviour {

	public GameObject NeironTarget;
	private float yellowFon;

	void Start () 
	{
		gameObject.GetComponent<LineRenderer> ().SetPosition (0, gameObject.transform.position);
		
		if (NeironTarget != null) 
		{
			gameObject.GetComponent<LineRenderer> ().SetPosition (1, NeironTarget.transform.position);
		}
		else gameObject.GetComponent<LineRenderer> ().SetPosition (1, gameObject.transform.position);
	}

	void Update ()
	{

		if (NeironTarget != null)  //&& (RN)
		{
			if (NeironTarget.GetComponent<NeironScript>().ActionN) yellowFon = 0.0f;

			if (yellowFon < 1f) 
			{
				yellowFon += Time.deltaTime * 0.8f;
				gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, yellowFon, 0.5f);
				gameObject.GetComponent<LineRenderer>().SetColors(new Color(1f, 1f, yellowFon, 0.5f), new Color(1f, 1f, yellowFon, 0.5f));
			}
		}

	}

	public void ApplyTarget (GameObject TargetOject)  
	{
		NeironTarget = TargetOject;
	}
}
