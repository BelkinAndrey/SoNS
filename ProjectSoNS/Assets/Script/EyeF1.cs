using UnityEngine;
using System.Collections;

public class EyeF1 : MonoBehaviour {

    public KeyCode Code1;
    public KeyCode Code2;
    public KeyCode Code3;

    public GameObject TextF1;
    public GameObject TextF2;
    public GameObject TextF3;

    public GameObject[] EyeBox; 

    private bool _RepetBool = false;
    private bool _EditBool = false;

    public bool RepetBool 
    {
        get { return _RepetBool; }
        set 
        {
            _RepetBool = value;
            if (value) TextF2.GetComponent<TextMesh>().color = new Color32(240, 240, 89, 204);
            else TextF2.GetComponent<TextMesh>().color = new Color32(255, 255, 255, 30);
        }
    }
    public bool EditBool 
    {
        get { return _EditBool; }
        set 
        {
            _EditBool = value;
            if (value)
            {
                TextF3.GetComponent<TextMesh>().color = new Color32(240, 240, 89, 204);
                foreach (GameObject valueEye in EyeBox) valueEye.tag = "EyeEdit";
            }
            else {
                TextF3.GetComponent<TextMesh>().color = new Color32(255, 255, 255, 30);
                foreach (GameObject valueEye in EyeBox) valueEye.tag = "Eye";
            }
        }
    }

    private GameObject _SelectReceptor;

    public GameObject SelectReceptor 
    {
        get { return _SelectReceptor; }
        set 
        {
            if (_SelectReceptor != null) _SelectReceptor.transform.localScale = new Vector3(40, 40, 1);
            _SelectReceptor = value;
            if (_SelectReceptor != null) _SelectReceptor.transform.localScale = new Vector3(45, 45, 1);
        }
    }

    void Update()
    {

        if (Input.GetKey(Code1)) 
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(240, 240, 89, 204);
            TextF1.GetComponent<TextMesh>().color = new Color32(240, 240, 89, 204);
            SelectReceptor = null;
        }
        else 
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 30);
            TextF1.GetComponent<TextMesh>().color = new Color32(255, 255, 255, 30);
        };

        if (Input.GetKeyDown(Code1) && (!RepetBool)) RunEye();

        if (Input.GetKeyDown(Code2))
        {
            SelectReceptor = null;
            RepetBool = !RepetBool;
        }

        if (Input.GetKeyDown(Code3)) 
        {
            SelectReceptor = null;
            EditBool = !EditBool; 
        }

        if (RepetBool) RunEye();

    }

    void OnGUI() 
    {
        if (SelectReceptor != null) 
        {
            SelectReceptor.GetComponent<LineNumEye>().Force = GUI.HorizontalSlider(new Rect(5, Screen.height - 15, 200, 30), SelectReceptor.GetComponent<LineNumEye>().Force, 0.0f, 1.0f);
            GUI.Label(new Rect(5, Screen.height - 35, 300, 30), SelectReceptor.name + " - Force : " + SelectReceptor.GetComponent<LineNumEye>().Force.ToString("0.00") +
                "; Time " + (1f - SelectReceptor.GetComponent<LineNumEye>().Force).ToString("0.00"));
        }
    }

    private void RunEye() 
    {
        foreach (GameObject value in EyeBox) value.SendMessage("RunRun");
    }
}
