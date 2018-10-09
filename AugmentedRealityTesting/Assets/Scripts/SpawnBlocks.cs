using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour {

    public GameObject prefabCube;
    public double timePerSpawn;

    private double timePassed = 0.0;

	// Update is called once per frame
	void Update () {

        timePassed += Time.deltaTime;

        if (timePassed >= timePerSpawn)
        {
            timePassed = 0.0;
            Instantiate(prefabCube, transform.position, Quaternion.identity, transform);
        }
	}
}
