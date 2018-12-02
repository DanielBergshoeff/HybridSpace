using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject baseParent;
    public GameObject canvas;

	// Use this for initialization
	void Start () {
        if (Client.OnBasePlaced == null) {
            Client.OnBasePlaced = new MyGameObjectEvent();
        }
        Client.OnBasePlaced.AddListener(BaseSpawned);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnBase() {
        Client.LocalClient.SpawnBaseClient();
    }

    public void BaseSpawned(GameObject go) {
        go.transform.parent = baseParent.transform;
        canvas.SetActive(false);
    }
}
