using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTagAlong : MonoBehaviour {

    public float distanceFromCamera = 2.0f;
	
	// Update is called once per frame
	void Update () {
        var cameraPos = Camera.main.transform.position;
        var myPos = cameraPos + new Vector3(0f, 0f, distanceFromCamera);
        this.transform.position = myPos;
	}
}
