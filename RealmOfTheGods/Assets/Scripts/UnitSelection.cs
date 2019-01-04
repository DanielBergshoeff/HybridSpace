using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class UnitSelection : MonoBehaviour, IVirtualButtonEventHandler {
    public enum ControlOption {
        NULL,
        Move
    }
    
    public VirtualButtonBehaviour vbbMove;
    public VirtualButtonBehaviour vbbCancel;
    public float laserWidth = 0.1f;
    public GameObject frontCard;
    public GameObject tempProjectionPrefab;
    public TeamType team;

    public Color buttonsStartColor;
    public Color buttonsActivatedColor;

    public GameObject linePrefab;
    private float laserMaxLength = 100.0f;
    private GameObject line;
    private LineRenderer laserLineRenderer;
    private Vector3 currentLaserPosition;

    private ControlOption controlOption;

    private List<VirtualButtonBehaviour> virtualButtonBehaviours;

    public void OnButtonPressed(VirtualButtonBehaviour vb) {
        if (vb == vbbMove) {
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
        vbbMove.RegisterEventHandler(this);
        vbbCancel.RegisterEventHandler(this);
        
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
        if(controlOption == ControlOption.Move) {
            ShootLaserFromTargetPosition(frontCard.transform.position, -frontCard.transform.up, laserMaxLength);
        }
        if(Input.GetKeyDown(KeyCode.L)) {
            OnMove();
        }
    }

    void OnMove() {
        if (controlOption == ControlOption.NULL) {
            //Turn on laser so the user can select a unit
            line.SetActive(true);
            controlOption = ControlOption.Move;
            vbbMove.gameObject.GetComponentInChildren<SpriteRenderer>().color = buttonsActivatedColor;
        }
        else {
            OnCancel();
        }
    }

    void OnCancel() {
        SetButtonColors(buttonsStartColor);
        line.SetActive(false);
        controlOption = ControlOption.NULL;
    }

    void SetButtonColors(Color color) {
        foreach(VirtualButtonBehaviour vbb in virtualButtonBehaviours) {
            vbb.gameObject.GetComponentInChildren<SpriteRenderer>().color = color;
        }
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length) {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        Debug.DrawRay(targetPosition, direction * length, Color.green);

        currentLaserPosition = Vector3.negativeInfinity;

        if (Physics.Raycast(ray, out raycastHit, length)) {
            currentLaserPosition = raycastHit.point;
            Client.LocalClient.SetUnitFlag(Client.LocalClient.baseCore.transform.InverseTransformPoint(currentLaserPosition), Client.LocalClient.team);
            endPosition = raycastHit.point;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}
