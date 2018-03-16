﻿using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigate : MonoBehaviour, IInputClickHandler, IInputHandler
{
    NavigationMenu navigationMenu;

    // Use this for initialization
    void Start()
    {
        navigationMenu = GetComponentInParent<NavigationMenu>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Hello");
        StartNavigation();
    }
    public void OnInputDown(InputEventData eventData)
    { Debug.Log("Hello"); }
    public void OnInputUp(InputEventData eventData)
    { }

    void StartNavigation()
    {
        navigationMenu.Navigate(GetComponentInChildren<Text>().text);
    }
}
