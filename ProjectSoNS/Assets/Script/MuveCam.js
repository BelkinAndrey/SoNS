#pragma strict

var target : Transform;          // "Цкль" внешняя переменная, Transform - любой обьект с координатами

public var distance = 500.0;     // Дистанция 

public var xSpeed = 5.0;        // Коэффициенты скорости вращения камеры
public var ySpeed = 5.0;

public var xMoveSpeed = 0.01;   //Коэффициент передвижения камеры
public var yMoveSpeed = 0.01;

public var ScrollSpeed = 100.0;   //Скорость зума

private var x = 0.0;              // Рабочие координаты
private var y = 0.0;

private var xMove = 0.0;         //Рабочие коорддинаты для предвижения
private var yMove = 0.0;



function Start () {
  var angles = transform.eulerAngles;   // Присваиваем переменной angles углы Эллера
    x = angles.y;                       // присваиваим х и у 
    y = angles.x;
  
}

function LateUpdate () {
    if (target) {    
      
       distance += -(Input.GetAxis("Mouse ScrollWheel")*ScrollSpeed);  //Скроллинг, зуум
       if (distance < 0) distance = 0;                           //Дистанция до таргета не менее 0
      
      
       if (Input.GetMouseButton(1)){                                   //При нажатой правой кнопке
                                         
        x += Input.GetAxis("Mouse X") * xSpeed;  
        y -= Input.GetAxis("Mouse Y") * ySpeed;
 		
 		y = ClampAngle(y);   //Ограничиваем у, не будет, заменить на ограничитель -360 : 360
 		x = ClampAngle(x);
 		              
                                  }  
     
       if (Input.GetMouseButton(2)){                                  //При нажвтой средней кнопке
      
        xMove = Input.GetAxis("Mouse X")*xMoveSpeed*(distance+10);     //  *(distance+10)  
        yMove = Input.GetAxis("Mouse Y")*yMoveSpeed*(distance+10);
        
        target.position += transform.TransformDirection(Vector3(-xMove,-yMove,0));
      
                                  }
      
      
        var rotation = Quaternion.Euler(y, x, 0);          
        var position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
        
        transform.rotation = rotation;
        transform.position = position;  
         
    
                }
}


static function ClampAngle (angle : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return angle;
}