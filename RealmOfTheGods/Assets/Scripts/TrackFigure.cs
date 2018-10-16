using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackFigure : MonoBehaviour,  IVirtualButtonEventHandler, ITrackableEventHandler{

    public GameObject line;
    public GameObject parentObjectsToSwap;

    public GameObject frontCard;
    public GameObject backCard;

    public GameObject[] buttons;

    private int currentButton;
    private GameObject lastPressedButton;

    public Color colorStart;
    public Color colorPressed;

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log("Button " + vb.gameObject.name + " has been pressed!");

        if (currentButton < buttons.Length)
        {
            if (buttons[currentButton] == vb.gameObject)
            {
                lastPressedButton = vb.gameObject;
                currentButton++;
                if (currentButton >= buttons.Length)
                {
                    line.SetActive(true);
                }

                vb.gameObject.GetComponentInChildren<SpriteRenderer>().color = colorPressed;
            }
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        
    }

    public void SetParent(Transform child, Transform parent, float xRotation) {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = new Quaternion(xRotation, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Front not found");
            SetParent(line.transform, backCard.transform, 180);
            SetParent(parentObjectsToSwap.transform, backCard.transform, 180);
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            Debug.Log("Front found");

            SetParent(line.transform, frontCard.transform, 0);
            SetParent(parentObjectsToSwap.transform, frontCard.transform, 0);
        }
    }

    // Use this for initialization
    void Start () {
        currentButton = 0;
        line.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
            buttons[i].GetComponentInChildren<SpriteRenderer>().color = colorStart;
        }

        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
