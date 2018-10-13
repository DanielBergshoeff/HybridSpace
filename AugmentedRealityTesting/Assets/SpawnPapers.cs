using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPapers : MonoBehaviour {

    public GameObject paperPrefab;
    public GameObject changeMatObject;

    public Material mat1;
    public Material mat2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RegisterModelTouch();
	}

    public void RegisterModelTouch()
    {
        if(Input.touches.Length != 0) {
        Touch touch = Input.touches[0];
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("PaperSpawner"))
            {
                Instantiate(paperPrefab, transform.position + transform.up, Quaternion.identity);
                if(changeMatObject.GetComponent<Renderer>().material != mat1)
                {
                    changeMatObject.GetComponent<Renderer>().material = mat1;
                }
                else
                {
                    changeMatObject.GetComponent<Renderer>().material = mat2;
                }
                
            }
        }
        }
    }
}
