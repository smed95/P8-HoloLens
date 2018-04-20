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
        Debug.Log(((VuMarkBehaviour)mTrackableBehaviour).VuMarkTarget.InstanceId.NumericValue);
        //Vector2 point = new Vector2(mTrackableBehaviour.transform.position.x, mTrackableBehaviour.transform.position.y);
        //AnchorPointsManager.AddActualPoint(Convert.ToInt32(id), point);
        Vector2 point;
        if (id == 1)
        {
            point = new Vector2(2, 5);
        }else
        {
            point = new Vector2(-0.3f, 7.3f);
        }
        AnchorPointsManager.AddActualPoint(Convert.ToInt32(id), point);
    }
}
