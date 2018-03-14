using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sort : MonoBehaviour, IInputClickHandler, IInputHandler
{
    NavigationMenu navigationMenu;

	// Use this for initialization
	void Start () {
        navigationMenu = GetComponentInParent<NavigationMenu>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //Debug.Log("Hello");
        SortDestinations();
    }
    public void OnInputDown(InputEventData eventData)
    { /*Debug.Log("Hello");*/ }
    public void OnInputUp(InputEventData eventData)
    { }

    public void SortDestinations()
    {
        if (GetComponentInChildren<Text>().text == "Clear sort")
            navigationMenu.SortDestinations("");
        else
            navigationMenu.SortDestinations(GetComponentInChildren<Text>().text);
    }
}
