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
    public float stunTimer = -0.1f;
    public float stunDuration = 2.0f;
    public Text pointText;
    public float points = 0.0f;
    public static int maxPoints = 500;
    public Animator animator;
    public bool falling = false;

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
                if(stunTimer < 0) {
                    //Send message to clients to stop stun animation
                    Client.UnitStun(team, false);
                }
            }
        }
    }

    public void SetSpeed(float time, float multiplier) {
        StartCoroutine(SetSpeedOverTime(time, multiplier));
        Debug.Log("IEnumerator activated");
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
            if (other.tag == "Ravine") {
                Stun();
                myRigidBody.useGravity = true;
                myRigidBody.isKinematic = false;
                Client.UnitFalling(team);
            }
            else if (other.tag == "Mountain" || other.tag == "Tree") {
                transform.parent.position = transform.parent.position - (transform.parent.forward * 0.05f);
                Client.SyncUnitOnce(team);
                if (points >= 100) {
                    points -= 100;
                }
                else {
                    points = 0;
                }
                Client.SyncUnitPoints(team, points);
                if (GetComponentInChildren<Egg>() != null) {
                    GetComponentInChildren<Egg>().SetSpawn();
                }
                Stun();
            }
        }

        if (other.tag == "Spawn") {
            myRigidBody.useGravity = false;
            myRigidBody.isKinematic = true;
            Client.UnitFalling(team);
            transform.localPosition = Vector3.zero;

            Client.RespawnUnitServer(team);
            if (points >= 100) {
                points -= 100;
            }
            else {
                points = 0;
            }
            Client.SyncUnitOnce(team);
            Client.SyncUnitPoints(team, points);

            if (GetComponentInChildren<Egg>() != null) {
                GetComponentInChildren<Egg>().SetSpawn();
            }
        }

    }

    private void Stun() {
        //Send message to clients to start stun animation
        Client.UnitStun(team, true);
        stunTimer = stunDuration;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stealRange);
    }
}
