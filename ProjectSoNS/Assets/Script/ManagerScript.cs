using UnityEngine;
using System.Collections;

public class ManagerScript : MonoBehaviour {

	public GameObject GizmaNeiron;                                    //Префаб гизмы нейрона
	private Transform CamTarget;                                       //Цель камеры - кубик
	public GameObject prefabNeiron;                                   //Префаб нейрона
	public GameObject GlobalArea;                                     //Глобальная область 
	public int EndIndexNeiron = 0;                                    //колличество нейронов 
	private bool AddNeironBool = false;                                //Кнопка Edit или Add..

    public GameObject EyeObject;

	public GameObject prefabSinaps;                                   //Префаб синапса
	public GameObject SelP;                                           //Индикатор выбора
	private string[] selStrings = new string[] {"S", "M", "A", "I"};  //Список нейронов
	private int selGridInt = 0;                                       //Индекс списка нейронов
	private int selNewGrid = 0;                                       //Индекс при создании нейрона

	public GameObject Selektor                                        //Селектор нейронв
	{
		get { return _Selektor;}
		set 
		{
			if (_Selektor != null) SelektNeironOff(_Selektor);
			_Selektor = value;
			if (_Selektor != null) SelektNeironOn(_Selektor);
		}
	}
	
	public GameObject SelektorLetterOut                               //Выделение индикаторов входов/выходов
	{
		get { return _SelektorLetterOut;}
		set 
		{
                if (_SelektorLetterOut != null)
                {
                    if (_SelektorLetterOut.tag == "LetterOut") _SelektorLetterOut.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
                    if (_SelektorLetterOut.tag == "EyeEdit")  _SelektorLetterOut.SendMessage("SelectOff");
                }
                _SelektorLetterOut = value;
                if (_SelektorLetterOut != null)
                {
                    if (_SelektorLetterOut.tag == "LetterOut") _SelektorLetterOut.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
                    if (_SelektorLetterOut.tag == "EyeEdit") 
                    {
                        _SelektorLetterOut.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
                        _SelektorLetterOut.SendMessage("SelectOn");
                    }
                    Selektor = null;
                    SelectLetterOut = true;
                }
                else SelectLetterOut = false;
		}
	}

	
	private GameObject _Selektor;                            //Селектор нейронов
	private GameObject _SelektorLetterOut;                   //Седлектор букв
	private bool SelectLetterOut = false;                    //Выбрана ли буква, для луча
	private bool AddSinapsBool = false;                      //Нажата кнопка AddSinaps, для луча

	private Vector2 _inventoryScroll;                  		   //Полосы прокрутки
	
	private bool WindowS = false;                            	//Есть ли окно настройки синапса
	private GameObject SinapsEdit;                           	//Редактируемый синапс
	private string StringWindows;                            	//Строка в окне
	private Rect WindowsRect;                                	//Положения и размер окна синапса
	private string[] selSinaps = new string[] {"A", "B", "C", "D"};	//Кнопка типа синапса
	private int selSinapsInt = 0;                               //Идекс типа синапса


	// Настройки нейрона // 0
	
	private string MaxAdderString = "";

	private string DampferAdderString = "";
	private string thresholdTopString = "";
	private string AnswerTimeString = "";
	private string TimeReposeString = "";

	// Настройки нейрона // 1

	private string thresholdDownString = "";
	private string timeIgnoreString = "";
	private string DempferBonusThresholdString = "";
	private string TimeEvaluationString = "";
	private string LimitRecurrenceString = "";
	private string thresholdTopUpString = "";

	private string AdaptationTimeString = "";
	private string thresholdAdaptString = "";

	// Настройки нейрона // 2

	private string MaxForceSinapsString = "";
	private string TimeChargeString = "";
	private string FocusNeironString = "";
	private string MaxFocusNeironString = "";
	private string StepFocusString = "";

	private string PlasticityString = "";
	private string BasicPlasticityString = "";
	private string StepPlasticityString = "";

	private bool FocusDinamicSetting = false;
	private bool PlasticityDinamicSetting = false;
	private bool NewNeironDinamicSetting = false;


	// END VAR // END VAR // END VAR // END VAR // END VAR // END VAR // END VAR //

	void Start ()
	{
		CamTarget = GameObject.Find("Target").transform;
	}

	private void SelektNeironOn (GameObject ObjectSelekt)    // При выборе нейрона     
	{
		Selektor.GetComponent<LineRenderer> ().enabled = true; //Влючить демонстрацию вектора пути
		gameObject.GetComponent<ManagerArea>().BlokVisible = false;

		selGridInt = Selektor.GetComponent<NeironScript>().TypeIndexNeiron;
		SelP.SetActive(true);
		SelP.transform.position = Selektor.transform.position;

		foreach (GameObject value in ObjectSelekt.GetComponent<NeironScript>().hitSinaps)  //Все связанные нейронам установить 
		{
			value.GetComponent<SinapsScript>().NeironTarget.tag = "SelectSinaps";
		}

		MaxAdderString = Selektor.GetComponent<NeironScript>().MaxAdder.ToString();

		DampferAdderString = Selektor.GetComponent<NeironScript>().DampferAdder.ToString();
		thresholdTopString = Selektor.GetComponent<NeironScript>().thresholdTop.ToString();
		AnswerTimeString = Selektor.GetComponent<NeironScript>().AnswerTime.ToString(); 
		TimeReposeString = Selektor.GetComponent<NeironScript>().TimeRepose.ToString();

		thresholdDownString = Selektor.GetComponent<NeironScript>().thresholdDown.ToString();
		timeIgnoreString = Selektor.GetComponent<NeironScript>().timeIgnore.ToString();
		DempferBonusThresholdString = Selektor.GetComponent<NeironScript>().DempferBonusThreshold.ToString();
		TimeEvaluationString = Selektor.GetComponent<NeironScript>().TimeEvaluation.ToString();
		LimitRecurrenceString = Selektor.GetComponent<NeironScript>().LimitRecurrence.ToString();
		thresholdTopUpString = Selektor.GetComponent<NeironScript>().thresholdTopUp.ToString();

		AdaptationTimeString = Selektor.GetComponent<NeironScript>().AdaptationTime.ToString();
		thresholdAdaptString = Selektor.GetComponent<NeironScript>().thresholdAdapt.ToString();



		MaxForceSinapsString = Selektor.GetComponent<NeironScript>().MaxForceSinaps.ToString();
		TimeChargeString = Selektor.GetComponent<NeironScript>().TimeCharge.ToString();
		FocusNeironString = Selektor.GetComponent<NeironScript>().FocusNeiron.ToString();
		MaxFocusNeironString = Selektor.GetComponent<NeironScript>().MaxFocus.ToString();
		StepFocusString = Selektor.GetComponent<NeironScript>().StepFocus.ToString();

		PlasticityString = Selektor.GetComponent<NeironScript>().Plasticity.ToString();
		BasicPlasticityString = Selektor.GetComponent<NeironScript>().BasicPlasticity.ToString();
		StepPlasticityString = Selektor.GetComponent<NeironScript>().StepPlasticity.ToString();

		FocusDinamicSetting = Selektor.GetComponent<NeironScript>().FocusDinamic;
		PlasticityDinamicSetting = Selektor.GetComponent<NeironScript>().PlasticityDinamic;
		NewNeironDinamicSetting = Selektor.GetComponent<NeironScript>().NewNeironDinamic;

	}
	
	private void SelektNeironOff (GameObject ObjectSelekt)  //При снятии с выбора нейрона
	{
		foreach (GameObject value in ObjectSelekt.GetComponent<NeironScript>().hitSinaps) 
		{
			value.GetComponent<LineRenderer>().SetColors(new Color32( 221, 221, 221, 109), new Color32( 221, 221, 221, 109));
			value.GetComponent<SinapsScript>().NeironTarget.tag = "Neiron";
			value.GetComponent<LineRenderer>().SetWidth(0.2f , 0.2f);
		}
		Selektor.GetComponent<LineRenderer> ().enabled = false;
		SelP.SetActive(false);
		AddSinapsBool = false;
	}

	void Update () 
	{
		int YY = 0;
		if (gameObject.GetComponent<ManagerArea>().BlokVisible) YY = 225;
		else YY = 25;
		if (Input.GetKeyDown(KeyCode.Mouse0) && (!WindowS))
		{
			if (((Selektor == null) && (Input.mousePosition.y > YY)) || ((Input.mousePosition.x > 200) && (Selektor != null) && (Input.mousePosition.y > YY))) 
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000.0F))
					{
						if (hit.collider.tag == "Neiron") 
						{
							if (!SelectLetterOut) 
							{
								if (!AddSinapsBool) Selektor = hit.collider.gameObject;
								else 
								{
									if (hit.collider.gameObject != Selektor) AddSinaps(hit.collider.gameObject);
								}
							} else 
							{
								SelektorLetterOut.SendMessage("ApplyTarget", hit.collider.gameObject);
								SelektorLetterOut.SendMessage("Start");
								SelektorLetterOut = null;
							}
						}

                        if (hit.collider.tag == "SelectSinaps")
						{
							SinapsEditFind(hit.collider.gameObject);
							WindowsRect = new Rect(Input.mousePosition.x + 5f, Screen.height - Input.mousePosition.y - 25f, 205, 55);//Размеры окна
						    selSinapsInt = SinapsEdit.GetComponent<SinapsScript>().TypeSinaps;
						}
				
						if (hit.collider.tag == "LetterOut") SelektorLetterOut = hit.collider.gameObject;
                        if (hit.collider.tag == "EyeEdit") SelektorLetterOut = hit.collider.gameObject;
					}
			
				else 
				{
					Selektor = null;
					SelektorLetterOut = null;
                    EyeObject.GetComponent<EyeF1>().SelectReceptor = null;
				}
			}
		}
		
		if (Selektor != null) 
		{
			foreach (GameObject value in Selektor.GetComponent<NeironScript>().hitSinaps) 
			{
				if (value.GetComponent<SinapsScript>().TypeSinaps == 0) value.GetComponent<LineRenderer>().SetColors(new Color32( 255, 255, 0, 109), new Color32( 255, 255, 0, 109));
				if (value.GetComponent<SinapsScript>().TypeSinaps == 1) value.GetComponent<LineRenderer>().SetColors(new Color32( 0, 255, 255, 109), new Color32( 0, 255, 255, 109));
				if (value.GetComponent<SinapsScript>().TypeSinaps == 2) value.GetComponent<LineRenderer>().SetColors(new Color32( 255, 0, 255, 109), new Color32( 255, 0, 255, 109));
				value.GetComponent<LineRenderer>().SetWidth(0.3f , 0.4f);//Толщина синапса
			}
		}
	}

	void OnGUI()
	{
		//...............................................................................................................................
		if (!AddNeironBool)
		{
			if (GUI.Button (new Rect (210, 10, 80, 20), "Edit")) 
			{
				AddNeironBool = true;
				GizmaNeiron.transform.position = CamTarget.position;
				GizmaNeiron.SetActive(true);
			}
		} else
		{
			if (GUI.Button(new Rect (210, 10, 80, 20), "Add neuron")) 
			{
				Collider[] hitColliders = Physics.OverlapSphere(GizmaNeiron.transform.position, 1f);
				bool ExistNeiron = false;
				foreach (Collider coll in hitColliders){
					if (coll.tag == "Neiron") {
						ExistNeiron = true;
						break;
					}
				}
				if (!ExistNeiron)
				{ 
					NewNeironVoid();
					AddNeironBool = false;
					GizmaNeiron.SetActive(false);
				}
			}
			if (GUI.Button(new Rect(291, 10, 50, 20), "END"))
			{
				AddNeironBool = false;
				GizmaNeiron.SetActive(false);
			}
			selNewGrid = GUI.SelectionGrid(new Rect(210, 31, 130, 20), selNewGrid, selStrings, selStrings.Length);
		}
		//..................................................................................................................................



		if (Selektor != null)
		{
            GUI.Label(new Rect(200, 130, 200, 20), "Charge: " + Selektor.GetComponent<NeironScript>().Charge.ToString());

			foreach (GameObject value in Selektor.GetComponent<NeironScript>().hitSinaps) 
			{
				GameObject valueLevel = value.GetComponent<SinapsScript>().NeironTarget;
				Vector3 screenPosition = Camera.main.WorldToScreenPoint(valueLevel.transform.position);
				Vector3 cameraRelative = Camera.main.transform.InverseTransformPoint(valueLevel.transform.position);
				if (cameraRelative.z > 0)
				{
					Rect position = new Rect(screenPosition.x + 10f, Screen.height - screenPosition.y - 23f, 100f, 20f);
					GUI.Label(position, "" + value.GetComponent<SinapsScript>().Force.ToString("0.0"));
				}
			}


			GUI.Label(new Rect(5, 5, 180, 20), "Index neuron: " + Selektor.GetComponent<NeironScript>().IndexNeiron);
			selGridInt = GUI.SelectionGrid(new Rect(5, 30, 180, 20), selGridInt, selStrings, selStrings.Length);
			if (selGridInt != Selektor.GetComponent<NeironScript>().TypeIndexNeiron) 
				Selektor.GetComponent<NeironScript>().TypeIndexNeiron = selGridInt;

			if (!AddSinapsBool) 
			{
                if (GUI.Button(new Rect(5, 55, 180, 20), "Add synapse")) AddSinapsBool = true;
			} else GUI.Label(new Rect(5, 55, 250, 20), "Select neuron on target synapse..");

			if (Selektor.GetComponent<NeironScript>().Area != null) 
			{
				string AreaButton = "";
				if (Selektor.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().Global) AreaButton = "Clear Global Area";
				else  {
					AreaButton = "Clear Area";
					GUI.Label(new Rect(5, 105, 180, 20), "" + Selektor.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().Name);
				}
				if (GUI.Button(new Rect(5, 80, 180, 20), AreaButton)) {
						Selektor.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().amount--;
						Selektor.GetComponent<NeironScript>().Area = null;
					}
			} else 
			{
				if (GUI.Button(new Rect(5, 80, 180, 20), "Add Global Area")) {
						Selektor.GetComponent<NeironScript>().Area = GlobalArea;
						GlobalArea.GetComponent<AreaScript>().amount++;
					}
				if (GUI.Button(new Rect(5, 105, 180, 20), "Add Included Area"))  {
					foreach (GameObject AreaBouns in gameObject.GetComponent<ManagerArea>().hitArea){
						if (AreaBouns.GetComponent<Collider>().bounds.Contains(Selektor.transform.position)) {
							Selektor.GetComponent<NeironScript>().Area = AreaBouns;
							AreaBouns.GetComponent<AreaScript>().amount++;
							break;
						}
						if (Selektor.GetComponent<NeironScript>().Area == null) Selektor.GetComponent<NeironScript>().Area = GlobalArea;
					}
				}
			}

			GUILayout.BeginArea(new Rect(5, 130, 180, Screen.height-160), GUI.skin.box);
			_inventoryScroll = GUILayout.BeginScrollView(_inventoryScroll, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));

            GUILayout.Label("Adder:  " + Selektor.GetComponent<NeironScript>().Adder);
            GUILayout.Label("Max adder: ");
			MaxAdderString = GUILayout.TextField(MaxAdderString);
            GUILayout.Label("Damper: ");
			DampferAdderString = GUILayout.TextField(DampferAdderString);
            GUILayout.Label("Upper threshold:");
			thresholdTopString = GUILayout.TextField(thresholdTopString);
            GUILayout.Label("Response time: ");
			AnswerTimeString = GUILayout.TextField(AnswerTimeString);
            GUILayout.Label("Time relax: ");
			TimeReposeString = GUILayout.TextField(TimeReposeString);

			if (Selektor.GetComponent<NeironScript>().TypeIndexNeiron > 0){
				GUILayout.Label("____________________");
				GUILayout.Label("t.Top + t.Bonus (t.Bonus)");
				float t = Selektor.GetComponent<NeironScript>().thresholdTop + Selektor.GetComponent<NeironScript>().bonusThreshold;
				GUILayout.Label(" " + t + "  (" + Selektor.GetComponent<NeironScript>().bonusThreshold + ")");
                GUILayout.Label("Lower threshold");
				thresholdDownString = GUILayout.TextField(thresholdDownString);
                GUILayout.Label("Negative time");
				timeIgnoreString = GUILayout.TextField(timeIgnoreString);
                GUILayout.Label("Damper threshold");
				DempferBonusThresholdString = GUILayout.TextField(DempferBonusThresholdString);
                GUILayout.Label("Valuation time");
				TimeEvaluationString = GUILayout.TextField(TimeEvaluationString);
                GUILayout.Label("Limit repeats");
				LimitRecurrenceString = GUILayout.TextField(LimitRecurrenceString);
                GUILayout.Label("Increasing threshold");
				thresholdTopUpString = GUILayout.TextField(thresholdTopUpString);
                GUILayout.Label("Adaptation time");
				AdaptationTimeString = GUILayout.TextField(AdaptationTimeString);
                GUILayout.Label("Basic threshold");
				thresholdAdaptString = GUILayout.TextField(thresholdAdaptString);
			}

			if (Selektor.GetComponent<NeironScript>().TypeIndexNeiron == 2){
				GUILayout.Label("____________________");
                GUILayout.Label("Max force synapse:");
				MaxForceSinapsString = GUILayout.TextField(MaxForceSinapsString);
                GUILayout.Label("Speed:");
				TimeChargeString = GUILayout.TextField(TimeChargeString);
                FocusDinamicSetting = GUILayout.Toggle(FocusDinamicSetting, "Focus Dynamics");
				if (FocusDinamicSetting){
                    GUILayout.Label("Focus: " + Selektor.GetComponent<NeironScript>().FocusNeiron.ToString());
                    GUILayout.Label("Max focus: ");
					MaxFocusNeironString = GUILayout.TextField(MaxFocusNeironString);
                    GUILayout.Label("Focus step:");
					StepFocusString = GUILayout.TextField(StepFocusString);
				}  else {
                    GUILayout.Label("Focus:");
					FocusNeironString = GUILayout.TextField(FocusNeironString);
				}

                PlasticityDinamicSetting = GUILayout.Toggle(PlasticityDinamicSetting, "Plastic Dynamics");
				if (PlasticityDinamicSetting){
                    GUILayout.Label("Plastic: " + Selektor.GetComponent<NeironScript>().Plasticity.ToString());
                    GUILayout.Label("Basic Plastic:");
					BasicPlasticityString = GUILayout.TextField(BasicPlasticityString);
                    GUILayout.Label("Plastic step:");
					StepPlasticityString = GUILayout.TextField(StepPlasticityString);
				} else {
                    GUILayout.Label("Plastic:");
					PlasticityString = GUILayout.TextField(PlasticityString);
                    GUILayout.Label("Basic Plastic:");
					BasicPlasticityString = GUILayout.TextField(BasicPlasticityString);
				}

                NewNeironDinamicSetting = GUILayout.Toggle(NewNeironDinamicSetting, "Dynamic net");
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();

			if (GUI.Button(new Rect(150, Screen.height - 25, 30, 20), "OK")) 
			{
				Selektor.GetComponent<NeironScript>().MaxAdder = float.Parse(MaxAdderString);

				Selektor.GetComponent<NeironScript>().DampferAdder = float.Parse(DampferAdderString);
				Selektor.GetComponent<NeironScript>().thresholdTop = float.Parse(thresholdTopString);
				Selektor.GetComponent<NeironScript>().AnswerTime = float.Parse(AnswerTimeString);
				Selektor.GetComponent<NeironScript>().TimeRepose = float.Parse(TimeReposeString);

				Selektor.GetComponent<NeironScript>().thresholdDown = float.Parse(thresholdDownString);
				Selektor.GetComponent<NeironScript>().timeIgnore = float.Parse(timeIgnoreString);
				Selektor.GetComponent<NeironScript>().DempferBonusThreshold = float.Parse(DempferBonusThresholdString);
				Selektor.GetComponent<NeironScript>().TimeEvaluation = float.Parse(TimeEvaluationString);
				Selektor.GetComponent<NeironScript>().LimitRecurrence = int.Parse(LimitRecurrenceString);
				Selektor.GetComponent<NeironScript>().thresholdTopUp = float.Parse(thresholdTopUpString);

				Selektor.GetComponent<NeironScript>().AdaptationTime = float.Parse(AdaptationTimeString);
				Selektor.GetComponent<NeironScript>().thresholdAdapt = float.Parse(thresholdAdaptString);

				Selektor.GetComponent<NeironScript>().MaxForceSinaps = float.Parse(MaxForceSinapsString);
				Selektor.GetComponent<NeironScript>().TimeCharge = float.Parse(TimeChargeString);
				Selektor.GetComponent<NeironScript>().FocusNeiron = float.Parse(FocusNeironString);
				Selektor.GetComponent<NeironScript>().MaxFocus = float.Parse(MaxFocusNeironString);
				Selektor.GetComponent<NeironScript>().StepFocus = float.Parse(StepFocusString);

				Selektor.GetComponent<NeironScript>().Plasticity = float.Parse(PlasticityString);
				Selektor.GetComponent<NeironScript>().BasicPlasticity = float.Parse(BasicPlasticityString);
				Selektor.GetComponent<NeironScript>().StepPlasticity = float.Parse(StepPlasticityString);

				Selektor.GetComponent<NeironScript>().FocusDinamic = FocusDinamicSetting;
				Selektor.GetComponent<NeironScript>().PlasticityDinamic = PlasticityDinamicSetting;
				Selektor.GetComponent<NeironScript>().NewNeironDinamic = NewNeironDinamicSetting;

			}

			if (GUI.Button(new Rect( 5, Screen.height - 25, 100, 20), "Delete neuron")) DeleteNeiron();


		}
		
		if (WindowS) 
		{
			WindowsRect = GUI.Window(0, WindowsRect, DoMyWindow, ""); 
		}
	} 

	private void NewNeironVoid()
	{
		GameObject clone = (GameObject) Instantiate(prefabNeiron, GizmaNeiron.transform.position, prefabNeiron.transform.rotation);
		foreach (GameObject AreaBouns in gameObject.GetComponent<ManagerArea>().hitArea){
			if (AreaBouns.GetComponent<Collider>().bounds.Contains(clone.transform.position)) {
				clone.GetComponent<NeironScript>().Area = AreaBouns;
				AreaBouns.GetComponent<AreaScript>().amount++;
				break;
			}
		}
		if (clone.GetComponent<NeironScript>().Area == null) clone.GetComponent<NeironScript> ().Area = GlobalArea; 
		clone.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().amount++;
		clone.GetComponent<NeironScript>().TypeIndexNeiron = selNewGrid;
		EndIndexNeiron++;
		clone.GetComponent<NeironScript>().IndexNeiron = EndIndexNeiron;
		clone.name = "Neiron" + EndIndexNeiron;
	}


	
	private void AddSinaps (GameObject targetS)
	{
		bool Conten = false;
		foreach (GameObject value in Selektor.GetComponent<NeironScript> ().hitSinaps) 
		{
			if (value.GetComponent<SinapsScript>().NeironTarget == targetS) Conten = true;
		}
		if (!Conten)
		{
			GameObject cSinaps = Instantiate (prefabSinaps, Selektor.transform.position, transform.rotation) as GameObject;
			cSinaps.GetComponent<SinapsScript> ().NeironTarget = targetS;
			cSinaps.GetComponent<SinapsScript> ().Force = 0.0f;
			cSinaps.transform.parent = Selektor.transform;
			Selektor.GetComponent<NeironScript> ().hitSinaps.Add (cSinaps);
			AddSinapsBool = false;
		}
	}

	private void SinapsEditFind(GameObject SinapsTarget)
	{
		if (Selektor != null)
		{
			foreach (GameObject value in Selektor.GetComponent<NeironScript>().hitSinaps) 
			{
				if (value.GetComponent<SinapsScript>().NeironTarget == SinapsTarget)
				{
					SinapsEdit = value;
					StringWindows = "" + SinapsEdit.GetComponent<SinapsScript>().Force;
					WindowS = true;
					break;
				} 
			}
		}
	}

	private void DeleteNeiron () //Кнопка Delete Neiron!!
	{
		if (Selektor.GetComponent<NeironScript>().Area != null) Selektor.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().amount--;
		GameObject SS = Selektor; 
		Selektor = null; 
		GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron");
		
		foreach (GameObject value in OjectNeiron) 
		{
			foreach (GameObject val in value.GetComponent<NeironScript>().hitSinaps)
			{
				if (val.GetComponent<SinapsScript>().NeironTarget == SS) 
				{
					value.GetComponent<NeironScript>().hitSinaps.Remove(val);
					Destroy(val);
					break;
				}
			}
		}
		
		Destroy (SS);
	}	

	void DoMyWindow(int windowID) 
	{
		StringWindows = GUI.TextField (new Rect (5, 5, 145, 20), StringWindows);
		if (GUI.Button (new Rect (150, 5, 30, 20), "OK")) 
		{
			float tryFloat;
			if (float.TryParse(StringWindows, out tryFloat))
			{
				SinapsEdit.GetComponent<SinapsScript>().Force = tryFloat;
				SelektNeironOn(Selektor);
				WindowS = false;
			}
			SinapsEdit.GetComponent<SinapsScript>().TypeSinaps = selSinapsInt;
		}
		if (GUI.Button (new Rect (180, 5, 20, 20), "x")) 
		{
			WindowS = false;
		}
		selSinapsInt = GUI.SelectionGrid(new Rect(5, 30, 192, 20), selSinapsInt, selSinaps, selSinaps.Length);

		GUI.DragWindow ();
	}
}
