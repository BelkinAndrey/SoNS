using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/* Скрипт для префаба Area */

public class AreaScript : MonoBehaviour {

	public bool Global; // Глобальная ли область
	public List<GameObject> NeironActionList = new List<GameObject> (); // Список активных нейронов

	public bool BallBool; // Сферическая ли область
	public float R; // её радиус
	public float S; // ширина
	public float H; // высота
	public float L; // длина области

	public string Name; // название области

	public float ColorR = 1f; // цвет
	public float ColorG = 1f;
	public float ColorB = 1f;

	public int amount = 0; //Колличество нейронов в области

	public float[] Spike1 = new float[16]; // закон изменения заряда в позитиве
	public float[] Spike2 = new float[16]; // закон изменения заряда в негативе 

	public float LevelOriginality = 0f; //уровень новизны
	public class MessageTime //Для средневременного значения необходимы значения которые будут храниться/суммироваться определёное врмя 
	{
	  public float Originality = 0f;
      public float TimeOrigin = 0.5f; // время жизни в списке 0.05с
	}

	public List<MessageTime> MessageList = new List<MessageTime>(); // Список для получения средневременной новизны
	public List<MessageTime> DeleteInt = new List<MessageTime>(); //Список на удаление

	void Start (){
		if (!Global) gameObject.GetComponent<Renderer>().material.color = new Color(ColorR, ColorG, ColorB, 0.25f); //глобальная область не имеет цвета, установка цвета
	}

	void FixedUpdate(){
		DeleteInt.Clear(); // Очистка списка на удаление
		float origin = 0f; 
		foreach (MessageTime val in MessageList) 
		{
			val.TimeOrigin -= 0.01f; //Fixed Time = 0.01 => время жизни в списке 0.05с
			origin += val.Originality;
			if (val.TimeOrigin < 0) DeleteInt.Add(val); // составляем список на удаление
		}
		if (MessageList.Count != 0) LevelOriginality = (origin/MessageList.Count) * 100; else LevelOriginality = 0; //дабы не делить на ноль
		foreach (MessageTime value in DeleteInt) MessageList.Remove(value); // удаляем те элементы которые в списке на удаление
	}

	public void MessageOriginality (float ArgumentMessage) // для получения сообщения от нейрона (SendMessage)
	{
     
		MessageTime NewMessage = new MessageTime();
		NewMessage.Originality = ArgumentMessage;
		MessageList.Add(NewMessage); // так заполняется список для подсчёта уровня новизны
	}

}
