using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeironScript : MonoBehaviour {

	public GameObject prefabNeiron;          //Префаб нейрона
	public GameObject prefabSinaps;          //Префаб синапса

	public int IndexNeiron = 0;              //Индекс нейрона

	private int _TypeIndexNeiron = 0;        //Индекс типа нейрона

	public int TypeIndexNeiron               //Индекс типа нейрона
	{
		get { return _TypeIndexNeiron; }
		set {
			if (value == 0) 
				{ _TypeIndexNeiron = value;
				  gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 0, 255);//Жёлтый, сумматор
				}	
			if (value == 1) 
				{ _TypeIndexNeiron = value;
				gameObject.GetComponent<SpriteRenderer>().color = new Color32( 0, 255, 0, 255); //Зелёный, модулирующий //
				}	
			if (value == 2) 
				{ _TypeIndexNeiron = value;
				gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 255, 255);//Голубой, асоциативный тип
				}
			if (value == 3) 
				{ _TypeIndexNeiron = value;
				gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);//Белый, тормозящие нейроны
				}
			}
	}
	// Настройки нейрона 0
	public float Adder = 0.0f; 										//Сумматор

	public float MaxAdder = 30f;									//Максимальное значение сумматора

	public float DampferAdder = 1.0f; 								//Регулятор сумматора
	public float thresholdTop = 1.0f; 								//Верхний баховый порог
	public float AnswerTime = 0.1f;      							//Время ответа
	public float TimeRepose = 0f;       							//Время отдыха 

	public bool IgnoreInput = false; 								//Игнорируется ли входы
	public List<GameObject> hitSinaps = new List<GameObject>();  	//Список синапсов

	public GameObject Area; 										//Область

	private bool _ActionN;											//Активен ли нейрон
	
	public bool ActionN												//Активен ли нейрон
	{
		get { return _ActionN; }
		set 
		{
			_ActionN = value;
			if (Area != null)
			{
				gameObject.GetComponent<LineRenderer>().enabled = value; //Показать вектор направления..
                bool existAction = Area.GetComponent<AreaScript>().NeironActionList.Contains(gameObject); //existAction = true - если нейрон в списке активных нейронов области
				if (_ActionN && (!existAction)) Area.GetComponent<AreaScript>().NeironActionList.Add(gameObject); //добавить в список активных области
				else Area.GetComponent<AreaScript>().NeironActionList.Remove(gameObject); //удалить из списка активных области
			}
		}
	}

	// Настройки нейрона 1 

	public float thresholdDown = -5.0f;			 		//Нижний порог 
	public float timeIgnore = 5.0f;   					//Время усиленной реполяризации при значении сумматора ниже нижнего порога
	public float bonusThreshold = 0f; 					//Надбавка на верхний порог
	public float DempferBonusThreshold = 1.0f; 			//Регулятор фактического значения порога
	public float TimeEvaluation = 5.0f;					//Время оценки
	public int LimitRecurrence = 5; 					//Лимит повторений
	public float thresholdTopUp = 1.0f;					//На сколько повысить порог

	public bool TimeEvaluationBool = false;				//Время оценки
	public int LimitEvaluationInt = 0;					//Счётчик  повторений в период оценки

	public float AdaptationTime = 0;                    //Время адаптации
	public float thresholdAdapt = 1f;                   //Минимум при адаптации

	// Настройка нейрона 2

	public float MaxForceSinaps = 100f;
	private Vector3 VectorPattern; 						//Вектор патерна
	public Vector3 VectorTrend; 						//Вектор пути

	public float Charge = 0.0f; 						//Заряд
	public float TimeCharge = 0.01f; 					//Время такта смены заряда

	private float changeAngle = 0f; 					//Изменение угла вектора

	public float FocusNeiron = 90f;						//Фокус нейрона
	public bool FocusDinamic = true;                    //Изменять фокус динамически
	public float StepFocus = 1f;						//Шаг изменения фокуса
	public float MaxFocus = 90f;						//Максимальное значение фокуса

	public float Plasticity = 1.0f; 					//Нейропластичность
	public bool PlasticityDinamic = true; 				//Изменяется ли пластичность 
	public float StepPlasticity = 0.01f;				//Шаг пластичности
	public float BasicPlasticity = 1.0f;				//Базовая пластичность (пластичность новых нейронов)
	public bool NewNeironDinamic = true;				//Создовать ли нейрон динамически

	

	private float angleMin = 0f;

	private bool CorunPlasticRun = false;

	// END VAR

    private Vector3 noveltyVector = Vector3.zero;
    private float noveltyFactor = 0.1f;

	IEnumerator StartSummator (){
		IgnoreInput = true;  //Включаем игнорирование внешних сигналов
		gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255); //Подсветка красным
		ActionN = true; //Активное состояние для идикаторов выхода
		yield return new WaitForSeconds(AnswerTime); //Время ответа
		ActionN = false; 
		ExcitationTransfer (); //передача возбуждения
		yield return new WaitForSeconds(TimeRepose);//время отдыха
		IgnoreInput = false; // отключаем игнорирование внешних сигналов
		TypeIndexNeiron = _TypeIndexNeiron; //Возращаем цвет нейроэлементы 
	}

	IEnumerator repolarizationTime (){
		IgnoreInput = true; //включаем игнорирование внешних сигналов
		gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 255, 255);//устанавливаем синий цвет
		yield return new WaitForSeconds(timeIgnore);//время игнора
		IgnoreInput = false;
		TypeIndexNeiron = _TypeIndexNeiron;//включаем свой цвет
	}

	IEnumerator StartModule (){
        IgnoreInput = true; //включаем игнорирование внешних сигналов
		ActionN = true; //Состояние активности, это значение считывают индикаторы выходов 
		gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);//крачный цвет
		yield return new WaitForSeconds(AnswerTime);//время ответа
		ExcitationTransfer ();//передача по всем исходящим синапсам
		ActionN = false;//выключаем активность 
		yield return new WaitForSeconds(TimeRepose);//время отдыха
		IgnoreInput = false;//перестаем игнорировать внешние сигналы
		TypeIndexNeiron = _TypeIndexNeiron;//возращаем цвет
		StartCoroutine ("EvaluationTime");//запуск времени оценки
		if ((AdaptationTime > 0) && (thresholdTop > thresholdAdapt)) StartCoroutine ("AdaptationVoid");//запуск адаптации, при значении настроект адаптации =0 адаптация не работает
        //и нет смысла запускать корунтину если порог на нижнем пределе
	}

	IEnumerator EvaluationTime(){ 
		TimeEvaluationBool = true;//Сейчас пойдет время оценки
		yield return new WaitForSeconds(TimeEvaluation);
		TimeEvaluationBool = false;//Время оценки кончилось
	}

	IEnumerator AdaptationVoid(){
		yield return new WaitForSeconds(AdaptationTime);//временной итервал 
        if (thresholdTop > thresholdAdapt) thresholdTop--;//снижаем порог, но не ниже базового
		if ((AdaptationTime > 0) && (thresholdTop > thresholdAdapt)) StartCoroutine ("AdaptationVoid");//снова запускаем адаптацию
	}

	IEnumerator NegativeRepolarization(){
		IgnoreInput = true; //влючаем игнорирование внешних сигналов
		ActionN = true; //Активность
        for (int i = 0; i < 16; i++)
        {  //перебираем занчения заряда
			Charge = Area.GetComponent<AreaScript>().Spike2[i];
			if (Charge > 0) gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255); //красный
			else gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 255, 255); //синий
			yield return new WaitForSeconds(TimeCharge); //с заданной частотой/скоростью
		}
		Charge = 0f;//заряд обнуляем
		TypeIndexNeiron = _TypeIndexNeiron;//свой цыет
		ActionN = false;//активность
		IgnoreInput = false;//больше не игнорируем внешние сигналы
	}

	IEnumerator StartAssociative(){
		IgnoreInput = true;//Игнорирование внешних сигналов
		ActionN = true;//Активность
        StartCoroutine("PositiveRepolarization"); //параллельно начинаем изменять заряд
		yield return new WaitForSeconds(AnswerTime); 		//Время ответа 
		Compass ();//отдельный блок кода
	}

	IEnumerator StartWhite() {
        IgnoreInput = true;//Игнорирование внешних сигналов
        ActionN = true;//Активность
        StartCoroutine("PositiveRepolarization");//параллельно начинаем изменять заряд
		yield return new WaitForSeconds(AnswerTime); 		//Время ответа
		ExcitationTransfer ();//ответ по всем исходящим синапсам
	}

	IEnumerator PositiveRepolarization(){
		for (int i = 0; i < 16; i++) {
            //перебор значений заряда
			Charge = Area.GetComponent<AreaScript>().Spike1[i];
			if (Charge > 0) gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255); //красный
			else gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 255, 255); //синий
			yield return new WaitForSeconds(TimeCharge); //с установленной частотой
		}
		Charge = 0f; //обнуляем заряд
		TypeIndexNeiron = _TypeIndexNeiron;//возращаем цвет
		ActionN = false;//активность отключаем
		yield return new WaitForSeconds(TimeRepose);//время отдыха
		IgnoreInput = false;//отключаем игнорирование
		StartCoroutine ("EvaluationTime");//Время оценки
		if ((AdaptationTime > 0) && (thresholdTop > thresholdAdapt)) StartCoroutine ("AdaptationVoid");//Адаптация
	}

	IEnumerator PlasticTimeCoruntine (Vector2 PT){//Временное изменение пластичности
		CorunPlasticRun = true;//Изменение временной пластичности началось
		float PlasticBuffer = Plasticity;//Сохраням текущюю пластичность
		Plasticity = PT.x;//Устанавливаем временную
		yield return new WaitForSeconds(PT.y);//Ждем необходимое время
		Plasticity = PlasticBuffer;//Возращаем прежнюю пластичность
		CorunPlasticRun = false;//Время изменения пластичности кончилось
	}

	public void ActiveNeiron (){ //В засисимости от типа нейрона запускаем соотвествующюю программу активации
		if (!IgnoreInput)
		{
			if (TypeIndexNeiron == 0) StartCoroutine ("StartSummator");//Синапс прямого действия
			if (TypeIndexNeiron == 1) StartCoroutine ("StartModule");//Модулирующий синапс
			if (TypeIndexNeiron == 2) StartCoroutine ("StartAssociative");//Изменяемый синапса, прямого действия ассоциативного нейроэлемента
			if (TypeIndexNeiron == 3) StartCoroutine ("StartWhite");//Синапс прямого действия
		}
	}

	private void Compass (){
		if (Area != null){ //Если нейрон не принадлежит области, то невозможно получить информацию о других обьектах системы
			VectorPattern = Vector3.zero; //Обнуляем точку паттерна
            //Подсчёт точки паттерна
			for (int i = 0; i < Area.GetComponent<AreaScript>().NeironActionList.Count; i++) { //Список всех активных нейронов
				if (gameObject == Area.GetComponent<AreaScript> ().NeironActionList [i]) continue; //Исключаем данный нейрон из расчётов
				Vector3 R = Area.GetComponent<AreaScript> ().NeironActionList [i].transform.position - transform.position;//получаем относительные кординаты, относительно данного нейрона
                //Формула определения заряд нейрона на единичный вектор 
				VectorPattern += (Area.GetComponent<AreaScript> ().NeironActionList [i].GetComponent<NeironScript> ().Charge * R.normalized);//R.sqrMagnitude; .normalized   //sqrMagnitude;!!!!!!!!!(Без квадрата лучше)
			}

			if (VectorPattern.sqrMagnitude < 3f) VectorPattern = VectorTrend; //незначительное влияние не учитываем, принимаем предыдущее значение вектора
			if (VectorPattern.sqrMagnitude == 0) VectorPattern = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); 
            //Нет предыдущих значений (новый нейрон), нет других активных нейронов, то берём случайное значение, возбуждение должно передаться куда-нибудь 

			VectorPattern.Normalize(); //Получаем единичный вектор направления

            if (noveltyVector == Vector3.zero) noveltyVector = -VectorPattern; //новый нейрон (новизна ранее не определялась) устанавливаем максимальное значения - противоположное установленному вектору направления
			changeAngle = Vector3.Angle(VectorPattern, noveltyVector);// определяем угол между направлением и предыдущем значением вектора новизны

			if (Area != null) Area.SendMessage("MessageOriginality", changeAngle/180);//сообщаем области об уровне новизны для нейрона

			VectorTrend = VectorPattern; //промежуточное начение
            noveltyVector = Vector3.Slerp(noveltyVector, VectorPattern, noveltyFactor);//изменям вектор новизны
            //вектор новизны постепенно приближается к векторы направления
            //это имитация постепенного угасания получения удовольстия от нового опыта
            //если сразу присвоить noveltyVector = VectorPattern, то новизна будет оцениваться более реактивно (в ранних версиях так и было)
            //новое действие вызывает интерес даже при нескольких повторениях, а не угасает сразу после однократного повторения
			gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);//визуализация вектора предпочитаемого направления
			gameObject.GetComponent<LineRenderer>().SetPosition(1, transform.position + VectorTrend * 6);

           
			if (PlasticityDinamic) {
				if (changeAngle < 10) Plasticity -= StepPlasticity; else Plasticity += StepPlasticity; //изменение пластичности
				if (Plasticity > 1) Plasticity = 1f;
				if (Plasticity < 0) Plasticity = 0f;
                //Идея с таким динамическим изменением пластичности сейчас считаю не перспективной
                //Пластичность характеризуется областью в мозге, а также имет большую роль играет в эмоциональных механизмах. 
                //Т.е. большее внимание стоит уделить внешним условиям изменения пластичности, чем изменять пластичность под влиянием внутренних состояний
			}

			if (FocusDinamic){
				if (changeAngle < 10) FocusNeiron -= StepFocus; else FocusNeiron = MaxFocus;
				if (FocusNeiron < 0) FocusNeiron = 0;
                //На данный моммент не получилось продемонстрировать эффективность динамически изменяемого фокуса.
                //Но перспектива для него есть в более маштабных моделях.
                //Динамическое изменение фопуса поможет имулировать иррадиацию и концентрацию.
                //Иррадиацию и концентрацию - наблюдаемые явления в нервной системе, модель нервной системы должна их имитировать
			}

            //динамическое создание нейронов
			if (NewNeironDinamic){
                if (!Physics.CheckSphere(transform.position + VectorTrend * 5, 3f))
                {   //Есть ли что-то в сферической области радиусом 3,
                    //центор которой расположен на растоянии 5 от данного нейрона
                    //в направлении вектора направления
					if (Area.GetComponent<AreaScript>().Global) NewNeiron(); //Глобальная область не имеет границ
					else 
					{
						if (Area.GetComponent<Collider>().bounds.Contains(transform.position + VectorTrend * 5)) NewNeiron(); //проверка выходит ли будующий нейрон за границы своей области  
					}
				}

				//Динамическое создание синапсов 
                Collider[] hitColliders = Physics.OverlapSphere(transform.position + VectorTrend * 5, 3f); //Список всех обьектов в сферической области по направлению   
				foreach (Collider value in hitColliders) //перебор всех из этого списка
				{
					if (value.tag == "Neiron") //нужны только нейроны
					{
						bool EnableSinaps = false; //синапса с этим обьектом нет
						foreach (GameObject sinapsValue in hitSinaps) //смотрим все свои синапсы 
						{
							if (sinapsValue.GetComponent<SinapsScript>().NeironTarget == value.gameObject) {
								EnableSinaps = true; //синапс такой есть 
								break; //дальше не перебираем
							} 	
						}
						
						if (!EnableSinaps) { //Если такого синапса нет
							GameObject cSinaps = Instantiate(prefabSinaps, transform.position, transform.rotation) as GameObject;//мы его создаём 
							cSinaps.transform.parent = transform;
							cSinaps.GetComponent<SinapsScript>().NeironTarget = value.gameObject;
							cSinaps.GetComponent<SinapsScript>().Force = 0f;
							hitSinaps.Add(cSinaps);

						}

					}
				}
			}

			//Нахождение минимального угла между векторами синапсов и вектором направления
			angleMin = 180f;
			if (hitSinaps.Count != 0) angleMin = Vector3.Angle(hitSinaps[0].GetComponent<SinapsScript>().NeironTarget.transform.position - transform.position, VectorTrend);
			foreach(GameObject ShershSinaps in hitSinaps)
			{
				float angleShersh = Vector3.Angle(ShershSinaps.GetComponent<SinapsScript>().NeironTarget.transform.position - transform.position, VectorTrend);
				if (angleShersh < angleMin) angleMin = angleShersh;
			}
           
			if (FocusNeiron < angleMin) FocusNeiron = angleMin;
            //Фокус не должен уменьшаться ниже того значения, которое приведёт к угасанию всех его синапсов.
            //В конус фокуса должен входить хотябы одни вектор синапса, 
            //иначе после укрепления рефлекса далее произойдет его необоснованное угнетение.

			//Подсчет весов 
			foreach(GameObject SinapsCoeff in hitSinaps){
					if (SinapsCoeff.GetComponent<SinapsScript>().TypeSinaps == 0){
					    float angleSinaps = Vector3.Angle(SinapsCoeff.GetComponent<SinapsScript>().NeironTarget.transform.position - transform.position, VectorTrend);
					    if (angleSinaps <= FocusNeiron) SinapsCoeff.GetComponent<SinapsScript>().Force += MaxForceSinaps * Plasticity;
					    else SinapsCoeff.GetComponent<SinapsScript>().Force -= MaxForceSinaps * Plasticity;
					    SinapsCoeff.GetComponent<SinapsScript>().Force = Mathf.Clamp(SinapsCoeff.GetComponent<SinapsScript>().Force, 0, MaxForceSinaps);
				    }
			}
		}

		ExcitationTransfer ();//передача по всем исходящим синапсам
	}

	private void NewNeiron (){
		GameObject clone = Instantiate(prefabNeiron, transform.position + VectorTrend * 6, transform.rotation) as GameObject;
        /* Интересная фича: если производить самокопирование из обьекта на сцене (как это происходит здесь),
         * то происходит копирование значений переменных родителя, а не беруться исходные значения настроенного префаба 
         * поэтому требуется это учитывать и устанавливать начальные настройки, значения переменных в новом экземпляре.
         * Не знание этого нюанса создало множество проблем....
         * */
		if (Area != null) Area.GetComponent<AreaScript>().amount++;//подсчёт нейронов в области принадлежащей нейрону

		clone.GetComponent<NeironScript>().Plasticity = BasicPlasticity;//стартовые настройки нейроэлемента
		clone.GetComponent<NeironScript>().ActionN = false;
		clone.GetComponent<NeironScript>().IgnoreInput = false;
		clone.GetComponent<NeironScript>().Adder = 0f;
		clone.GetComponent<NeironScript>().VectorTrend = Vector3.zero;
		clone.GetComponent<NeironScript>().Area = Area;
		clone.GetComponent<NeironScript>().TimeEvaluationBool = false;
		clone.GetComponent<NeironScript>().LimitEvaluationInt = 0;
		clone.GetComponent<NeironScript>().Charge = 0.0f; 
		clone.GetComponent<NeironScript>().FocusNeiron = MaxFocus;
		clone.GetComponent<NeironScript>().Plasticity =  BasicPlasticity;
		clone.GetComponent<NeironScript>().TypeIndexNeiron = 2;
        clone.GetComponent<NeironScript>().noveltyVector = Vector3.zero;
        clone.GetComponent<NeironScript>().VectorTrend = Vector3.zero;

		clone.GetComponent<LineRenderer>().SetPosition(0, clone.transform.position);
		clone.GetComponent<LineRenderer>().SetPosition(1, clone.transform.position);

		clone.SendMessage("StopNeiron"); //в новом обьекте даже корунтины находятся в той же фазе работы, что и родителе

		GameObject ManagerObj = GameObject.Find("Manager"); //...нужно исправить, Find перегружает  
		ManagerObj.GetComponent<ManagerScript>().EndIndexNeiron++;//подсчитываем нейроны
		clone.GetComponent<NeironScript>().IndexNeiron = ManagerObj.GetComponent<ManagerScript>().EndIndexNeiron;//определяем номер для нового нейрона
		clone.name = "Neiron" + clone.GetComponent<NeironScript>().IndexNeiron;//отмечаем идекс нейрона в имени обьекта

        foreach (GameObject sd in clone.GetComponent<NeironScript>().hitSinaps) Destroy(sd); //префаб размещенный в сцене из подобного себе обьекта, копирует и дочерниие обьеты родителя
		clone.GetComponent<NeironScript>().hitSinaps.Clear(); //приходится очищать новый обьект. Это странно..
	}

	void FixedUpdate(){ //Главная функция для нейрона срабатывает каждые 0.01с


		if (!IgnoreInput) //Если внешние сигналы не игнорируется
		{
			if (TypeIndexNeiron == 0)  // Это простой сумматор 
			{
				if (Adder > thresholdTop) //Пороговая функция
				{
					StartCoroutine ("StartSummator"); 
				}
			}

			if (TypeIndexNeiron == 1) //Это модулируемый нейроэлемент
			{
				if (Adder > thresholdTop + bonusThreshold) //Пороговая функция
				{
					
					if (TimeEvaluationBool) //Сейчас время оценки?
					{                       
						LimitEvaluationInt++; //Счётчик повторов
						StopCoroutine("EvaluationTime"); //Остановка корунтины отстивающей время оценки
						TimeEvaluationBool = false; //выключение фазы оценки
					}
					else LimitEvaluationInt = 0; //иначе сбрасываем счётчик повторов

					if ((LimitEvaluationInt > LimitRecurrence) && (bonusThreshold == 0)) thresholdTop += thresholdTopUp; //лимит повторов превышен и небыло модуляции значит повышаем порого - превыкание

					StopCoroutine ("AdaptationVoid");  //остановка адаптации, она выполняется только при протое нейрона
					StartCoroutine ("StartModule"); // Запуск модулируемого нейроэлемента 
					
				}

				if (Adder < thresholdDown) //пороговая функция на нижний порог
				{
					if (Area != null) StartCoroutine ("repolarizationTime"); //при достаточном тормозящем воздействии, биологический нейрон усиленно поляризуется
				}
			}

			if (TypeIndexNeiron == 2) //модулируемый нейроэлемент
			{
				if (Adder > thresholdTop + bonusThreshold) //порог сладывается из двух частей: обычной и модулируемой
				{
					if (TimeEvaluationBool) //если активация произошла во время оценки
					{
						LimitEvaluationInt++; //считаем лимит повторов
						StopCoroutine("EvaluationTime");//останавливаем подсчёт времени
						TimeEvaluationBool = false;
					}
					else LimitEvaluationInt = 0; //иначе время оценки прошло, сбрасываем счётчик

					if ((LimitEvaluationInt > LimitRecurrence) && (bonusThreshold == 0)) thresholdTop += thresholdTopUp; //лемит превышен и дело не в модуляции, то повышам порог 

					StopCoroutine ("AdaptationVoid");//активация призошла, приостанавливаем адаптацию (адаптация - снижеине порога при простое)
					StartCoroutine ("StartAssociative"); //Старт активации модулируемого нейроэлемента 
				}

				if (Adder < thresholdDown) //сумматор ниже нижнего порога
				{
					StartCoroutine ("NegativeRepolarization");  //усиленная реполяризация (торможение)
				}
			}

			if (TypeIndexNeiron == 3) //Ассоциативный нейроэлемент 
			{
				if (Adder > thresholdTop + bonusThreshold)//Порог из двух состовляющих
				{
					if (TimeEvaluationBool)//Время оценки...
					{
						LimitEvaluationInt++;
						StopCoroutine("EvaluationTime");
						TimeEvaluationBool = false;
					}
					else LimitEvaluationInt = 0;

					if ((LimitEvaluationInt > LimitRecurrence) && (bonusThreshold == 0)) thresholdTop += thresholdTopUp;

					StopCoroutine ("AdaptationVoid");
					StartCoroutine ("StartWhite");  
				}

				if (Adder < thresholdDown)
				{
					StartCoroutine ("NegativeRepolarization");  
				}
			}

		}

        if (Mathf.Abs(Adder) <= DampferAdder) Adder = 0f; //Демпфер сумматора
		if (Adder > DampferAdder) Adder -= DampferAdder;
		if (Adder < -DampferAdder) Adder += DampferAdder;

        if (Mathf.Abs(bonusThreshold) <= DempferBonusThreshold) bonusThreshold = 0f; //Демпфер дополнительного порога
		if (bonusThreshold > DempferBonusThreshold) bonusThreshold -= DempferBonusThreshold;
		if (bonusThreshold < -DempferBonusThreshold) bonusThreshold += DempferBonusThreshold;
	} 

	private void ExcitationTransfer () //Передача возбуждения
	{
		foreach (GameObject value in hitSinaps) // по всем синапсам
		{
			int T = value.GetComponent<SinapsScript>().TypeSinaps; //тип синапса
			float F = value.GetComponent<SinapsScript>().Force; //его сила
			GameObject NT = value.GetComponent<SinapsScript>().NeironTarget;//целевой нейрон
			if (T == 0) NT.SendMessage("AddSummator", F);//Передача сообщения
			if (T == 1) NT.SendMessage("AddTActual", F);
			if (T == 2) NT.SendMessage("ActiveNeiron");
            if (T == 3) NT.SendMessage("AddSummator", F);
			value.GetComponent<SinapsScript>().GoAction = true;//Запуск анимации синапса
		}
	}

	public void AddSummator (float Summ) //Для управления сумматором из других нейронов 
	{
		Adder += Summ;
		if (Adder > MaxAdder) Adder = MaxAdder;
        if (Adder < - MaxAdder) Adder = -MaxAdder;
	}

	public void AddTActual (float T)// управление порогом, модулирующее воздействие
	{
		bonusThreshold += T;
		if (bonusThreshold + thresholdTop < 0f) bonusThreshold = - thresholdTop + 0.0001f;
	}

	private static bool PointFromSphere (Vector3 Point, Vector3 Sphere, float Radius)
	{
		if ((Point - Sphere).magnitude > Radius) return true;
		else return false;
	}

	public void StopNeiron(){//остановка всех корунтин, возможно из других обьектов GameOject.SendMessage("StopNeiron") 
		StopAllCoroutines();
	}

	public void plasticSetTime (Vector2 plasticTime){
        //для управления пластичностью, SendMessage может передать только один параметр, Vectir2 - это два реальных числа
        //уровень пластичности и время изменения 
		if (!CorunPlasticRun) StartCoroutine("PlasticTimeCoruntine", plasticTime);
        if (TypeIndexNeiron == 2) thresholdTop = thresholdAdapt;
	}
}
