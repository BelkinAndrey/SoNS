using UnityEngine;
using System.Collections;

public class GizmaEnterX : MonoBehaviour {

	public GameObject X2;

	private Vector3 position0;
	private Vector3 position1;

	private bool ActionMouve
	{
		get { return _ActionMouve;}
		set 
		{
			_ActionMouve = value;
			if (_ActionMouve) 
			{
				gameObject.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 255), new Color32 (255, 0, 0, 255));
				X2.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 255), new Color32 (255, 0, 0, 255));
				position0 = transform.position;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000.0F)) position1 = hit.point;
				position1 = position0 - position1;
			} else 
			{
				gameObject.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 100), new Color32 (255, 0, 0, 30));
				X2.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 100), new Color32 (255, 0, 0, 30));
			}
		}
	}

	private bool _ActionMouve;

	void Start()
	{
		ActionMouve = false;
	}

	void OnMouseEnter ()
	{
		if (!ActionMouve) gameObject.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 255), new Color32 (255, 0, 0, 255));
	}
	
	void OnMouseExit()
	{
		if (!ActionMouve) gameObject.GetComponent<LineRenderer> ().SetColors (new Color32 (255, 0, 0, 100), new Color32 (255, 0, 0, 30));
	}

	void OnMouseDown()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0)) ActionMouve = true;
	}

	void Update ()
	{
		if ((!Input.GetKey(KeyCode.Mouse0)) && ActionMouve) ActionMouve = false;
		if (ActionMouve) 
		{
			Plane plane = new Plane (Vector3.forward, transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			Vector3 positionPointMouse;
			if (plane.Raycast(ray, out rayDistance)) 
			{
				positionPointMouse = ray.GetPoint(rayDistance);
				positionPointMouse[0] += position1.x;
				positionPointMouse[1] = position0.y;
				positionPointMouse[2] = position0.z;
				gameObject.transform.parent.transform.position = positionPointMouse;
			}
		}
	}
	
}
