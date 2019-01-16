using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MyDefaultTrackableEventHandler : DefaultTrackableEventHandler {

    public override void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus) {

        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED
            ) {
            Debug.Log("Trackable (DEFAULT)" + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE ||
                 newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
            Debug.Log("Trackable (DEFAULT) " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }

    }

    protected override void OnTrackingFound() {
        base.OnTrackingFound();
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        UnitSelectionManager.TurnLaserOn(mTrackableBehaviour.gameObject);
    }

    protected override void OnTrackingLost() {
        base.OnTrackingLost();
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        UnitSelectionManager.TurnLaserOff(mTrackableBehaviour.gameObject);
    }
}
