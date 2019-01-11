using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ostrich : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W)) {
            GetComponent<CharacterController>().Move(Vector3.forward * Time.deltaTime);
        }
	}
}
