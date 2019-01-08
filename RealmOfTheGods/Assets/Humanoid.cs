using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour {

    public float speed = 0.3f;
    public float stealRange = 0.3f;
    public TeamType team;
    public GameObject Egg = null;
    public float stunTimer = 0.0f;
    public float stunDuration = 2.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Client.LocalClient != null) {
            if (!Client.LocalClient.isServer) { return; }
        }

        if(stunTimer >= 0.0f) {
            stunTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered!");
        if (Client.LocalClient != null) {
            if (!Client.LocalClient.isServer) { return; }
        }

        if (stunTimer <= 0.0f) {
            if (other.tag == "Ravine") {
                Client.RespawnUnitServer(team);
                stunTimer = stunDuration;

                if(GetComponentInChildren<Egg>() != null) {
                    GetComponentInChildren<Egg>().SetSpawn();
                }
            }
        }
    }

    private void TakeEgg(GameObject egg) {
        Egg = egg;
        Egg.transform.parent = transform;
        Egg.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stealRange);
    }
}
