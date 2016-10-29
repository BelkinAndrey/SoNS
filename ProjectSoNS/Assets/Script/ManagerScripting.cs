using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerScripting : MonoBehaviour {

	private bool VisibleEdit = false;

	public class ScriptNet 
	{
	  	public int TypeActivator = 0; 	//Тип активатора 
	  	public int TypeAct = 0; 		//Тип действия

	  	public GameObject ANeiron; 		//Активирующий Нейрон
	  	public GameObject AArea;		//Активирующая область
	  	public float threshold = 0;		//Порог 
	  	public bool controller = true;	//Больше или Меньше
	  	public float delay = 0;			//Задержка?
	  	public float delayStep = 0;	
	  	public string IndexANeiron = "1";
	  	public string thresholdString = "";
	  	public bool hunger = true;      //Голод

	  	public GameObject ActNeiron;	//Целевой нейрон
	  	public GameObject ActArea;		//Целевая область
	  	public float ActMod = 0;		//Модуляция, пластичность
	  	public float TimeMod = 0;		//Время воздействия
	  	public string IndexActNeiron = "";
	  	public string ActModString = "";
	}

	public List<ScriptNet> ListScript = new List<ScriptNet>();

	private ScriptNet DeleteList;

	private Vector2 _ScrollBlok;
	private string[] AToolBarString = new string[] {"0", "N", "%", "!"};
	private string[] ActToolBarString = new string[] {"0", "N", "M", "P"};

	private bool ScriptOnOff = true;

	void OnGUI(){
		if (GUI.Button(new Rect(500, 10, 80, 20), "Edit Script")) VisibleEdit = !VisibleEdit;

		string Buttonstring = "";
		if (ScriptOnOff) Buttonstring = "Script: on"; else Buttonstring = "Script: off";
		if (GUI.Button(new Rect(581, 10, 80, 20), Buttonstring)) ScriptOnOff = !ScriptOnOff;

		if (VisibleEdit) {

			GUILayout.BeginArea(new Rect(500, 35, 200, Screen.height - 240), GUI.skin.box);
			_ScrollBlok = GUILayout.BeginScrollView(_ScrollBlok, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));

			foreach (ScriptNet value in ListScript) 
			{
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical(GUI.skin.box);
					GUILayout.BeginVertical(GUI.skin.box);
					//Активатор блок
					if (value.TypeActivator == 0) value.TypeActivator  = GUILayout.Toolbar(value.TypeActivator , AToolBarString);
					if (value.TypeActivator == 1) {
						GUILayout.Label("Activate Neuron");
						if (value.ANeiron == null) GUILayout.Label("neuron null");
						else GUILayout.Label(value.ANeiron.name);
						GUILayout.Label("index neuron:");
						value.IndexANeiron = GUILayout.TextField(value.IndexANeiron);
						if (GUILayout.Button("Find")) value.ANeiron = GameObject.Find("Neiron" + value.IndexANeiron);
					}
					if (value.TypeActivator == 2) {
						GUILayout.Label("Activity Level");
						if (value.AArea == null) GUILayout.Label("Area null");
						else {
							if (value.AArea.GetComponent<AreaScript>().Global) GUILayout.Label("Global Area");
							else GUILayout.Label(value.AArea.GetComponent<AreaScript>().Name);
						}
						if (GUILayout.Button("selected Area")) value.AArea = gameObject.GetComponent<ManagerArea>().SelectArea;
						GUILayout.Label("Total (" + value.threshold +")");
						string moreless = "";
						if (value.controller) moreless = ">"; else moreless = "<";
						if (GUILayout.Button(moreless)) value.controller = !value.controller;
						value.thresholdString = GUILayout.TextField(value.thresholdString);
						if (GUILayout.Button("OK")) value.threshold = float.Parse(value.thresholdString);
					}
					if (value.TypeActivator == 3){
						GUILayout.Label("Originality");
						if (value.AArea == null) GUILayout.Label("Area null");
						else {
							if (value.AArea.GetComponent<AreaScript>().Global) GUILayout.Label("Global Area");
							else GUILayout.Label(value.AArea.GetComponent<AreaScript>().Name);
						}
						if (GUILayout.Button("selected Area")) value.AArea = gameObject.GetComponent<ManagerArea>().SelectArea;
						if (value.hunger) {
							if (GUILayout.Button("Hunger (" + value.threshold + ")")) value.hunger = !value.hunger;
							value.thresholdString = GUILayout.TextField(value.thresholdString);
							GUILayout.Label("Time (" + value.delay + ")");
							value.IndexANeiron = GUILayout.TextField(value.IndexANeiron);
						} else {
							if (GUILayout.Button("New (" + value.threshold + ")")) value.hunger = !value.hunger;
							value.thresholdString = GUILayout.TextField(value.thresholdString);
						}
						if (GUILayout.Button("OK")) {
							value.delay = float.Parse(value.IndexANeiron);
							value.delayStep = value.delay;
							value.threshold = float.Parse(value.thresholdString);
						}
					}


					GUILayout.EndVertical();
					GUILayout.BeginVertical(GUI.skin.box);
					//Действия блок
					if (value.TypeAct == 0) value.TypeAct  = GUILayout.Toolbar(value.TypeAct , ActToolBarString);
					if (value.TypeAct == 1) {
						GUILayout.Label("Activated Neuron");
						if (value.ActNeiron == null) GUILayout.Label("neuron null");
						else GUILayout.Label(value.ActNeiron.name);
						GUILayout.Label("index neuron:");
						value.IndexActNeiron = GUILayout.TextField(value.IndexActNeiron);
						if (GUILayout.Button("Find")) value.ActNeiron = GameObject.Find("Neiron" + value.IndexActNeiron);
					}
					if (value.TypeAct == 2) {
						GUILayout.Label("Modulation");
						if (value.ActArea == null) GUILayout.Label("Area null");
						else {
							if (value.ActArea.GetComponent<AreaScript>().Global) GUILayout.Label("Global Area");
							else GUILayout.Label(value.ActArea.GetComponent<AreaScript>().Name);
						}
						if (GUILayout.Button("selected Area")) value.ActArea = gameObject.GetComponent<ManagerArea>().SelectArea;
						GUILayout.Label("Module (" + value.ActMod + ")");
						value.ActModString = GUILayout.TextField(value.ActModString);
						if (GUILayout.Button("OK")) value.ActMod = float.Parse(value.ActModString);
					}
					if (value.TypeAct == 3){
						GUILayout.Label("Plasticity");
						if (value.ActArea == null) GUILayout.Label("Area null");
						else {
							if (value.ActArea.GetComponent<AreaScript>().Global) GUILayout.Label("Global Area");
							else GUILayout.Label(value.ActArea.GetComponent<AreaScript>().Name);
						}
						if (GUILayout.Button("selected Area")) value.ActArea = gameObject.GetComponent<ManagerArea>().SelectArea;
						GUILayout.Label("Plastic (" + value.ActMod.ToString("0.00") + ")");
						value.ActMod = GUILayout.HorizontalScrollbar(value.ActMod, 0f, 0f, 1f);
						GUILayout.Label("Time (" + value.TimeMod + ")");
						value.ActModString = GUILayout.TextField(value.ActModString);
						if (GUILayout.Button("OK")) value.TimeMod = float.Parse(value.ActModString);
					}

					GUILayout.EndVertical();
				GUILayout.EndVertical();
				if (GUILayout.Button("X", GUILayout.MaxWidth(25), GUILayout.MaxHeight(25))) DeleteList = value;
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Add Script")) ListScript.Add(new ScriptNet());

			GUILayout.EndScrollView();
			GUILayout.EndArea();

			if (DeleteList != null) ListScript.Remove(DeleteList); 
		}
	}

	void FixedUpdate(){
		
		if (ScriptOnOff){

			foreach (ScriptNet value in ListScript) 
			{
				bool Run = false;
				if (value.TypeActivator == 1) {
					if (value.ANeiron != null) {
						if (value.ANeiron.GetComponent<NeironScript>().ActionN) Run = true;
					}
				}

				if (value.TypeActivator == 2) {
					if (value.AArea != null){
						if (value.controller) {
							if (value.AArea.GetComponent<AreaScript>().NeironActionList.Count > value.threshold) Run = true;
						} else {
							if (value.AArea.GetComponent<AreaScript>().NeironActionList.Count < value.threshold) Run = true;
						}
					}
				}

				if (value.TypeActivator == 3){
					if (value.AArea != null){
						if (value.hunger){
							if (value.threshold > value.AArea.GetComponent<AreaScript>().LevelOriginality) {
								if (value.delayStep > 0) value.delayStep -= 0.01f;
								if (value.delayStep <= 0) Run = true;
							} else value.delayStep = value.delay;
						} else {
							if (value.threshold < value.AArea.GetComponent<AreaScript>().LevelOriginality) Run = true;
						}
					}
				}

				/////////////////////////////////////////////RUN//////////////////////////////////////////////
				if (Run) {
					if (value.TypeAct == 1){
						if (value.ActNeiron != null) value.ActNeiron.SendMessage("ActiveNeiron");
					}

					if (value.TypeAct == 2){
						if (value.ActArea != null) {
							GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron");
							foreach (GameObject N in OjectNeiron) 
							{
								if (value.ActArea == N.GetComponent<NeironScript>().Area) N.SendMessage("AddTActual", value.ActMod);
							}
						}
					}
					
					if (value.TypeAct == 3){
						if (value.ActArea != null) {
							GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron");
							foreach (GameObject N in OjectNeiron) 
							{
								if (value.ActArea == N.GetComponent<NeironScript>().Area) {
									Vector2 PT = new Vector2(value.ActMod, value.TimeMod);
									N.SendMessage("plasticSetTime", PT);
								}
							}	
						}
					}
				}
			}
		}
	}
}
