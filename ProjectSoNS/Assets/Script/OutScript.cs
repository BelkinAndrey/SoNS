using UnityEngine;
using System.Collections;

public class OutScript : MonoBehaviour {

	public GameObject NeironTarget;

	public bool EnableLetter
	{
		get { return _EnableLetter; }
		set 
		{
			_EnableLetter = value;
			if (_EnableLetter)
			{ 
				gameObject.GetComponent<SpriteRenderer>().color = new Color(0.94f, 0.94f, 0.35f, 0.8f);
				gameObject.GetComponent<LineRenderer> ().SetColors(new Color(0.94f, 0.94f, 0.35f, 0.8f), new Color(0.94f, 0.94f, 0.35f, 0.8f));
				gameObject.GetComponent<LineRenderer> ().SetWidth(0.5f, 0.5f);
				if (NeironTarget != null) NeironTarget.SendMessage("ActiveNeiron");
			}
			else 
			{
				gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4f);
				gameObject.GetComponent<LineRenderer> ().SetColors(new Color(1f, 1f, 1f, 0.6f), new Color(1f, 1f, 1f, 0.6f));
				gameObject.GetComponent<LineRenderer> ().SetWidth(0.2f, 0.2f);
			}
		}
	}

	private bool _EnableLetter;

	void Start()
	{
		EnableLetter = false;

		gameObject.GetComponent<LineRenderer> ().SetPosition (0, gameObject.transform.position);
		
		if (NeironTarget != null) 
		{
			gameObject.GetComponent<LineRenderer> ().SetPosition (1, NeironTarget.transform.position);
		}
		else gameObject.GetComponent<LineRenderer> ().SetPosition (1, gameObject.transform.position);
	}

    public void ApplyTarget(GameObject TargetOject)  //Для получения сообщения (SendMessage)
	{
		NeironTarget = TargetOject;
	}
}
