using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MyDefaultTrackableEventHandler : DefaultTrackableEventHandler {

    protected override void OnTrackingFound() {
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }

    protected override void OnTrackingLost() {
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }
}
