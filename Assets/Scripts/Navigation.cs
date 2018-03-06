using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{

    public GameObject DebugTextDisplay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenNavigationMenu()
    {
        Debug.Log("Hello");
        DebugTextDisplay.GetComponent<DebugScript>().SetDebugMessage("It works!");
    }
}
