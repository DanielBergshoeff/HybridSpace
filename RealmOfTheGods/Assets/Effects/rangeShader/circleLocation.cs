using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleLocation : MonoBehaviour {
    public Material circleTerrain;
    public GameObject base1;
    public GameObject base2;
    private Vector4 base1Location;
    private Vector4 base2Location;
    void Start () {
       
       // circleTerrain.SetVector("_Center", cubeLocation);
    }
	
	// Update is called once per frame
	void Update () {
        base1Location = base1.transform.position;
        circleTerrain.SetVector("_Center", base1Location);

        base2Location = base2.transform.position;
        circleTerrain.SetVector("_Center2", base2Location);
    }
}
