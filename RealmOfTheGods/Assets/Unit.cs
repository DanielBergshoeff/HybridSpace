using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    public float speed = 3.0f;
    [SerializeField] private float health = 10.0f;
    public float attackRange = 1.0f;
    public float timePerAttack = 1.0f;

    public float Health {
        get {
            return health;
        }
        set {
            healthSlider.value = health / totalHealth;
            health = value;
        }
    }

    public float timePerAttackCheck = 0.1f;


    private float timePassed = 0.0f;
    private float attackTimer = 0.0f;
    private Unit target;
    private float targetDistance = 0.0f;

    private float totalHealth;

    public GameObject prefabHealthSlider;
    public GameObject prefabCanvas;
    private Slider healthSlider;

    public static GameObject canvas;


    // Use this for initialization
    void Start () {
        totalHealth = Health;
        if (canvas == null)
            canvas = Instantiate(prefabCanvas);
        healthSlider = Instantiate(prefabHealthSlider, transform.position + transform.parent.up * 0.3f, Quaternion.identity, canvas.transform).GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        healthSlider.transform.position = transform.position + transform.parent.up * 0.3f;

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
                    //Set target script if unit is the closest one from the colliders hit or no other colliders were found
                    if (Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position) < targetDistance || targetDistance == 0.0f) {
                        target = unitFromCollider;
                        targetDistance = Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position);
                    }
                }
            }
            timePassed = 0.0f;
        }

        if (target != null) {
            attackTimer -= Time.deltaTime;
            //If the timer reaches 0 -> attack
            if (attackTimer <= 0.0f) {
                target.Health -= 1.0f;
                attackTimer = timePerAttack;
            }
        }
	}

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
