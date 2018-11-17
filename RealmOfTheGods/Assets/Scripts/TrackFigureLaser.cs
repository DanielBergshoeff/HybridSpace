using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class TrackFigureLaser : AbstractTrackFigure, ITrackableEventHandler{

    public GameObject line;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 10f;

    public GameObject prefabWarrior;

    public GameObject parentObjectsToSwap;

    public GameObject[] objectsToHide;

    public GameObject frontCard;
    public GameObject backCard;

    private GameObject currentCard;

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
            //SetParent(line.transform, backCard.transform, 0);
            SetParent(parentObjectsToSwap.transform, backCard.transform, 0);
            currentCard = backCard;

            foreach (GameObject go in objectsToHide) {
                go.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            }
        }
        else //if (newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED || newStatus == TrackableBehaviour.Status.DETECTED)
        {
            Debug.Log("Front found");

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
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        Debug.DrawRay(targetPosition, direction * length, Color.green);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            Debug.Log(raycastHit.collider.name);
            endPosition = raycastHit.point;

            OnLaserHit(raycastHit);
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }

    protected void OnLaserHit(RaycastHit raycastHit) {
        /*if(raycastHit.collider.gameObject.tag == "Humanoid")
            {
                raycastHit.collider.gameObject.GetComponent<Humanoid>().myParticleSystem.SetActive(true);
                line.SetActive(false);
                completed = false;
                ResetButtonColours();
            }*/
        GameObject go = Instantiate(prefabWarrior, raycastHit.point, Quaternion.identity);
        NetworkServer.Spawn(go);
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
