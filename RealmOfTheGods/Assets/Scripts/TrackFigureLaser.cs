using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class TrackFigureLaser : AbstractTrackFigure, ITrackableEventHandler{

    public GameObject tempProjectionPrefab;
    private GameObject tempProjection;

    public UnitType typeToSpawn;

    public GameObject line;
    public LineRenderer laserLineRenderer;
    private float laserWidth = 0.1f;
    private float laserMaxLength = 1000f;

    public GameObject vbbAction;

    public GameObject parentObjectsToSwap;

    public GameObject[] objectsToHide;

    public GameObject frontCard;
    public GameObject backCard;

    private GameObject currentCard;

    private Vector3 currentLaserPosition;

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
            //SetParent(line.transform, backCard.transform, 0);
            SetParent(parentObjectsToSwap.transform, backCard.transform, 0);
            currentCard = backCard;

            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            }
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            //SetParent(line.transform, frontCard.transform, 0);
            SetParent(parentObjectsToSwap.transform, frontCard.transform, 0);

            currentCard = frontCard;

            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            }
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        vbbAction.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

        vbbAction.GetComponentInChildren<SpriteRenderer>().enabled = false;

        //vbbAction.gameObject.SetActive(false);
        vbbAction.GetComponent<VirtualButtonBehaviour>().enabled = false;

        ResetButtonColours();

        line.SetActive(false);
        frontCard.GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);

        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    protected override void OnCompletedFigure() {
        base.OnCompletedFigure();
        line.SetActive(true);
        vbbAction.GetComponent<VirtualButtonBehaviour>().enabled = true;
        vbbAction.GetComponentInChildren<SpriteRenderer>().enabled = false;
        foreach (VirtualButtonBehaviourArray vbba in vbBehaviourArray) {
            foreach (VirtualButtonBehaviour vbb in vbba.vbBehaviours) {
                vbb.enabled = false;
                vbb.gameObject.SetActive(false);
            }
        }
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        Debug.DrawRay(targetPosition, direction * length, Color.green);

        currentLaserPosition = Vector3.negativeInfinity;

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            //If there is no projection yet, create a projection at the point of impact
            if(tempProjection == null) {
                tempProjection = Instantiate(tempProjectionPrefab, raycastHit.point, Quaternion.identity, Client.LocalClient.baseCore.transform);
            }
            //If there is a projection, but its not active, set to active
            else if(tempProjection.activeSelf == false) {
                tempProjection.SetActive(true);
            }

            //Set the projection position to the point of impact
            tempProjection.transform.position = raycastHit.point;
            endPosition = raycastHit.point;

            currentLaserPosition = raycastHit.point;
        }
        else {
            if (tempProjection != null) {
                tempProjection.SetActive(false);
            }
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
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

        if(vb.gameObject == vbbAction) {
            if (currentLaserPosition != Vector3.negativeInfinity) {
                Client.LocalClient.SpawnWarriorClient(Client.LocalClient.baseCore.transform.InverseTransformPoint(currentLaserPosition));
                line.SetActive(false);
                completed = false;
                ResetButtonColours();
                vbbAction.GetComponentInChildren<SpriteRenderer>().enabled = false;
                vbbAction.GetComponent<VirtualButtonBehaviour>().enabled = false;
                foreach (VirtualButtonBehaviourArray vbba in vbBehaviourArray) {
                    foreach (VirtualButtonBehaviour vbb in vbba.vbBehaviours) {
                        vbb.enabled = true;
                        vbb.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public override void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        base.OnButtonReleased(vb);
    }

    // Update is called once per frame
    protected override void Update () {
        if(Input.GetKeyDown(KeyCode.B)) {
            Client.LocalClient.SpawnBaseClient();
            
        }
        else if(Input.GetKeyDown(KeyCode.W)) {
            Client.LocalClient.SpawnWarriorClient(Vector3.zero);
        }

        base.Update();
        if(completed)
        {
                ShootLaserFromTargetPosition(currentCard.transform.position, -currentCard.transform.up, laserMaxLength);
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
