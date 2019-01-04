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
        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = Vector3.zero;
        //canvas.SetActive(false);
    }

    public void SpawnUnitRed() {
        Client.LocalClient.SpawnUnitClient(TeamType.Red);
    }

    public void SpawnUnitBlue() {
        Client.LocalClient.SpawnUnitClient(TeamType.Blue);
    }

    public void SpawnUnitGreen() {
        Client.LocalClient.SpawnUnitClient(TeamType.Green);
    }

    public void SpawnUnitYellow() {
        Client.LocalClient.SpawnUnitClient(TeamType.Yellow);
    }
}
