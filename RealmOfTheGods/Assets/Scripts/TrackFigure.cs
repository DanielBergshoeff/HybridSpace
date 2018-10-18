using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackFigure : AbstractTrackFigure, ITrackableEventHandler{

    public GameObject line;
    public GameObject parentObjectsToSwap;

    public GameObject[] objectsToHide;

    public GameObject frontCard;
    public GameObject backCard;

    public Color colorStart;
    public Color colorPressed;

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
            SetParent(line.transform, backCard.transform, -90);
            //SetParent(parentObjectsToSwap.transform, backCard.transform, 180);

            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            }
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            Debug.Log("Front found");

            SetParent(line.transform, frontCard.transform, -90);
            //SetParent(parentObjectsToSwap.transform, frontCard.transform, 0);


            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            }
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        ResetButtonColours();

        line.SetActive(false);
        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
	}

    protected override void OnCompletedFigure() {
        line.SetActive(true);
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
