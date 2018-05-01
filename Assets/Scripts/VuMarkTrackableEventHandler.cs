using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuMarkTrackableEventHandler : DefaultTrackableEventHandler {

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        //get id of vumark
        var id = ((VuMarkBehaviour)mTrackableBehaviour).VuMarkTarget.InstanceId.NumericValue;
        Debug.Log(id + ": " + mTrackableBehaviour.transform.position.x + ", " + mTrackableBehaviour.transform.position.z);
        //notify anchorpointsmanager of the newly detected mark and its position and rotation
        AnchorPointsManager.AddActualPoint(Convert.ToInt32(id), mTrackableBehaviour.transform);
        
    }
}
