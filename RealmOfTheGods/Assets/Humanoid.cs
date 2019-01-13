using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Humanoid : NetworkBehaviour {

    public float speed = 0.3f;
    public float stealRange = 0.3f;
    public TeamType team;
    public float stunTimer = 0.0f;
    public float stunDuration = 2.0f;
    public Text pointText;
    public float points = 0.0f;

    public int intPoints = 0;

    public void AddPoints() {
        Debug.Log("Points changed");
        points += Time.deltaTime * 10;
        if (Mathf.RoundToInt(points) > intPoints) {
            intPoints = Mathf.RoundToInt(points);
            Client.SyncUnitPoints(team, points);
        }
    }

    // Update is called once per frame
    void Update () {
        pointText.text = Mathf.RoundToInt(points).ToString();

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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stealRange);
    }
}
