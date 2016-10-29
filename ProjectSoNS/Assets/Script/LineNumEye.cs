using UnityEngine;
using System.Collections;

public class LineNumEye : MonoBehaviour {

    public GameObject NeironTarget;
    private float yellowFon;

    private float _Force = 0f;
    private int TimeBee = 0;

    private bool Active = false;

    public float Force 
    {
        get { return _Force; }
        set 
        {
            _Force = value;
            SelectOff();
        }
    }

    IEnumerator RunTimeEye ()
    {
        Active = true;
        yield return new WaitForSeconds(1 - Force);
        if (NeironTarget != null) NeironTarget.SendMessage("ActiveNeiron");
        TimeBee = 10;
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(240, 240, 20, 255);
        gameObject.GetComponent<LineRenderer>().SetColors(new Color32(240, 240, 20, 255), new Color32(240, 240, 20, 255));
        Active = false;
    }

    void Start()
    {
        gameObject.GetComponent<LineRenderer>().SetPosition(0, gameObject.transform.position);

        if (NeironTarget != null)
        {
            gameObject.GetComponent<LineRenderer>().SetPosition(1, NeironTarget.transform.position);
        }
        else gameObject.GetComponent<LineRenderer>().SetPosition(1, gameObject.transform.position);
    }

    void FixedUpdate() 
    {
        if (TimeBee == 1) 
        {
            SelectOff();
            gameObject.GetComponent<LineRenderer>().SetColors(new Color32(255, 255, 255, 38), new Color32(255, 255, 255, 38));
        } 
        TimeBee--;
        if (TimeBee < 0) TimeBee = 0;
    }

    void OnMouseDown() 
    {
        if (!transform.parent.GetComponent<EyeF1>().EditBool)
        {
            if (Force == 1f) Force = 0f;
            else if (Force == 0f) Force = 1f;
            SelectOff();
            transform.parent.GetComponent<EyeF1>().SelectReceptor = gameObject;
        }
    }

    public void ApplyTarget(GameObject TargetOject)
    {
        NeironTarget = TargetOject;
    }

    public void SelectOn() 
    {
        ///
    }

    public void SelectOff() 
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.15f + (Force * 0.85f));
    }

    public void RunRun() 
    {
        if (Force > 0) 
        {
            if (Force == 1)
            {
                if (NeironTarget != null) NeironTarget.SendMessage("ActiveNeiron");
                TimeBee = 10;
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(240, 240, 20, 255);
                gameObject.GetComponent<LineRenderer>().SetColors(new Color32(240, 240, 20, 255), new Color32(240, 240, 20, 255));
            }
            else 
            {
                if (!Active) StartCoroutine("RunTimeEye");
            }
        }
    }
}
