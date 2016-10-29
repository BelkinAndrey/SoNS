using UnityEngine;
using System.Collections;

public class IndexScript : MonoBehaviour {

	/*Для демострации требовалось прекрепить индикатор новизны к пряилугольной области*/
	void Update () {
        gameObject.GetComponent<TextMesh>().text = gameObject.transform.parent.gameObject.GetComponent<AreaScript>().LevelOriginality.ToString("0");	
	}
}
