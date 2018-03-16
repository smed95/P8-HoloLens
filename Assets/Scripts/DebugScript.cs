using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    private TextMesh textMesh;

	// Use this for initialization
	void Start ()
	{
	    textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDebugMessage(string debugText)
    {
        textMesh.text = debugText;
    }

}
