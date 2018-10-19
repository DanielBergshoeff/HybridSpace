using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour {

    public GameObject spherePrefab;

    public float timePerSpawn;
    public float forceUpward;

    private float timer;

	// Use this for initialization
	void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer >= timePerSpawn) {
            GameObject go = Instantiate(spherePrefab, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0, forceUpward, 0));
            timer = 0;
        }
	}
}
