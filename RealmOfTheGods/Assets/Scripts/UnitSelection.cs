﻿using System.Collections;
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
    public VirtualButtonBehaviour vbbBoost;

    public float boostTimer = 0.0f;
    public float boostCoolDown = 30.0f;

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
    private int layerMask;
    public ControlOption controlOption;

    private List<VirtualButtonBehaviour> virtualButtonBehaviours;

    public void OnButtonPressed(VirtualButtonBehaviour vb) {
        if (vb == vbbMove) {
            OnMove();
        }
        else if (vb == vbbCancel) {
            OnCancel();
        }
        else if(vb == vbbBoost) {
            OnBoost();
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb) {

    }

    public void SetLineActive(bool active) {
        line.SetActive(active);
    }

    // Use this for initialization
    void Start() {
        Debug.Log("Start");
        virtualButtonBehaviours = new List<VirtualButtonBehaviour>();
        vbbMove.RegisterEventHandler(this);
        vbbCancel.RegisterEventHandler(this);
        vbbBoost.RegisterEventHandler(this);
        
        virtualButtonBehaviours.Add(vbbMove);
        virtualButtonBehaviours.Add(vbbCancel);

        SetButtonColors(buttonsStartColor);

        layerMask = LayerMask.GetMask("LaserTarget");

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
        else if (Input.GetKeyDown(KeyCode.P)) {
            OnBoost();
        }
        if(boostTimer >= 0.0f) {
            boostTimer -= Time.deltaTime;
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

    public void OnCancel() {
        SetButtonColors(buttonsStartColor);
        line.SetActive(false);
        controlOption = ControlOption.NULL;
    }

    void OnBoost() {
        Debug.Log("Check boost timer");
        if (boostTimer <= 0) {
            Debug.Log("Boost!");
            Client.LocalClient.BoostUnit(Client.team);
            boostTimer = boostCoolDown;
        }
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

        if (Physics.Raycast(ray, out raycastHit, length, layerMask)) {
            currentLaserPosition = raycastHit.point;
            Client.LocalClient.SetUnitFlag(Client.baseCore.transform.InverseTransformPoint(currentLaserPosition), Client.team);
            endPosition = raycastHit.point;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}
