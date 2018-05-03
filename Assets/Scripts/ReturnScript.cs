using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ReturnScript : MonoBehaviour, IInputClickHandler, IInputHandler
{
    // Reference to the navigation menu which we return to
    public GameObject NavigationMenuCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
        this.transform.parent.parent.gameObject.SetActive(false);
        NavigationMenuCanvas.SetActive(true);
    }

    public void OnInputDown(InputEventData eventData)
    { /*Debug.Log("Hello");*/ }
    public void OnInputUp(InputEventData eventData)
    { }
}
