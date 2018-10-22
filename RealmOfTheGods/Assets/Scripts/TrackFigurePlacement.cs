using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackFigurePlacement : AbstractTrackFigure, ITrackableEventHandler{

    public GameObject parentObjectsToSwap;

    public GameObject[] objectsToHide;

    public GameObject prefabToPlace;
    public GameObject parentToPrefab;

    public GameObject frontCard;
    public GameObject backCard;

    public Color colorStart;
    public Color colorPressed;

    public void SetParent(Transform child, Transform parent, float xRotation) {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = new Vector3(xRotation, Quaternion.identity.y, Quaternion.identity.z);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Front not found");
            if (!completed)
            {
                foreach (GameObject go in objectsToHide)
                {
                    go.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
                }
            }
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            Debug.Log("Front found");
            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            }
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        ResetButtonColours();

        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
	}

    protected override void OnCompletedFigure() {
        base.OnCompletedFigure();
        prefabToPlace.SetActive(true);

        foreach (GameObject go in objectsToHide)
        {
            go.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        }
    }

    protected override void OnFigureFailed()
    {
        base.OnFigureFailed();
        ResetButtonColours();
    }

    public override void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        base.OnButtonPressed(vb);
        vb.gameObject.GetComponentInChildren<SpriteRenderer>().color = colorPressed;
    }

    public override void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        base.OnButtonReleased(vb);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    private void ResetButtonColours()
    {
        foreach (VirtualButtonBehaviourArray vbba in vbBehaviourArray)
        {
            foreach (VirtualButtonBehaviour vbb in vbba.vbBehaviours)
            {
                vbb.gameObject.GetComponentInChildren<SpriteRenderer>().color = colorStart;
            }
        }
    }

}
