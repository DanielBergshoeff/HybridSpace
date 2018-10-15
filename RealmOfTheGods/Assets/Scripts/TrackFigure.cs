using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackFigure : MonoBehaviour,  IVirtualButtonEventHandler, ITrackableEventHandler{

    public GameObject line;
    public GameObject[] objectsToSwap;

    public GameObject frontCard;
    public GameObject backCard;

    public GameObject[] buttons;

    private int currentButton;
    private GameObject lastPressedButton;

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        if (buttons[currentButton] == vb.gameObject)
        {
            lastPressedButton = vb.gameObject;
            currentButton++;
            if(currentButton >= buttons.Length)
            {
                line.SetActive(true);
            }
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Front not found");
            line.transform.parent = backCard.transform;
            for (int i = 0; i < objectsToSwap.Length; i++)
            {
                objectsToSwap[i].transform.parent = backCard.transform;
            }
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            Debug.Log("Front found");

            line.transform.parent = frontCard.transform;
            for (int i = 0; i < objectsToSwap.Length; i++)
            {
                objectsToSwap[i].transform.parent = frontCard.transform;
            }
        }
    }

    // Use this for initialization
    void Start () {
        currentButton = 0;
        line.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        }

        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
