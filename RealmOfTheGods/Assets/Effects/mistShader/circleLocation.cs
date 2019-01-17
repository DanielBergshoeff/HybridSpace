using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleLocation : MonoBehaviour {
    public static circleLocation CircleLocation;
    public bool active = false; 

    public Material circleTerrain;
    public GameObject player;
    public GameObject base2;
    public GameObject base3;
    public GameObject base4;
    public GameObject playground;

    public float minSphereSoftness;
    public float maxSphereSoftness;
    public float playerSphereSoftness;
    public float playerSphereRadius;
    public float maxSphereRadius;

    private float currentSphereRadius;
    private float currentSphereSoftness;
    public float CurrentSphereSoftness {
        get {
            return currentSphereSoftness;
        }
        set {
            if(currentSphereSoftness > minSphereSoftness && value <= minSphereSoftness) {
                softnessGoingUp = true;
            }
            else if(currentSphereSoftness < maxSphereSoftness && value >= maxSphereSoftness) {
                softnessGoingUp = false;
            }
            currentSphereSoftness = value;
        }
    }

    private bool softnessGoingUp;

    private Vector4 base1Location;
    private Vector4 base2Location;
    private Vector4 base3Location;
    private Vector4 base4Location;
    private Vector4 playgroundLocation;

    void Start () {
        Debug.Log("Circle location set");
        // circleTerrain.SetVector("_Center", cubeLocation);
        CircleLocation = this;
        CircleLocation.circleTerrain.SetFloat("_RadiusPlayer", 100);
    }

    public static void ActivateMist() {
        CircleLocation.currentSphereSoftness = CircleLocation.maxSphereSoftness;
        CircleLocation.currentSphereRadius = CircleLocation.maxSphereRadius;
        CircleLocation.circleTerrain.SetFloat("_SoftnessPlayer", CircleLocation.playerSphereSoftness);
        CircleLocation.circleTerrain.SetFloat("_RadiusPlayer", CircleLocation.playerSphereRadius);
        CircleLocation.active = true;
    }

    // Update is called once per frame
    void Update () {
        if (active) {
            if (softnessGoingUp) {
                CurrentSphereSoftness += Time.deltaTime;
                currentSphereRadius -= Time.deltaTime;
            }
            else {
                CurrentSphereSoftness -= Time.deltaTime;
                currentSphereRadius += Time.deltaTime;
            }

            circleTerrain.SetFloat("_Softness", CurrentSphereSoftness);
            circleTerrain.SetFloat("_Radius", currentSphereRadius);


            base1Location = player.transform.position;
            circleTerrain.SetVector("_Center", base1Location);

            base2Location = base2.transform.position;
            circleTerrain.SetVector("_Center2", base2Location);

            base3Location = base3.transform.position;
            circleTerrain.SetVector("_Center3", base3Location);

            base4Location = base4.transform.position;
            circleTerrain.SetVector("_Center4", base4Location);

            playgroundLocation = playground.transform.position;
            circleTerrain.SetVector("_TotalCenter", playgroundLocation);
        }
    }
}
