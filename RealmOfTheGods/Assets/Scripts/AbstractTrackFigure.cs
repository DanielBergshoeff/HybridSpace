﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

[Serializable]
public class VirtualButtonBehaviourArray
{
    public VirtualButtonBehaviour[] vbBehaviours;
}

public abstract class AbstractTrackFigure : MonoBehaviour, IVirtualButtonEventHandler {
    [SerializeField]
    public VirtualButtonBehaviourArray[] vbBehaviourArray;

    private int currentButtonSet;
    private VirtualButtonBehaviour btnPressed;

    [SerializeField]
    private double timeToPress;

    private double currentTime;

    protected bool completed;

    public virtual void OnButtonPressed(VirtualButtonBehaviour vb) {
        Debug.Log("Button " + vb.gameObject.name + " has been pressed!");
        Debug.Log(vb.Pressed);

        if (currentButtonSet < vbBehaviourArray.Length) {
            btnPressed = vb;
        }
    }

    public virtual void CheckButtonsPressed(VirtualButtonBehaviour vb) {
        bool nextSet = true;
        bool correctSet = false;
        bool lastSet = false;
        bool anySet = false;

        foreach (VirtualButtonBehaviourArray vbba in vbBehaviourArray) {
            foreach (VirtualButtonBehaviour vbb in vbba.vbBehaviours) {
                if(vbb == vb) {
                    anySet = true;
                }
            }
        }

        foreach (VirtualButtonBehaviour vbb in vbBehaviourArray[currentButtonSet].vbBehaviours) {
            if (!vbb.Pressed) {
                nextSet = false;
                Debug.Log("Not next set");
            }
            if (vbb == vb) {
                correctSet = true;
                Debug.Log("Correct set");
            }
        }

        if (currentButtonSet > 0) {
            foreach (VirtualButtonBehaviour vbb in vbBehaviourArray[currentButtonSet - 1].vbBehaviours) {
                if (vbb == vb) {
                    lastSet = true;
                    Debug.Log("Last set");
                }
            }
        }

        if (nextSet && correctSet) {
            currentButtonSet++;
            currentTime = 0;
            Debug.Log(currentButtonSet);

            if (currentButtonSet >= vbBehaviourArray.Length) {
                OnCompletedFigure();
            }
        }
        else if (!correctSet && !lastSet && anySet) {
            Debug.Log("Not the correct set");
            OnFigureFailed();
        }

        btnPressed = null;
    }

    public virtual void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        
    }

    protected virtual void OnCompletedFigure() {
        completed = true;
    }

    protected virtual void OnFigureFailed()
    {
        Debug.Log("Figure failed");
        currentButtonSet = 0;
        currentTime = 0;
    }

    protected virtual void Start()
    {
        currentButtonSet = 0;

        foreach (VirtualButtonBehaviourArray vbarray in vbBehaviourArray)
        {
            for (int i = 0; i < vbarray.vbBehaviours.Length; i++)
            {
                vbarray.vbBehaviours[i].GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
            }
        }
    }

    protected virtual void Update()
    {
        if (currentButtonSet > 0 && !completed)
        {
            currentTime += Time.deltaTime;
            if(currentTime > timeToPress)
            {
                OnFigureFailed();
            }
        }

        if(btnPressed != null) {
            CheckButtonsPressed(btnPressed);
        }
    }

}
