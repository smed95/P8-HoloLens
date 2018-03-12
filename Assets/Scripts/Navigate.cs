using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigate : MonoBehaviour, IInputClickHandler, IInputHandler
{
    Destinations destinations;

    // Use this for initialization
    void Start()
    {
        destinations = GetComponentInParent<Destinations>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Hello");
        //SortDestinations();
    }
    public void OnInputDown(InputEventData eventData)
    { Debug.Log("Hello"); }
    public void OnInputUp(InputEventData eventData)
    { }

    void StartNavigation()
    {
        if (GetComponentInChildren<Text>().text == "Clear sort")
            destinations.SortDestinations("");
        else
            destinations.SortDestinations(GetComponentInChildren<Text>().text);
    }
}
