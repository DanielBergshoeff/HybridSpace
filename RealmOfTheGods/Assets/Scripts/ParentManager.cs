﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentManager : MonoBehaviour {

    public GameObject baseParent;

	// Use this for initialization
	void Start () {
        if (Client.OnBasePlaced == null) {
            Client.OnBasePlaced = new MyGameObjectEvent();
        }
        Client.OnBasePlaced.AddListener(SetBaseParent);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetBaseParent(GameObject go) {
        go.transform.parent = baseParent.transform;
    }
}
