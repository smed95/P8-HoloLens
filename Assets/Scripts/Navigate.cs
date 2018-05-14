using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigate : MonoBehaviour, IInputClickHandler, IInputHandler
{
    RoomMenu navigationMenu;

    // Use this for initialization
    void Start()
    {
        navigationMenu = GetComponentInParent<RoomMenu>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //StartNavigation();
    }
    public void OnInputDown(InputEventData eventData)
    {
        StartNavigation();
    }
    public void OnInputUp(InputEventData eventData)
    { }

    void StartNavigation()
    {
        navigationMenu.Navigate(GetComponentInChildren<Text>().text);
    }
}
