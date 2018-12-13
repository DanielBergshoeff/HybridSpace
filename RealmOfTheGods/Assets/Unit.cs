using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float speed = 3.0f;
    public float attackRange = 1.0f;
    public float timePerAttack = 1.0f;

    public float timePerAttackCheck = 0.1f;


    private float timePassed = 0.0f;
    private float attackTimer = 0.0f;
    private Unit target;
    private float targetDistance = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime;
        //Check for attack possibilities every TIMEPERATTACKCHECK seconds
        if(timePassed > timePerAttackCheck) {
            //Check in sphere around unit
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            target = null;
            targetDistance = 0.0f;
            for (int i = 0; i < hitColliders.Length; i++) {
                Unit unitFromCollider = hitColliders[i].GetComponent<Unit>();
                if(unitFromCollider != null) {
                    if(Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position) < targetDistance || targetDistance == 0.0f) {
                        //Set target script if unit is the closest one from the colliders hit or no other colliders were found
                        target = unitFromCollider;
                        targetDistance = Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position);
                    }
                }
            }
            timePassed = 0.0f;
        }

        if (target != null) {
            if (attackTimer <= 0.0f) {


                attackTimer = timePerAttack;
            }
        }
	}

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
