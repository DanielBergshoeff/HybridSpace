using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour {

    public float speed = 3.0f;
    [SyncVar] [SerializeField] private float health = 10.0f;
    public float attackRange = 1.0f;
    public float timePerAttack = 1.0f;

    public float Health {
        get {
            return health;
        }
        set {
            healthSlider.value = value / totalHealth;
            if (value <= 0)
                alive = !alive;
            health = value;
        }
    }

    public float timePerAttackCheck = 0.1f;

    public TeamType team;

    //Temporary for showing team colours
    public GameObject meshes;

    private float timePassed = 0.0f;
    private float attackTimer = 0.0f;
    private Unit target;
    private float targetDistance = 0.0f;
    public bool alive = true;

    private float totalHealth;

    public GameObject prefabHealthSlider;
    public GameObject prefabCanvas;
    private Slider healthSlider;

    public static GameObject canvas;


    // Use this for initialization
    void Start () {
        Renderer[] renderers = meshes.GetComponentsInChildren<Renderer>();
        if (team == TeamType.Magic) {
            foreach (Renderer r in renderers) {
                r.material.color = Color.green;
            }
        }
        else if(team == TeamType.Might) {
            foreach (Renderer r in renderers) {
                r.material.color = Color.yellow;
            }
        }

        totalHealth = Health;
        if (canvas == null)
            canvas = Instantiate(prefabCanvas);
        healthSlider = Instantiate(prefabHealthSlider, transform.position + transform.parent.up * 0.3f, Quaternion.identity, canvas.transform).GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        healthSlider.value = health / totalHealth;
        healthSlider.transform.position = transform.position + transform.parent.up * 0.3f;

        if (Client.LocalClient != null) {
            if(!Client.LocalClient.isServer) { return; }
        }

        if (alive) {
            //If a target has been set and is still alive
            if (target != null && target.alive) {
                attackTimer -= Time.deltaTime;
                //If the timer reaches 0 -> attack
                if (attackTimer <= 0.0f) {
                    target.Health -= 1.0f;
                    attackTimer = timePerAttack;
                }
            }
            else {
                attackTimer = timePerAttack;
            }

            timePassed += Time.deltaTime;
            //Check for attack possibilities every 'timePerAttackCheck' seconds
            if (timePassed > timePerAttackCheck) {
                //Check in sphere around unit
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
                target = null;
                targetDistance = 0.0f;
                for (int i = 0; i < hitColliders.Length; i++) {
                    Unit unitFromCollider = hitColliders[i].GetComponent<Unit>();
                    if (unitFromCollider != null) {
                        //Set target script if unit is the closest one from the colliders hit or no other colliders were found
                        if ((Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position) < targetDistance || targetDistance == 0.0f) && unitFromCollider.alive && team != unitFromCollider.team) {
                            target = unitFromCollider;
                            targetDistance = Vector3.Distance(unitFromCollider.gameObject.transform.position, transform.position);
                        }
                    }
                }
                timePassed = 0.0f;
            }
        }
	}

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
