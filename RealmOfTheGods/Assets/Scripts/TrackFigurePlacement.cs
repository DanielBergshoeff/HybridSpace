using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackFigurePlacement : AbstractTrackFigure, ITrackableEventHandler{
    public GameObject[] objectsToHide;

    public GameObject frontCard;
    public GameObject backCard;

    public UnitType unitToSpawn;

    public Color colorStart;
    public Color colorPressed;

    private Vector3 currentPosition;

    public void SetParent(Transform child, Transform parent, float xRotation) {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = new Vector3(xRotation, Quaternion.identity.y, Quaternion.identity.z);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.NO_POSE)
        {
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
            if(!completed) { 
                foreach (GameObject go in objectsToHide) {
                    go.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
                }
            }
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        ResetButtonColours();

        currentPosition = Vector3.zero;

        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
	}

    protected override void OnCompletedFigure() {
        base.OnCompletedFigure();

        Ray ray = new Ray(frontCard.transform.position + new Vector3(0, 100, 0), -Client.baseCore.transform.up);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, 110)) {
            Debug.Log(raycastHit.collider.name);
            //Client.LocalClient.SpawnUnitClient(raycastHit.point - Client.baseCore.transform.position, unitToSpawn);

            foreach (VirtualButtonBehaviourArray vbba in vbBehaviourArray) {
                foreach (VirtualButtonBehaviour vbb in vbba.vbBehaviours) {
                    vbb.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
            }
        }
        else {
            OnFigureFailed();
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

        if(completed && currentPosition != frontCard.transform.position) {
            Ray ray = new Ray(frontCard.transform.position + new Vector3(0, 100, 0), -frontCard.transform.up);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 110)) {
                Debug.Log(raycastHit.collider.name);
                //Client.LocalClient.SpawnWarriorClient(raycastHit.point - Client.LocalClient.baseCore.transform.position));

                //Set warrior flag

                currentPosition = frontCard.transform.position;
            }
        }
        else {
            currentPosition = frontCard.transform.position;
        }
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
