﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LogThis(string message)
    {
        Debug.Log(message);
    }
}