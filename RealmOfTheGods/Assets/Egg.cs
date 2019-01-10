using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour {

    public Humanoid owner = null;

    private static List<Vector3> spawnPositions = new List<Vector3>() {
        new Vector3(0, 0.175f, 0)
    };

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other) {
        if (Client.LocalClient != null) {
            if (!Client.LocalClient.isServer) { return; }
        }

        if (other.gameObject.tag == "Humanoid") {
            //Set humanoid as new owner and parent
            if (other.gameObject.GetComponent<Humanoid>() != null) {
                if (other.gameObject.GetComponent<Humanoid>() != owner) {
                    if (other.gameObject.GetComponent<Humanoid>().stunTimer <= 0.0f) {
                        if (owner != null) {
                            owner.stunTimer = owner.stunDuration;
                        }
                        transform.parent = other.gameObject.transform;
                        transform.localPosition = new Vector3(0, 0, 0);
                        owner = other.gameObject.GetComponent<Humanoid>();
                    }
                }
            }
        }
        else if(other.gameObject.tag == "Mountain") {
            //Break egg and respawn it in different spot
            if(owner != null) {
                owner.stunTimer = owner.stunDuration;
            }
            SetSpawn();
        }
    }

    public void SetSpawn() {
        owner = null;
        transform.parent = Client.baseCore.transform;
        transform.localPosition = spawnPositions[Random.Range(0, spawnPositions.Count)];
    }
}
