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
    void Update() {
        if (!GameManager.GameOver) {
            if (Client.LocalClient != null) {
                if (!Client.LocalClient.isServer) { return; }
            }

            if (owner != null) {
                owner.AddPoints();
            }
        }
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
                            owner.Stun();
                        }
                        transform.parent = other.gameObject.transform;
                        transform.localPosition = new Vector3(-0.01f, 0.02f, 0f);
                        owner = other.gameObject.GetComponent<Humanoid>();
                        Client.SetEggParent(owner.team, transform.localPosition);
                    }
                }
            }
        }
    }

    public void SetSpawn() {
        owner = null;
        transform.parent = Client.baseCore.transform;
        Vector3 newPosition = spawnPositions[Random.Range(0, spawnPositions.Count)];
        transform.localPosition = newPosition;
        Client.SetEggParent(TeamType.Null, newPosition);
    }
}
