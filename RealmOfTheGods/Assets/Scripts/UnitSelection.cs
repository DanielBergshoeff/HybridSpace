using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class UnitSelection : MonoBehaviour, IVirtualButtonEventHandler {
    public enum ControlOption {
        NULL,
        Select,
        Move
    }

    public VirtualButtonBehaviour vbbSelect;
    public VirtualButtonBehaviour vbbMove;
    public VirtualButtonBehaviour vbbCancel;
    public ControlOption controlOption;
    public float laserWidth = 0.1f;
    public GameObject frontCard;
    public GameObject tempProjectionPrefab;

    public Color buttonsStartColor;
    public Color buttonsActivatedColor;

    public GameObject linePrefab;
    private float laserMaxLength = 100.0f;
    private GameObject line;
    private LineRenderer laserLineRenderer;
    private Vector3 currentLaserPosition;
    private GameObject selectedUnit;
    private GameObject tempProjection;
    private List<VirtualButtonBehaviour> virtualButtonBehaviours;

    public void OnButtonPressed(VirtualButtonBehaviour vb) {
        if (vb == vbbSelect) {
            OnSelect();
        }
        else if (vb == vbbMove) {
            OnMove();
        }
        else if (vb == vbbCancel) {
            OnCancel();
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb) {

    }

    // Use this for initialization
    void Start() {
        virtualButtonBehaviours = new List<VirtualButtonBehaviour>();

        vbbSelect.RegisterEventHandler(this);
        vbbMove.RegisterEventHandler(this);
        vbbCancel.RegisterEventHandler(this);

        virtualButtonBehaviours.Add(vbbSelect);
        virtualButtonBehaviours.Add(vbbMove);
        virtualButtonBehaviours.Add(vbbCancel);

        SetButtonColors(buttonsStartColor);

        line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        line.SetActive(false);
        laserLineRenderer = line.GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    // Update is called once per frame
    void Update() {
        if(controlOption == ControlOption.Select || controlOption == ControlOption.Move) {
            ShootLaserFromTargetPosition(frontCard.transform.position, -frontCard.transform.up, laserMaxLength);
        }
        else {
            tempProjection.SetActive(false);
        }
    }

    void OnSelect() {
        if (controlOption == ControlOption.NULL || controlOption == ControlOption.Move) {
            //Turn on laser so the user can select a unit
            line.SetActive(true);
            controlOption = ControlOption.Select;
            vbbSelect.gameObject.GetComponentInChildren<Renderer>().material.color = buttonsActivatedColor;
        }
        else if (controlOption == ControlOption.Select) {
            //Select unit at end of laser, turn off laser, and set controloption to move
            if(selectedUnit != null) {
                controlOption = ControlOption.Move;
                vbbSelect.gameObject.GetComponentInChildren<Renderer>().material.color = buttonsStartColor;
                vbbMove.gameObject.GetComponentInChildren<Renderer>().material.color = buttonsActivatedColor;
            }
        }
    }

    void OnMove() {
        if(controlOption == ControlOption.Move) {
            //Send unit to end laser
            if (currentLaserPosition != Vector3.negativeInfinity) {
                Client.LocalClient.SetUnitFlag(currentLaserPosition, selectedUnit);
                vbbMove.gameObject.GetComponentInChildren<Renderer>().material.color = buttonsStartColor;
                line.SetActive(false);
                controlOption = ControlOption.NULL;
            }
        }
    }

    void OnCancel() {
        SetButtonColors(buttonsStartColor);
        selectedUnit = null;
        line.SetActive(false);
        controlOption = ControlOption.NULL;
    }

    void SetButtonColors(Color color) {
        foreach(VirtualButtonBehaviour vbb in virtualButtonBehaviours) {
            vbb.gameObject.GetComponentInChildren<Renderer>().material.color = color;
        }
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length) {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        Debug.DrawRay(targetPosition, direction * length, Color.green);

        currentLaserPosition = Vector3.negativeInfinity;

        selectedUnit = null;

        if (Physics.Raycast(ray, out raycastHit, length)) {
            if (controlOption == ControlOption.Move) {
                //If there is no projection yet, create a projection at the point of impact
                if (tempProjection == null) {
                    tempProjection = Instantiate(tempProjectionPrefab, raycastHit.point, Quaternion.identity, Client.LocalClient.baseCore.transform);
                }
                //If there is a projection, but its not active, set to active
                else if (tempProjection.activeSelf == false) {
                    tempProjection.SetActive(true);
                }

                //Set the projection position to the point of impact
                tempProjection.transform.position = raycastHit.point;

                if (raycastHit.collider.gameObject.GetComponent<Unit>() != null)
                    selectedUnit = raycastHit.collider.gameObject.transform.parent.gameObject;
            }

            currentLaserPosition = raycastHit.point;
            endPosition = raycastHit.point;
        }
        else {
            if (tempProjection != null) {
                tempProjection.SetActive(false);
            }
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}
