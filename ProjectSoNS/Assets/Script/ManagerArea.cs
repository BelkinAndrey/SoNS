using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerArea : MonoBehaviour {

	public GameObject Gizma;

	public GameObject prefabAreaBox;
	public GameObject prefabAreaBall;

	public GameObject GlobalArea;
	public List<GameObject> hitArea = new List<GameObject>();

	private bool AddAreaBool = false;
	private Transform CamTarget;
	private string[] selStrings = new string[] {"Box", "Ball"};  
	private int selGridInt = 0;                                       
	private int selNewGrid = 0;

	private string NameStrig = "Area brain";
	public GameObject SelectArea;
	private int SelectIndex;

	private GameObject cloneArea;
	private Vector2 _ScrollBlok;
	private Vector2 _ScrollBlok2;

	private float SelectR;
	private float SelectG;
	private float SelectB;

	public bool BlokVisible = false;                           			//Кнопка ">" "<"  
	private string[] selBlok = new string[] {"Positive", "Negative"}; 	//Кнопки Positive / Negative
	private int selBlokInt = 0;                                 		//индекс Positive / Negative
	private Vector2 _inventoryScrollBlok;                   			 //Полосы прокрутки

	private float PlasticForArea = 0f;

	void Start(){
		CamTarget = GameObject.Find("Target").transform;
	}

	void OnGUI (){
		////Editor
		if (!AddAreaBool){
			if (GUI.Button(new Rect(350, 10, 80, 20), "Edit Area")){
				Gizma.transform.position = CamTarget.position;
				Gizma.SetActive(true);
				AddAreaBool = true;
			}
		} else {
			if (GUI.Button(new Rect(350, 10, 80, 20), "Add Area")){
				if (selGridInt == 0) {
					cloneArea = (GameObject) Instantiate(prefabAreaBox, Gizma.transform.position, Gizma.transform.rotation);
					cloneArea.transform.localScale = new Vector3(Gizma.GetComponent<AreaGizma>().S*2, Gizma.GetComponent<AreaGizma>().H*2, Gizma.GetComponent<AreaGizma>().L*2);
					cloneArea.GetComponent<AreaScript>().S = Gizma.GetComponent<AreaGizma>().S * 2;
					cloneArea.GetComponent<AreaScript>().H = Gizma.GetComponent<AreaGizma>().H * 2;
					cloneArea.GetComponent<AreaScript>().L = Gizma.GetComponent<AreaGizma>().L * 2;
				} else {
					cloneArea = (GameObject) Instantiate(prefabAreaBall, Gizma.transform.position, Gizma.transform.rotation);
					cloneArea.transform.localScale = new Vector3(Gizma.GetComponent<AreaGizma>().Radiys*2, Gizma.GetComponent<AreaGizma>().Radiys*2, Gizma.GetComponent<AreaGizma>().Radiys*2);
					cloneArea.GetComponent<AreaScript>().R = Gizma.GetComponent<AreaGizma>().Radiys;
				}
				cloneArea.GetComponent<AreaScript>().Name = NameStrig;
				cloneArea.name = "Area(" + NameStrig + ")";
				hitArea.Add(cloneArea);
				AddAreaBool = false;
				Gizma.SetActive(false);
			}

			if (GUI.Button(new Rect(431, 10, 50, 20), "End")){
				Gizma.SetActive(false);
				AddAreaBool = false;
			}
			selNewGrid = GUI.SelectionGrid(new Rect(350, 31, 130, 20), selNewGrid, selStrings, selStrings.Length);
			if (selNewGrid != selGridInt) {
				selGridInt = selNewGrid;
				if (selGridInt == 0) Gizma.GetComponent<AreaGizma>().BallOnOff(false);
				else Gizma.GetComponent<AreaGizma>().BallOnOff(true);
			} 

			int NewIdexArea = hitArea.Count + 1;
			GUI.Label(new Rect(330, 52, 20, 22), "" + NewIdexArea + " -");
			NameStrig = GUI.TextField(new Rect(350, 52, 130, 22), NameStrig);
			if (selGridInt == 1){
				GUI.Label(new Rect(350, 75, 100, 20), "R" + Gizma.GetComponent<AreaGizma>().Radiys.ToString("0.0"));
			} else {
				float SS = Gizma.GetComponent<AreaGizma>().S * 2;
				float HH = Gizma.GetComponent<AreaGizma>().H * 2;
				float LL = Gizma.GetComponent<AreaGizma>().L * 2;
				GUI.Label(new Rect(350, 75, 130, 20), SS.ToString("0.0") + " x " + HH.ToString("0.0") + " x " + LL.ToString("0.0"));
			}
		}
		//////END Editor
		/// Selektor
		if (GUI.Button(new Rect(Screen.width - 150, 30, 120, 20), "Select NULL")) SelectArea = null;
		if (GUI.Button(new Rect(Screen.width - 150, 51, 120, 20), "Global Area")) SelectArea = GlobalArea;

		if (hitArea.Count > 0){
			GUILayout.BeginArea(new Rect( Screen.width - 150, 72, 135, 150), GUI.skin.box);
			_ScrollBlok = GUILayout.BeginScrollView(_ScrollBlok, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
			for (int i = 0; i < hitArea.Count; i++){
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("" + (i+1) + " - " + hitArea[i].GetComponent<AreaScript>().Name)) {
					SelectArea = hitArea[i];
					CamTarget.position = hitArea[i].transform.position;
					SelectIndex = i + 1;
					SelectR = SelectArea.GetComponent<AreaScript>().ColorR;
					SelectG = SelectArea.GetComponent<AreaScript>().ColorG;
					SelectB = SelectArea.GetComponent<AreaScript>().ColorB;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		if (SelectArea != null){
			if (!SelectArea.GetComponent<AreaScript>().Global){
				if (GUI.Button(new Rect(Screen.width - 20, 51, 20, 20), "x")) {
					hitArea.Remove(SelectArea);
					Destroy(SelectArea);
					SelectArea = null;
				}
			}
		}

		////End Selector
		/// Info
		if (SelectArea != null){

			GUILayout.BeginArea(new Rect( Screen.width - 150, 225, 135, Screen.height - 230), GUI.skin.box);
			_ScrollBlok2 = GUILayout.BeginScrollView(_ScrollBlok2, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
			if (!SelectArea.GetComponent<AreaScript>().Global){
				GUILayout.Label("" + SelectIndex + " - " + SelectArea.GetComponent<AreaScript>().Name);
				GUILayout.Label("Color Area RGB:");
				SelectR = GUILayout.HorizontalSlider(SelectR, 0, 1);
				SelectG = GUILayout.HorizontalSlider(SelectG, 0, 1);
				SelectB = GUILayout.HorizontalSlider(SelectB, 0, 1);
				SelectArea.GetComponent<Renderer>().material.color = new Color(SelectR, SelectG, SelectB, 0.25f);
				SelectArea.GetComponent<AreaScript>().ColorR = SelectR;
				SelectArea.GetComponent<AreaScript>().ColorG = SelectG;
				SelectArea.GetComponent<AreaScript>().ColorB = SelectB;
			} else {
				GUILayout.Label("Global Area");
			}
			GUILayout.Label("Total:        " + SelectArea.GetComponent<AreaScript>().amount);
			GUILayout.Label("Total Action: " + SelectArea.GetComponent<AreaScript>().NeironActionList.Count.ToString());
            GUILayout.Label("Novelty:    " + SelectArea.GetComponent<AreaScript>().LevelOriginality.ToString("0"));
			GUILayout.Label("");
			GUILayout.Label("Plastic (" + PlasticForArea.ToString("0.00") + ")");
			PlasticForArea = GUILayout.HorizontalScrollbar(PlasticForArea, 0f, 0f, 1f);
			if (GUILayout.Button("Plast. OK")) {
				GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron");
							foreach (GameObject N in OjectNeiron) 
							{
								if (SelectArea == N.GetComponent<NeironScript>().Area) {
									N.GetComponent<NeironScript>().Plasticity = PlasticForArea;
								}
							}
			}
			if (GUILayout.Button("base. OK")) {
				GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron");
							foreach (GameObject N in OjectNeiron) 
							{
								if (SelectArea == N.GetComponent<NeironScript>().Area) {
									N.GetComponent<NeironScript>().BasicPlasticity = PlasticForArea;
								}
							}
			}


			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		///Blok
		if (SelectArea != null){
			if (BlokVisible)
			{
				if (GUI.Button(new Rect(Screen.width - 175, Screen.height - 25, 20, 20), ">")) BlokVisible = false;
				selBlokInt = GUI.SelectionGrid(new Rect( 10, Screen.height - 225, 200, 20), selBlokInt, selBlok, selBlok.Length);
				
				GUILayout.BeginArea(new Rect( 10, Screen.height - 200, Screen.width - 190, 195), GUI.skin.box);
				_inventoryScrollBlok = GUILayout.BeginScrollView(_inventoryScrollBlok, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
				
				GUILayout.BeginHorizontal();
				for (int i = 0; i < 16; i++)
				{
					GUILayout.BeginVertical(); 
					if (selBlokInt == 0) GUILayout.Label("" + SelectArea.GetComponent<AreaScript>().Spike1[i].ToString("0.0"),
					                                     GUILayout.Width(40), GUILayout.ExpandWidth(false));
					else GUILayout.Label("" + SelectArea.GetComponent<AreaScript>().Spike2[i].ToString("0.0"), 
					                     GUILayout.Width(40), GUILayout.ExpandWidth(false));
					
					if (selBlokInt == 0) SelectArea.GetComponent<AreaScript>().Spike1[i] = GUILayout.VerticalSlider(SelectArea.GetComponent<AreaScript>().Spike1[i], 10f, -10f);
					else SelectArea.GetComponent<AreaScript>().Spike2[i] = GUILayout.VerticalSlider(SelectArea.GetComponent<AreaScript>().Spike2[i], 10f, -10f);
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
				
				GUILayout.EndScrollView();
				GUILayout.EndArea();
				
			} else
			{
				if (GUI.Button(new Rect(Screen.width - 175, Screen.height - 25, 20, 20), "<")) BlokVisible = true;
			}
		}
	}
}
