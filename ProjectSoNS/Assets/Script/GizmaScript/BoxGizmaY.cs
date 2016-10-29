using UnityEngine;
using System.Collections;

public class BoxGizmaY : MonoBehaviour {

	public bool Plus;
	
	private Vector3 position0;
	private Vector3 position1;
	
	public bool ActionMouve
	{
		get { return _ActionMouve;}
		set 
		{
			_ActionMouve = value;
			if (_ActionMouve) 
			{
				
				position0 = transform.position;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000.0F)) position1 = hit.point;
				position1 = position0 - position1;
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
		if (!ActionMouve) gameObject.GetComponent<MeshRenderer>().material.color = new Color32 (255, 255, 0, 255);
	}
	
	void OnMouseExit()
	{
		if (!ActionMouve) gameObject.GetComponent<MeshRenderer>().material.color = new Color32 (255, 255, 0, 100);
	}
	
	void OnMouseDown()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0)) ActionMouve = true;
	}
	
	void Update ()
	{
		if ((!Input.GetKey(KeyCode.Mouse0)) && ActionMouve) {
			ActionMouve = false;
			gameObject.GetComponent<MeshRenderer>().material.color = new Color32 (255, 255, 0, 100);
		}
		
		if (ActionMouve) 
		{
			Plane plane = new Plane (Vector3.forward, transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			Vector3 positionPointMouse;
			if (plane.Raycast(ray, out rayDistance)) 
			{
				positionPointMouse = ray.GetPoint(rayDistance);
				positionPointMouse[0] = position0.x;
				positionPointMouse[1] += position1.y;
				positionPointMouse[2] = position0.z;
				if (Plus) {
					if (positionPointMouse[1] < gameObject.transform.parent.transform.position.y + 10) positionPointMouse[1] = gameObject.transform.parent.transform.position.y + 10;
				} else {
					if (positionPointMouse[1] > gameObject.transform.parent.transform.position.y - 10) positionPointMouse[1] = gameObject.transform.parent.transform.position.y - 10;
				}
				gameObject.transform.position = positionPointMouse;
				gameObject.transform.parent.SendMessage("ActionY", Mathf.Abs(gameObject.transform.localPosition.y));
			}
		}
	}
}
