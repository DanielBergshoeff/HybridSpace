using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour {

    public Humanoid owner = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered!");
        if (Client.LocalClient != null) {
            if (!Client.LocalClient.isServer) { return; }
        }

        if (other.gameObject.GetComponent<Humanoid>() != null) {
            if(other.gameObject.GetComponent<Humanoid>() != owner) {
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
}
