using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Animations;

public class Humanoid : NetworkBehaviour {

    public float speed = 0.3f;
    public float stealRange = 0.3f;
    public TeamType team;
    public float stunTimer = 0.0f;
    public float stunDuration = 2.0f;
    public Text pointText;
    public float points = 0.0f;
    public static int maxPoints = 500;
    public Animator animator;

    public Rigidbody myRigidBody;

    public int intPoints = 0;

    public void AddPoints() {
        Debug.Log("Points changed");
        points += Time.deltaTime * 10;
        if (Mathf.RoundToInt(points) > intPoints) {
            intPoints = Mathf.RoundToInt(points);
            Client.SyncUnitPoints(team, points);
            if(intPoints >= maxPoints) {
                Client.GameOver(team);
            }
        }
    }

    private void Start() {
        myRigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (!GameManager.GameOver) {
            pointText.text = Mathf.RoundToInt(points).ToString();

            if (Client.LocalClient != null) {
                if (!Client.LocalClient.isServer) { return; }
            }

            if (stunTimer >= 0.0f) {
                stunTimer -= Time.deltaTime;
            }
        }
    }

    public void SetSpeed(float time, float multiplier) {
        SetSpeedOverTime(time, multiplier);
    }

    IEnumerator SetSpeedOverTime(float time, float multiplier) {
        Debug.Log("Set speed to " + (speed * multiplier).ToString());
        speed *= multiplier;
        yield return new WaitForSeconds(time);
        Debug.Log("Set speed to " + (speed / multiplier).ToString());
        speed /= multiplier;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered!");
        if (Client.LocalClient != null) {
            if (!Client.LocalClient.isServer) { return; }
        }

        if (stunTimer <= 0.0f) {
            if (other.tag == "Ravine" || other.tag == "Mountain") {
                Client.RespawnUnitServer(team);
                stunTimer = stunDuration;
                if(points >= 100) {
                    points -= 100;
                }
                else {
                    points = 0;
                }
                Client.SyncUnitPoints(team, points);

                if (GetComponentInChildren<Egg>() != null) {
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
