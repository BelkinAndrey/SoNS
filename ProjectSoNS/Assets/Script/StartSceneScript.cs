using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
/* Сохранение и загрузка в фаил 
 * используется флрмат файла .ini */
public class StartSceneScript : MonoBehaviour { 

	public string PathFile;  //путь к файлу
	
	public GameObject _prefabNeiron; //префабы размещаемых обьектов при загрузке
	public GameObject _prefabSinaps;

	public GameObject _prefabAreaBall;
	public GameObject _prefabAeaBox;
	public GameObject ManagerOject; //Ссылки на обьекты в новой сцене
	
	public GameObject EntryOject;
	public GameObject OutputOject;
	
	private List<GameObject> NewGameOjectList = new List<GameObject>(); //Списки обьектов
	private List<GameObject> NewLidtArea = new List<GameObject>();
	
	private GameObject BuferPrefabArea; 

	private int MaxIndex = 0;

    IEnumerator StartLoad() // Костыль: необходио немного подождать загрузки сцены, что бы размещать обьеты функцией LoadNet() 
	{
		yield return new WaitForSeconds(0.2f);
		LoadNet(); 
	}

    /* при старете сцены StartScene сразу загружаем сцену One.
     * Этот обьект не удаляем и поэтому Awake() выполняется только один раз.
     * Каждый раз при загрузке файла перезагружаем сцену One соотвественно удаляем все размещённые обьекты, своего рода очистка сцены.
     * Градиент фона: http://wiki.unity3d.com/index.php?title=CameraGradientBackground */
	void Awake () {  
		DontDestroyOnLoad(gameObject); // при загрузке новых сцен сохраняем этот обьект
		DontDestroyOnLoad(GameObject.Find("Target")); // и этот
		DontDestroyOnLoad(GameObject.Find("Gradient Cam")); //необходим для красивого градиента 
		DontDestroyOnLoad(GameObject.Find("Gradient Plane")); // и это
		Application.LoadLevel(1); //загружаем сцену One

	}

	void Start () {

        if (PlayerPrefs.HasKey("Path")) PathFile = PlayerPrefs.GetString("Path"); // PlayerPrefs для пути к файлу.
		
		LoadNet (); //загрузка 
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (Screen.width - 452, 0, 50, 20), "New")) Application.LoadLevel(1); //Кнопка New просто загружает сцену One

        PathFile = GUI.TextField(new Rect(Screen.width - 300, 0, 300, 20), PathFile); //  записываем в сторку имя файла из PlayerPrefs

        if (GUI.Button(new Rect(Screen.width - 350, 0, 50, 20), "Save")) SaveNet(); // кнопка Save

		if (GUI.Button (new Rect (Screen.width - 401, 0, 50, 20), "Open"))  // кнопка Open
		{
			if (File.Exists (Application.dataPath + "/Data/" + PathFile)){ 
				Application.LoadLevel(1); // загрузка сцены One
                StartCoroutine("StartLoad"); // Костыль: необходио немного подождать загрузки сцены, что бы размещать обьеты функцией LoadNet() 
			}
		}

	}

	public void SaveNet() //Сохранение
	{
        ManagerOject = GameObject.Find("Manager"); // Ссылка на Manager - важный обьект в сцене.

		ManagerOject.GetComponent<ManagerScript> ().Selektor = null; // Очищаем селектор, так как селектор меняет теги обьектов, нам нужны исходные теги.
        GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron"); // Список обьектов с тегом Neiron
        GameObject[] ListArea = GameObject.FindGameObjectsWithTag("Area"); // Список обьектов с тегом Area


        PlayerPrefs.SetString("Path", PathFile);  // Сохраняем в PlayerPrefs имя файла
		
		XmlDocument xmlDoc = new XmlDocument(); //создаем новый документ xml
        XmlNode rootNode = xmlDoc.CreateElement("NetSave"); // Главный раздел <NetSave>...
		xmlDoc.AppendChild(rootNode); // добавляем главный раздел 
		
		XmlNode userNode; 
		XmlAttribute attribute;

		XmlNode userS;
		XmlAttribute attributeS;

		//Сохранение глобальной зоны
        userNode = xmlDoc.CreateElement("AreaGlobal"); // <AreaGlobal>
		for (int n = 0; n < 16; n++){
			attribute = xmlDoc.CreateAttribute("P" + n);
			attribute.Value = ManagerOject.GetComponent<ManagerArea>().GlobalArea.GetComponent<AreaScript>().Spike1[n].ToString(); //настройка закона изменения заряда в позитиве
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("N" + n);
            attribute.Value = ManagerOject.GetComponent<ManagerArea>().GlobalArea.GetComponent<AreaScript>().Spike2[n].ToString(); //настройка закона изменения заряда в негативе
			userNode.Attributes.Append(attribute);
		}
        rootNode.AppendChild(userNode); // </AreaGlobal>

		//Сохранение зон
		for (int i = 0; i < ListArea.Length; i++) {
			userNode = xmlDoc.CreateElement("Area");               	//Раздел Area
			attribute = xmlDoc.CreateAttribute("index");        	//Атрибут index
			attribute.Value = i.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("Name");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().Name;
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("Ball");
			if (ListArea[i].GetComponent<AreaScript>().BallBool) attribute.Value = "+";
			else attribute.Value = "-";
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("x");             //Атрибут x
			attribute.Value = ListArea[i].transform.position.x.ToString();
			userNode.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("y");             //Атрибут y
			attribute.Value = ListArea[i].transform.position.y.ToString();
			userNode.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("z");             //Атрибут z
			attribute.Value = ListArea[i].transform.position.z.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("S");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().S.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("H");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().H.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("L");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().L.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("R");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().R.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("ColorR");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().ColorR.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("ColorG");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().ColorG.ToString();
			userNode.Attributes.Append(attribute);
		
			attribute = xmlDoc.CreateAttribute("ColorB");           
			attribute.Value = ListArea[i].GetComponent<AreaScript>().ColorB.ToString();
			userNode.Attributes.Append(attribute);

			userS = xmlDoc.CreateElement("Spaik");
			for (int s = 0; s < 16; s++){
					attributeS = xmlDoc.CreateAttribute("P" + s);
					attributeS.Value = ListArea[i].GetComponent<AreaScript>().Spike1[s].ToString();
					userS.Attributes.Append(attributeS);

					attributeS = xmlDoc.CreateAttribute("N" + s);
					attributeS.Value = ListArea[i].GetComponent<AreaScript>().Spike2[s].ToString();
					userS.Attributes.Append(attributeS);
				}
			userNode.AppendChild(userS);

			rootNode.AppendChild(userNode);
		}

		//////////////////////////////////////////////////////////////////////////////////
		for (int i = 0; i < OjectNeiron.Length; i++) 
		{
			userNode = xmlDoc.CreateElement("N");               //Раздел N
			attribute = xmlDoc.CreateAttribute("index");        //Атрибут index
			attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().IndexNeiron.ToString();
			userNode.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("x");             //Атрибут x
			attribute.Value = OjectNeiron[i].transform.position.x.ToString();
			userNode.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("y");             //Атрибут y
			attribute.Value = OjectNeiron[i].transform.position.y.ToString();
			userNode.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("z");             //Атрибут z
			attribute.Value = OjectNeiron[i].transform.position.z.ToString();
			userNode.Attributes.Append(attribute);

			attribute = xmlDoc.CreateAttribute("type");           //Атрибут type
			attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().TypeIndexNeiron.ToString();
			userNode.Attributes.Append(attribute);


			/////////Сохранение настроек нейрона
			if (OjectNeiron[i].GetComponent<NeironScript>().MaxAdder != 30) {
				attribute = xmlDoc.CreateAttribute("MaxAdder");         
				attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().MaxAdder.ToString();
				userNode.Attributes.Append(attribute);
			}

			if (OjectNeiron[i].GetComponent<NeironScript>().DampferAdder != 1) {
				attribute = xmlDoc.CreateAttribute("DampferAdder");         
				attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().DampferAdder.ToString();
				userNode.Attributes.Append(attribute);
			}
			
			if (OjectNeiron[i].GetComponent<NeironScript>().thresholdTop != 1) {
				attribute = xmlDoc.CreateAttribute("thresholdTop");         
				attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().thresholdTop.ToString();
				userNode.Attributes.Append(attribute);
			}

			if (OjectNeiron[i].GetComponent<NeironScript>().AnswerTime != 0.05f) {
				attribute = xmlDoc.CreateAttribute("AnswerTime");         
				attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().AnswerTime.ToString();
				userNode.Attributes.Append(attribute);
			}

			if (OjectNeiron[i].GetComponent<NeironScript>().TimeRepose != 0) {
				attribute = xmlDoc.CreateAttribute("TimeRepose");         
				attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().TimeRepose.ToString();
				userNode.Attributes.Append(attribute);
			}

			if (OjectNeiron[i].GetComponent<NeironScript>().TypeIndexNeiron > 0){
				if (OjectNeiron[i].GetComponent<NeironScript>().thresholdDown != -5) {
					attribute = xmlDoc.CreateAttribute("thresholdDown");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().thresholdDown.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().timeIgnore != 5) {
					attribute = xmlDoc.CreateAttribute("timeIgnore");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().timeIgnore.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().DempferBonusThreshold != 0.005f) {
					attribute = xmlDoc.CreateAttribute("DempferBonusThreshold");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().DempferBonusThreshold.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().TimeEvaluation != 2) {
					attribute = xmlDoc.CreateAttribute("TimeEvaluation");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().TimeEvaluation.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().LimitRecurrence != 2) {
					attribute = xmlDoc.CreateAttribute("LimitRecurrence");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().LimitRecurrence.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().thresholdTopUp != 1) {
					attribute = xmlDoc.CreateAttribute("thresholdTopUp");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().thresholdTopUp.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().AdaptationTime != 2) {
					attribute = xmlDoc.CreateAttribute("AdaptationTime");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().AdaptationTime.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().thresholdAdapt != 1) {
					attribute = xmlDoc.CreateAttribute("thresholdAdapt");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().thresholdAdapt.ToString();
					userNode.Attributes.Append(attribute);
				}				
			}

			if (OjectNeiron[i].GetComponent<NeironScript>().TypeIndexNeiron == 2){
				if (OjectNeiron[i].GetComponent<NeironScript>().MaxForceSinaps != 10) {
					attribute = xmlDoc.CreateAttribute("MaxForceSinaps");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().MaxForceSinaps.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().TimeCharge != 0.01f) {
					attribute = xmlDoc.CreateAttribute("TimeCharge");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().TimeCharge.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().FocusDinamic != true) {
					attribute = xmlDoc.CreateAttribute("FocusDinamic");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().FocusDinamic.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().MaxFocus != 45) {
					attribute = xmlDoc.CreateAttribute("MaxFocus");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().MaxFocus.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().StepFocus != 1) {
					attribute = xmlDoc.CreateAttribute("StepFocus");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().StepFocus.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().PlasticityDinamic != false) {
					attribute = xmlDoc.CreateAttribute("PlasticityDinamic");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().PlasticityDinamic.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().Plasticity != 0.5f) {
					attribute = xmlDoc.CreateAttribute("Plasticity");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().Plasticity.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().BasicPlasticity != 0.5f) {
					attribute = xmlDoc.CreateAttribute("BasicPlasticity");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().BasicPlasticity.ToString();
					userNode.Attributes.Append(attribute);
				}

				if (OjectNeiron[i].GetComponent<NeironScript>().NewNeironDinamic != true) {
					attribute = xmlDoc.CreateAttribute("NewNeironDinamic");         
					attribute.Value = OjectNeiron[i].GetComponent<NeironScript>().NewNeironDinamic.ToString();
					userNode.Attributes.Append(attribute);
				}
			}

			////END Настроек нейрона
			attribute = xmlDoc.CreateAttribute("IndexArea");           //Атрибут type
			if (OjectNeiron[i].GetComponent<NeironScript>().Area == null) attribute.Value = "-2";
			else {
				if (OjectNeiron[i].GetComponent<NeironScript>().Area.GetComponent<AreaScript>().Global) attribute.Value = "-1";
				else {
					string FindArea = "-2";
					for (int j = 0; j < ListArea.Length; j++){
						if (ListArea[j] == OjectNeiron[i].GetComponent<NeironScript>().Area) {
							FindArea = j.ToString();
							break;
						}
					}
					attribute.Value = FindArea;
				}
			}
			userNode.Attributes.Append(attribute);

			
			foreach (GameObject value in OjectNeiron[i].GetComponent<NeironScript>().hitSinaps)
			{
				userS = xmlDoc.CreateElement("S");

				attributeS = xmlDoc.CreateAttribute("target");
				attributeS.Value = value.GetComponent<SinapsScript>().NeironTarget.GetComponent<NeironScript>().IndexNeiron.ToString();
				userS.Attributes.Append(attributeS);
				
				attributeS = xmlDoc.CreateAttribute("type");
				attributeS.Value = value.GetComponent<SinapsScript>().TypeSinaps.ToString();
				userS.Attributes.Append(attributeS);

				attributeS = xmlDoc.CreateAttribute("force");
				attributeS.Value = value.GetComponent<SinapsScript>().Force.ToString();
				userS.Attributes.Append(attributeS);

				userNode.AppendChild(userS);
			}
			rootNode.AppendChild(userNode);
		}

		///////////////Запись скриптов 
		userNode = xmlDoc.CreateElement("Scripts");
		foreach (ManagerScripting.ScriptNet value in ManagerOject.GetComponent<ManagerScripting>().ListScript) 
		{
			userS = xmlDoc.CreateElement("Script");

			attributeS = xmlDoc.CreateAttribute("TypeActivator");
			attributeS.Value = value.TypeActivator.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("TypeAct");
			attributeS.Value = value.TypeAct.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("ANeiron");
			if (value.ANeiron == null) attributeS.Value = "0"; else attributeS.Value = value.ANeiron.GetComponent<NeironScript>().IndexNeiron.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("AArea");
			if (value.AArea == null) attributeS.Value = "-2";
			else {
				if (value.AArea.GetComponent<AreaScript>().Global) attributeS.Value = "-1";
				else {
					string FindAArea = "-2";
					for (int n = 0; n < ListArea.Length; n++){
						if (ListArea[n] == value.AArea) {
							FindAArea = "" + n;
							break;
						}
					}
					attributeS.Value = FindAArea;
				}
			}
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("threshold");
			attributeS.Value = value.threshold.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("controller");
			attributeS.Value = value.controller.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("delay");
			attributeS.Value = value.delay.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("delayStep");
			attributeS.Value = value.delayStep.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("hunger");
			attributeS.Value = value.hunger.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("ActNeiron");
			if (value.ActNeiron == null) attributeS.Value = "0"; else attributeS.Value = value.ActNeiron.GetComponent<NeironScript>().IndexNeiron.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("ActArea");
			if (value.ActArea == null) attributeS.Value = "-2";
			else {
				if (value.ActArea.GetComponent<AreaScript>().Global) attributeS.Value = "-1";
				else {
					string FindAArea = "-2";
					for (int n = 0; n < ListArea.Length; n++){
						if (ListArea[n] == value.ActArea) {
							FindAArea = "" + n;
							break;
						}
					}
					attributeS.Value = FindAArea;
				}
			}
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("ActMod");
			attributeS.Value = value.ActMod.ToString();
			userS.Attributes.Append(attributeS);

			attributeS = xmlDoc.CreateAttribute("TimeMod");
			attributeS.Value = value.TimeMod.ToString();
			userS.Attributes.Append(attributeS);

			userNode.AppendChild(userS);
		}
		rootNode.AppendChild(userNode);

		///////////////Запись ввода
		EntryOject = GameObject.Find("BlokEnter");
		OutputOject = GameObject.Find("BlokExit"); 
		userNode = xmlDoc.CreateElement("Entry"); 
		for (int i = 0; i < EntryOject.transform.childCount; i++) 
		{
			if (EntryOject.transform.GetChild(i).GetComponent<OutScript>().NeironTarget == null) continue;

			userS = xmlDoc.CreateElement("Element");

			attribute = xmlDoc.CreateAttribute("Index"); 
			attribute.Value = i.ToString();
			userS.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("target");     
			attribute.Value = EntryOject.transform.GetChild(i).GetComponent<OutScript>().NeironTarget.GetComponent<NeironScript>().IndexNeiron.ToString();
			userS.Attributes.Append(attribute);

			userNode.AppendChild(userS);
		}
		rootNode.AppendChild(userNode);

        //////////////Запись ввода блока
        EntryOject = GameObject.Find("Eye");

        userNode = xmlDoc.CreateElement("EntryBlock");
        for (int i = 0; i < EntryOject.transform.childCount; i++)
        {
            if (EntryOject.transform.GetChild(i).GetComponent<LineNumEye>())
            {
                if (EntryOject.transform.GetChild(i).GetComponent<LineNumEye>().NeironTarget == null) continue;

                userS = xmlDoc.CreateElement("Element");

                attribute = xmlDoc.CreateAttribute("Index");
                attribute.Value = i.ToString();
                userS.Attributes.Append(attribute);

                attribute = xmlDoc.CreateAttribute("target");
                attribute.Value = EntryOject.transform.GetChild(i).GetComponent<LineNumEye>().NeironTarget.GetComponent<NeironScript>().IndexNeiron.ToString();
                userS.Attributes.Append(attribute);

                userNode.AppendChild(userS);
            }
        }
        rootNode.AppendChild(userNode);

		//////////////Запись вывода

		userNode = xmlDoc.CreateElement("output"); 
		for (int i = 0; i < OutputOject.transform.childCount; i++) 
		{
			if (OutputOject.transform.GetChild(i).GetComponent<LineNum>().NeironTarget == null) continue;
			
			userS = xmlDoc.CreateElement("Element");
			
			attribute = xmlDoc.CreateAttribute("Index"); 
			attribute.Value = i.ToString();
			userS.Attributes.Append(attribute);
			
			attribute = xmlDoc.CreateAttribute("target");     
			attribute.Value = OutputOject.transform.GetChild(i).GetComponent<LineNum>().NeironTarget.GetComponent<NeironScript>().IndexNeiron.ToString();
			userS.Attributes.Append(attribute);
			
			userNode.AppendChild(userS);
		}
        rootNode.AppendChild(userNode); // </NetSave>

		xmlDoc.Save(Application.dataPath + "/Data/" + PathFile); // Сохранение в файл

	}

	public void LoadNet() //Загрузка
	{
		if (File.Exists (Application.dataPath + "/Data/" + PathFile)) //Если существует файл иначе ничего не делаем
		{

			MaxIndex = 0; //Счётчик нейроэлемментов

            PlayerPrefs.SetString("Path", PathFile); //сохраняем в PlayerPrefs имя файла 
            ManagerOject = GameObject.Find("Manager"); // ищем обьект Manager во вновь открытой сцене One 

			NewGameOjectList.Clear(); //Очищаем списки перед использованием
			NewLidtArea.Clear();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(Application.dataPath + "/Data/" + PathFile); //открываем файл

			//Загрузка глобально зоны
			XmlNode nodeGlobal = xmlDoc.SelectSingleNode("NetSave/AreaGlobal");   
			for (int n = 0; n < 16; n++){
				ManagerOject.GetComponent<ManagerArea>().GlobalArea.GetComponent<AreaScript>().Spike1[n] = float.Parse(nodeGlobal.Attributes.GetNamedItem("P" + n).Value);
				ManagerOject.GetComponent<ManagerArea>().GlobalArea.GetComponent<AreaScript>().Spike2[n] = float.Parse(nodeGlobal.Attributes.GetNamedItem("N" + n).Value);
			}

			//Загрузка зон
			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/Area")){
				Vector3 positionArea = new Vector3(float.Parse(node.Attributes.GetNamedItem("x").Value),
				                                   float.Parse(node.Attributes.GetNamedItem("y").Value),
				                                   float.Parse(node.Attributes.GetNamedItem("z").Value));

				if (node.Attributes.GetNamedItem("Ball").Value == "+") BuferPrefabArea = _prefabAreaBall;
				else BuferPrefabArea = _prefabAeaBox;

				GameObject clonArea = Instantiate(BuferPrefabArea, positionArea, _prefabAreaBall.transform.rotation) as GameObject;
				clonArea.GetComponent<AreaScript>().Name = node.Attributes.GetNamedItem("Name").Value;
				clonArea.GetComponent<AreaScript>().R = float.Parse(node.Attributes.GetNamedItem("R").Value);
				clonArea.GetComponent<AreaScript>().S = float.Parse(node.Attributes.GetNamedItem("S").Value);
				clonArea.GetComponent<AreaScript>().H = float.Parse(node.Attributes.GetNamedItem("H").Value);
				clonArea.GetComponent<AreaScript>().L = float.Parse(node.Attributes.GetNamedItem("L").Value);
				clonArea.GetComponent<AreaScript>().ColorR = float.Parse(node.Attributes.GetNamedItem("ColorR").Value);
				clonArea.GetComponent<AreaScript>().ColorG = float.Parse(node.Attributes.GetNamedItem("ColorG").Value);
				clonArea.GetComponent<AreaScript>().ColorB = float.Parse(node.Attributes.GetNamedItem("ColorB").Value);
				if (node.Attributes.GetNamedItem("Ball").Value == "+") {
					clonArea.transform.localScale = new Vector3(clonArea.GetComponent<AreaScript>().R*2, clonArea.GetComponent<AreaScript>().R*2, clonArea.GetComponent<AreaScript>().R*2);
				} else {
					clonArea.transform.localScale = new Vector3(clonArea.GetComponent<AreaScript>().S, clonArea.GetComponent<AreaScript>().H, clonArea.GetComponent<AreaScript>().L);
				}
				clonArea.SendMessage("Start");
				clonArea.name = "Area(" + node.Attributes.GetNamedItem("Name").Value + ")";

                XmlNode AreaSetting = node.SelectSingleNode("Spaik");
				for (int g = 0; g < 16; g++) 
								{
									clonArea.GetComponent<AreaScript>().Spike1[g] = float.Parse(AreaSetting.Attributes.GetNamedItem("P" + g).Value);
									clonArea.GetComponent<AreaScript>().Spike2[g] = float.Parse(AreaSetting.Attributes.GetNamedItem("N" + g).Value);
								}				

				NewLidtArea.Add(clonArea);
				ManagerOject.GetComponent<ManagerArea>().hitArea.Add(clonArea);

			}

			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/N"))
			{
				Vector3 positionNew = new Vector3(float.Parse(node.Attributes.GetNamedItem("x").Value), 
				                                  float.Parse(node.Attributes.GetNamedItem("y").Value), 
				                                  float.Parse(node.Attributes.GetNamedItem("z").Value));

				GameObject clone = Instantiate(_prefabNeiron, positionNew, _prefabNeiron.transform.rotation) as GameObject;

				clone.GetComponent<NeironScript>().TypeIndexNeiron = int.Parse(node.Attributes.GetNamedItem("type").Value);
				int indexTemp = int.Parse(node.Attributes.GetNamedItem("index").Value);
				clone.GetComponent<NeironScript>().IndexNeiron = indexTemp;
				if (indexTemp > MaxIndex) MaxIndex = indexTemp;
				int FA = int.Parse(node.Attributes.GetNamedItem("IndexArea").Value);
				if (FA == -1) clone.GetComponent<NeironScript>().Area = ManagerOject.GetComponent<ManagerArea>().GlobalArea;
				if (FA >= 0) clone.GetComponent<NeironScript>().Area = NewLidtArea[FA];
				if (clone.GetComponent<NeironScript>().Area != null) clone.GetComponent<NeironScript>().Area.GetComponent<AreaScript>().amount++;
				clone.name = "Neiron" + indexTemp; //////////////////////////////NEURON NEURON NEURON NEURON NEURON NEURON NEURON NEURON NEURON NEURON  
				
				//Загрузка настроек нейрона ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

				if (node.Attributes["MaxAdder"] != null) clone.GetComponent<NeironScript>().MaxAdder = float.Parse(node.Attributes.GetNamedItem("MaxAdder").Value);
				if (node.Attributes["DampferAdder"] != null) clone.GetComponent<NeironScript>().DampferAdder = float.Parse(node.Attributes.GetNamedItem("DampferAdder").Value);
				if (node.Attributes["thresholdTop"] != null) clone.GetComponent<NeironScript>().thresholdTop = float.Parse(node.Attributes.GetNamedItem("thresholdTop").Value);
				if (node.Attributes["AnswerTime"] != null) clone.GetComponent<NeironScript>().AnswerTime = float.Parse(node.Attributes.GetNamedItem("AnswerTime").Value);
				if (node.Attributes["TimeRepose"] != null) clone.GetComponent<NeironScript>().TimeRepose = float.Parse(node.Attributes.GetNamedItem("TimeRepose").Value);
				if (node.Attributes["thresholdDown"] != null) clone.GetComponent<NeironScript>().thresholdDown = float.Parse(node.Attributes.GetNamedItem("thresholdDown").Value);
				if (node.Attributes["timeIgnore"] != null) clone.GetComponent<NeironScript>().timeIgnore = float.Parse(node.Attributes.GetNamedItem("timeIgnore").Value);
				if (node.Attributes["DempferBonusThreshold"] != null) clone.GetComponent<NeironScript>().DempferBonusThreshold = float.Parse(node.Attributes.GetNamedItem("DempferBonusThreshold").Value);
				if (node.Attributes["TimeEvaluation"] != null) clone.GetComponent<NeironScript>().TimeEvaluation = float.Parse(node.Attributes.GetNamedItem("TimeEvaluation").Value);
				if (node.Attributes["LimitRecurrence"] != null) clone.GetComponent<NeironScript>().LimitRecurrence = int.Parse(node.Attributes.GetNamedItem("LimitRecurrence").Value);
				if (node.Attributes["thresholdTopUp"] != null) clone.GetComponent<NeironScript>().thresholdTopUp = float.Parse(node.Attributes.GetNamedItem("thresholdTopUp").Value);
				if (node.Attributes["AdaptationTime"] != null) clone.GetComponent<NeironScript>().AdaptationTime = float.Parse(node.Attributes.GetNamedItem("AdaptationTime").Value);
				if (node.Attributes["thresholdAdapt"] != null) clone.GetComponent<NeironScript>().thresholdAdapt = float.Parse(node.Attributes.GetNamedItem("thresholdAdapt").Value);
				if (node.Attributes["MaxForceSinaps"] != null) clone.GetComponent<NeironScript>().MaxForceSinaps = float.Parse(node.Attributes.GetNamedItem("MaxForceSinaps").Value);
				if (node.Attributes["TimeCharge"] != null) clone.GetComponent<NeironScript>().TimeCharge = float.Parse(node.Attributes.GetNamedItem("TimeCharge").Value);
				if (node.Attributes["FocusDinamic"] != null) clone.GetComponent<NeironScript>().FocusDinamic = bool.Parse(node.Attributes.GetNamedItem("FocusDinamic").Value);
				if (node.Attributes["MaxFocus"] != null) clone.GetComponent<NeironScript>().MaxFocus = float.Parse(node.Attributes.GetNamedItem("MaxFocus").Value);
				if (node.Attributes["StepFocus"] != null) clone.GetComponent<NeironScript>().StepFocus = float.Parse(node.Attributes.GetNamedItem("StepFocus").Value);
				if (node.Attributes["PlasticityDinamic"] != null) clone.GetComponent<NeironScript>().PlasticityDinamic = bool.Parse(node.Attributes.GetNamedItem("PlasticityDinamic").Value);
				if (node.Attributes["Plasticity"] != null) clone.GetComponent<NeironScript>().Plasticity = float.Parse(node.Attributes.GetNamedItem("Plasticity").Value);
				if (node.Attributes["BasicPlasticity"] != null) clone.GetComponent<NeironScript>().BasicPlasticity = float.Parse(node.Attributes.GetNamedItem("BasicPlasticity").Value);
				if (node.Attributes["NewNeironDinamic"] != null) clone.GetComponent<NeironScript>().NewNeironDinamic = bool.Parse(node.Attributes.GetNamedItem("NewNeironDinamic").Value);


				NewGameOjectList.Add(clone);
			}

			GameObject.Find("Manager").GetComponent<ManagerScript>().EndIndexNeiron = MaxIndex;


			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/N"))
			{
				Transform OjectNeironIndex = null;
				foreach (GameObject neironPapa in NewGameOjectList){
					if (neironPapa.GetComponent<NeironScript>().IndexNeiron == int.Parse(node.Attributes.GetNamedItem("index").Value)) {
						OjectNeironIndex = neironPapa.transform;
						break;
					}
				}


				foreach (XmlNode author in node.SelectNodes("S"))
				{
					GameObject cSinaps = Instantiate(_prefabSinaps, OjectNeironIndex.position, transform.rotation) as GameObject; 
					cSinaps.transform.parent = OjectNeironIndex;

					foreach (GameObject neironTarget in NewGameOjectList){
						if (neironTarget.GetComponent<NeironScript>().IndexNeiron == int.Parse(author.Attributes.GetNamedItem("target").Value)){
							cSinaps.GetComponent<SinapsScript>().NeironTarget = neironTarget;
							break;
						}
					}

					cSinaps.GetComponent<SinapsScript>().Force = float.Parse(author.Attributes.GetNamedItem("force").Value);
					cSinaps.GetComponent<SinapsScript>().TypeSinaps = int.Parse(author.Attributes.GetNamedItem("type").Value);
					OjectNeironIndex.GetComponent<NeironScript>().hitSinaps.Add(cSinaps);
				}
			}

			////////////////////Загрузка скриптов

			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/Scripts/Script")){
				ManagerScripting.ScriptNet NewScriptNet = new ManagerScripting.ScriptNet();
				NewScriptNet.TypeActivator = int.Parse(node.Attributes.GetNamedItem("TypeActivator").Value);
				NewScriptNet.TypeAct = int.Parse(node.Attributes.GetNamedItem("TypeAct").Value);
				if (node.Attributes.GetNamedItem("ANeiron").Value != "0") NewScriptNet.ANeiron = GameObject.Find("Neiron" + node.Attributes.GetNamedItem("ANeiron").Value);
				int FA = int.Parse(node.Attributes.GetNamedItem("AArea").Value);
				if (FA == -1) NewScriptNet.AArea = ManagerOject.GetComponent<ManagerArea>().GlobalArea;
				if (FA >= 0) NewScriptNet.AArea = NewLidtArea[FA];
				NewScriptNet.threshold = float.Parse(node.Attributes.GetNamedItem("threshold").Value);
				NewScriptNet.controller = bool.Parse(node.Attributes.GetNamedItem("controller").Value);
				NewScriptNet.delay = float.Parse(node.Attributes.GetNamedItem("delay").Value);
				NewScriptNet.delayStep = float.Parse(node.Attributes.GetNamedItem("delayStep").Value);
				NewScriptNet.hunger = bool.Parse(node.Attributes.GetNamedItem("hunger").Value);
				if (node.Attributes.GetNamedItem("ActNeiron").Value != "0") NewScriptNet.ActNeiron = GameObject.Find("Neiron" + node.Attributes.GetNamedItem("ActNeiron").Value);
				FA = int.Parse(node.Attributes.GetNamedItem("ActArea").Value);
				if (FA == -1) NewScriptNet.ActArea = ManagerOject.GetComponent<ManagerArea>().GlobalArea;
				if (FA >= 0) NewScriptNet.ActArea = NewLidtArea[FA];
				NewScriptNet.ActMod = float.Parse(node.Attributes.GetNamedItem("ActMod").Value);
				NewScriptNet.TimeMod = float.Parse(node.Attributes.GetNamedItem("TimeMod").Value);

				ManagerOject.GetComponent<ManagerScripting>().ListScript.Add(NewScriptNet);
			}

			//Загрузка входов/выходов 
			///// Загрузка ввода
			EntryOject = GameObject.Find("BlokEnter");
			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/Entry/Element"))
			{
				foreach (GameObject neironTarget in NewGameOjectList){
					if (neironTarget.GetComponent<NeironScript>().IndexNeiron == int.Parse(node.Attributes.GetNamedItem("target").Value)) {
						EntryOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.GetComponent<OutScript>().NeironTarget = neironTarget; 
						EntryOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.SendMessage("Start");
						break;
					}
				}
			}
            /////// Загрузка ввода блок
            EntryOject = GameObject.Find("Eye");
            foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/EntryBlock/Element"))
            {
                foreach (GameObject neironTarget in NewGameOjectList)
                {
                    if (neironTarget.GetComponent<NeironScript>().IndexNeiron == int.Parse(node.Attributes.GetNamedItem("target").Value))
                    {
                        EntryOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.GetComponent<LineNumEye>().NeironTarget = neironTarget;
                        EntryOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.SendMessage("Start");
                        break;
                    }
                }
            }
			///Загрузка выхода
			OutputOject = GameObject.Find("BlokExit");
			foreach (XmlNode node in xmlDoc.SelectNodes("NetSave/output/Element"))
			{
				foreach (GameObject neironTarget in NewGameOjectList){
					if (neironTarget.GetComponent<NeironScript>().IndexNeiron == int.Parse(node.Attributes.GetNamedItem("target").Value)) {
						OutputOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.GetComponent<LineNum>().NeironTarget = neironTarget; 
						OutputOject.transform.GetChild(int.Parse(node.Attributes.GetNamedItem("Index").Value)).gameObject.SendMessage("Start");
						break;
					}
				}
			}
		}

	}
}
