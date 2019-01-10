using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudParticle : MonoBehaviour {

    public ParticleSystem ps;
    private float timeRadius;
    private float timeVar;
    public float radiusIncrease;
    public float spreadTime;

	void Update () {
        timeVar += Time.deltaTime;
            if (timeVar <= spreadTime)
            {
                timeRadius += radiusIncrease;
            } 
            else
            {
            timeRadius += 0;
        }
           
    

        ParticleSystem.ShapeModule psShape = ps.shape;
        psShape.scale = new Vector3(timeRadius, timeRadius, 4);
    }
}
