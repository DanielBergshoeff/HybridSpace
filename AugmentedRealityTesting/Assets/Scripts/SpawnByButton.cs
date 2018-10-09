using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SpawnByButton : MonoBehaviour,  IVirtualButtonEventHandler{

    public GameObject btn;
    public GameObject prefabObject;

    public GameObject btnRemove;

    private int childCount;
    private int currentChild;

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        // DO NOTHING
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        if (vb.gameObject == btn)
        {
            if (currentChild < childCount)
            {
                Instantiate(prefabObject, transform.GetChild(currentChild).position, transform.GetChild(currentChild).rotation, transform.GetChild(currentChild));
                currentChild++;
            }
        }
        else if(vb.gameObject == btnRemove)
        {
            if(currentChild != 0)
            {
                currentChild--;
                Destroy(transform.GetChild(currentChild).GetChild(0));
            }
        }
    }

    // Use this for initialization
    void Start () {
        btn.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        btnRemove.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        childCount = transform.childCount;
        currentChild = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
