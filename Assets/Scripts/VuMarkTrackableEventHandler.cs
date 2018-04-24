using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuMarkTrackableEventHandler : DefaultTrackableEventHandler {

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        var id = ((VuMarkBehaviour)mTrackableBehaviour).VuMarkTarget.InstanceId.NumericValue;
        Debug.Log(id + ": " + mTrackableBehaviour.transform.position.x + ", " + mTrackableBehaviour.transform.position.z);
        
        Vector2 point = new Vector2(mTrackableBehaviour.transform.position.x, mTrackableBehaviour.transform.position.z);
        AnchorPointsManager.AddActualPoint(Convert.ToInt32(id), point);
        
    }
}
