using UnityEngine;
using System.Collections;
/* Для размещения шаблона */
public class TargetCamScript : MonoBehaviour {

    public KeyCode Code1; //Клавишы для управление, настраиваются в редакторе.
    public KeyCode CodeHorizonPlus;
    public KeyCode CodeHorizonMinus;
    public KeyCode CodeVerticalPlus;
    public KeyCode CodeVerticalMinus;

    public KeyCode CodeRun;

    public GameObject prefabAreaBox; // префабы
    public GameObject prefabNeiron;
    public GameObject prefabSinaps;

    private GameObject cloneArea;

    private GameObject ManagerGame;
	void Update () {
        if (Input.GetKeyDown(Code1)) gameObject.GetComponent<MeshRenderer>().enabled = !gameObject.GetComponent<MeshRenderer>().enabled; //видим ли обьект, маркер размещениея шаблона
        if (Input.GetKeyDown(CodeHorizonPlus)) transform.localScale = new Vector3(transform.localScale.x + 10, transform.localScale.y, transform.localScale.z); // Управление размером маркера
        if (Input.GetKeyDown(CodeHorizonMinus))
        {
            if (transform.localScale.x > 20) transform.localScale = new Vector3(transform.localScale.x - 10, transform.localScale.y, transform.localScale.z);
        }
        if (Input.GetKeyDown(CodeVerticalPlus)) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 10, transform.localScale.z);
        if (Input.GetKeyDown(CodeVerticalMinus))
        {
            if (transform.localScale.y > 20) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 10, transform.localScale.z);
        }

        if (Input.GetKeyDown(CodeRun)) RunNewArea(); //размещение шаблона
	}

    private void RunNewArea() 
    {
        cloneArea = (GameObject)Instantiate(prefabAreaBox, transform.position, transform.rotation); // новая область
        cloneArea.transform.localScale = transform.localScale;
        cloneArea.GetComponent<AreaScript>().S = transform.localScale.x;
        cloneArea.GetComponent<AreaScript>().H = transform.localScale.y;
        cloneArea.GetComponent<AreaScript>().L = transform.localScale.z;
        cloneArea.GetComponent<AreaScript>().Name = "layer";
        cloneArea.name = "Area(layer)"; 

        ManagerGame = GameObject.Find("Manager");
        ManagerGame.GetComponent<ManagerArea>().hitArea.Add(cloneArea); // добавляем в новую область в список областей сцены

        GameObject[] OjectNeiron = GameObject.FindGameObjectsWithTag("Neiron"); //список всех нейронов сцены

        foreach (GameObject value in OjectNeiron) 
        {
            if (cloneArea.GetComponent<Collider>().bounds.Contains(value.transform.position)) // если нерйон расположен в пределах области
            {
                value.GetComponent<NeironScript>().Area = cloneArea; // то присваиваем нейрону данную область
                cloneArea.GetComponent<AreaScript>().amount++; // счётчик нейронов области
            }
        }
        ////Размещение нейронов в области
        int sx = (int) (cloneArea.transform.localScale.x - 2) / 2;
        int sy = (int) (cloneArea.transform.localScale.y - 2) / 2;
        for (int ix = -sx; ix <= sx; ix += 4) 
        {
            for (int iy = -sy; iy <= sy; iy += 4) 
            {
                float XR = ix + Random.Range(-1f, 1f);
                float YR = iy + Random.Range(-1f, 1f);
                float ZR = Random.Range(-1.5f, 1.5f);
                if (!Physics.CheckSphere(cloneArea.transform.position + new Vector3(XR, YR, ZR), 3f))
                    NewNeironVoid(cloneArea.transform.position + new Vector3(XR, YR, ZR), cloneArea);
            }
        }
        /////////Размещение сумматоров
        int ssx = (int)(cloneArea.transform.localScale.x - 6) / 2;
        int ssy = (int)(cloneArea.transform.localScale.y - 6) / 2;
        for (int ix = -ssx; ix <= ssx; ix += 10)
        {
            for (int iy = -ssy; iy <= ssy; iy += 10)
            {
                NewSummator(cloneArea.transform.position + new Vector3(ix, iy, 20));
            }
        }
    }

    private void NewNeironVoid(Vector3 positionNew, GameObject AreaClon)
    {
        GameObject clone = (GameObject)Instantiate(prefabNeiron, positionNew, prefabNeiron.transform.rotation);
        clone.GetComponent<NeironScript>().Area = AreaClon;
        AreaClon.GetComponent<AreaScript>().amount++;
        clone.GetComponent<NeironScript>().TypeIndexNeiron = 2;
        ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron++;
        clone.GetComponent<NeironScript>().IndexNeiron = ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron;
        clone.name = "Neiron" + ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron;
    }

    private void NewSummator(Vector3 positionNew)
    {
        GameObject clone = (GameObject)Instantiate(prefabNeiron, positionNew, prefabNeiron.transform.rotation);
        clone.GetComponent<NeironScript>().TypeIndexNeiron = 0;
        clone.GetComponent<NeironScript>().thresholdTop = 3;
        ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron++;
        clone.GetComponent<NeironScript>().IndexNeiron = ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron;
        clone.name = "Neiron" + ManagerGame.GetComponent<ManagerScript>().EndIndexNeiron;

        Collider[] hitColliders = Physics.OverlapSphere(positionNew + new Vector3(0, 0, -20), 15);
        foreach (Collider value in hitColliders)
        {
            if (value.gameObject.tag == "Neiron")
            {
                GameObject cSinaps = Instantiate(prefabSinaps, value.gameObject.transform.position, transform.rotation) as GameObject;
                cSinaps.transform.parent = value.gameObject.transform;
                cSinaps.GetComponent<SinapsScript>().NeironTarget = clone;
                cSinaps.GetComponent<SinapsScript>().TypeSinaps = 3;
                cSinaps.GetComponent<SinapsScript>().Force = 1f;
                value.GetComponent<NeironScript>().hitSinaps.Add(cSinaps);
            }
        }
    }
}
