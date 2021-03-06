﻿using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{
    // Reference to the navigation menu which we return to
    public GameObject NavigationMenuCanvas;
    public Graph graph;
    private float _y = -47.5f;
    // A dictionary containing all the id of the node and the gameobject of the button
    Dictionary<int, GameObject> buttonsInScene = new Dictionary<int, GameObject>();
    // Prefab of the destination button
    GameObject destinationButtonPrefab;
    // Dictionary with the nodes, which are possible destination for the user
    Dictionary<int, Node> _destinationNodes = new Dictionary<int, Node>();
    // Dictionary with the nodes which is presented to the viewer when a filter button is pressed.
    Dictionary<int, GameObject> filteredButtonsInScene = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> buttonsRemovedFromSearch = new Dictionary<int, GameObject>();

    public Button nextButton;
    public Button prevButton;
    public Canvas RoomMenuCanvas;
    public GameObject roomMenu;
    public InputField searchField;
    private int destinationsPrPage = 12;
    private int currentPageNr = 0;
    private int previousPageNr = 0;

    // Use this for initialization
    void Start()
    {
       searchField.onValueChanged.AddListener(delegate { InputFieldSearch(searchField); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This method filters the destination buttons based on a parsed query, which is the tags found before
    public void FilterRooms(string query)
    {
        destinationButtonPrefab = (GameObject)Resources.Load("Prefabs/DestinationButton");

        _destinationNodes = graph.destinationNodes;


        SetButtonPositions(_destinationNodes);

        Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();
        if (query == "")
        {
            foreach (var button in buttonsInScene)
            {
                filteredButtonsInScene.Add(button.Key, buttonsInScene[button.Key]);
                filteredButtonsInScene[button.Key].SetActive(true);
                buttonsToReposition.Add(button.Key, buttonsInScene[button.Key]);
            }
        }

        else
        {
            foreach (var dn in _destinationNodes)
            {
                if (dn.Value.Type != query)
                {
                    buttonsInScene[dn.Key].SetActive(false);
                }
                else
                {
                    filteredButtonsInScene.Add(dn.Key, buttonsInScene[dn.Key]);
                    filteredButtonsInScene[dn.Key].SetActive(true);
                    buttonsToReposition.Add(dn.Key, buttonsInScene[dn.Key]);
                }
            }
        }

        InitialPrevAndNextButtonCheck();

        UpdateButtonPositions(buttonsToReposition);

        UpdateButtons();
    }

    private void InitialPrevAndNextButtonCheck()
    {
        prevButton.gameObject.SetActive(false);
        if (filteredButtonsInScene.Count < destinationsPrPage)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    // Method to set the positions of the destination buttons.
    void SetButtonPositions(Dictionary<int, Node> destNodes)
    {
        int x = 55;
        int i = 0;

        foreach (var dn in destNodes)
        {
            GameObject destinationButton = Instantiate(destinationButtonPrefab);
            if (i % 3 == 2)
            {
                destinationButton.transform.SetParent(transform);
                destinationButton.transform.localPosition = new Vector3(x, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
                x = 55;
                _y -= 30;
            }
            else
            {
                destinationButton.transform.SetParent(transform);
                destinationButton.transform.localPosition = new Vector3(x, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
                x += 95;
            }
            buttonsInScene.Add(dn.Key, destinationButton);
            i++;
        }
    }

    void UpdateButtonPositions(Dictionary<int, GameObject> destNodes)
    {
        int x = 55;
        int i = 0;
        _y = -47.5f;
        foreach (var dn in destNodes)
        {
            if (i % 3 == 2)
            {
                dn.Value.transform.localPosition = new Vector3(x, _y, 0);
                x = 55;
                _y -= 30;
            }
            else
            {
                dn.Value.transform.localPosition = new Vector3(x, _y, 0);
                x += 95;
            }
            i++;
        }
    }

    private void ActivateDestinations(int pageNr, bool activate = true)
    {
        Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();

        int startIndex = pageNr * destinationsPrPage;
        int endIndex = (pageNr + 1) * destinationsPrPage;
        if (endIndex > buttonsInScene.Count)
            endIndex = buttonsInScene.Count;

        int i = 0;
        foreach (var btn in filteredButtonsInScene)
        {
            if (i >= startIndex && i < endIndex)
            {
                btn.Value.SetActive(activate);
                buttonsToReposition.Add(btn.Key, btn.Value);
            }
            else
                btn.Value.SetActive(!activate);
            i++;
        }
        UpdateButtonPositions(buttonsToReposition);
    }

    // Used to go to the next page of the destination buttons, if possible
    public void NextPage()
    {
        if ((currentPageNr + 2) * destinationsPrPage >= filteredButtonsInScene.Count)
        {
            Debug.Log("Setting next page GameObject inactive");
            nextButton.gameObject.SetActive(false);
        }

        previousPageNr = currentPageNr;
        currentPageNr += 1;
        UpdateButtons();
        prevButton.gameObject.SetActive(true);

    }

    // Used to go to the previous page of the destination buttons, if possible
    public void PrevPage()
    {
        if ((currentPageNr - 2) * destinationsPrPage < 0)
        {
            Debug.Log("Setting prev page GameObject inactive");
            prevButton.gameObject.SetActive(false);
        }

        previousPageNr = currentPageNr;
        currentPageNr -= 1;
        UpdateButtons();
        nextButton.gameObject.SetActive(true);
    }

    // Used to update the update the destination buttons, setting them either active or not, based on what is needed.
    void UpdateButtons()
    {
        ActivateDestinations(previousPageNr, false);
        ActivateDestinations(currentPageNr, true);
    }

    public void ReturnButtonClick()
    {
        this.transform.parent.gameObject.SetActive(false);
        NavigationMenuCanvas.SetActive(true);

        ResetMenu();
    }

    private void ResetMenu()
    {
        currentPageNr = 0;
        previousPageNr = 0;

        foreach (var button in buttonsInScene.Values)
        {
            button.SetActive(false);
        }
        buttonsInScene = new Dictionary<int, GameObject>();
        filteredButtonsInScene = new Dictionary<int, GameObject>();
        buttonsRemovedFromSearch=new Dictionary<int, GameObject>();
    }

    // Used to start the navigation to an end point. Calls the "FindShortestPath" form the Graph class.
    public void Navigate(string nodeName)
    {
        int nodeId = 0;
        foreach (var dn in _destinationNodes)
        {
            //Change to Name property later
            if (dn.Value.Name == nodeName)
            {
                nodeId = dn.Key;
                break;
            }
        }
        graph.FindShortestPath(nodeId);
        ResetMenu();
        roomMenu.gameObject.SetActive(false);
    }

    public void InputFieldSearch(InputField input)
    {
        List<int> buttonsToRemoveFromInScene = new List<int>();
        List<int> buttonsToRemoveFromStorage = new List<int>();
        string argument = input.text.Trim(' ');

        foreach (var btn in filteredButtonsInScene)
        {
            var buttonText = btn.Value.GetComponentInChildren<Text>().text;
            if (buttonText.Contains(argument))
            {
                btn.Value.SetActive(true);
            }
            else
            {
                if (filteredButtonsInScene.ContainsKey(btn.Key))
                {
                    buttonsRemovedFromSearch.Add(btn.Key, btn.Value);
                    buttonsToRemoveFromInScene.Add(btn.Key);
                }
            }
        }

        foreach (var button in buttonsRemovedFromSearch)
        {
            var buttonText = button.Value.GetComponentInChildren<Text>().text;
            if (buttonText.Contains(argument))
            {
                button.Value.SetActive(true);
                filteredButtonsInScene.Add(button.Key, button.Value);
                buttonsToRemoveFromStorage.Add(button.Key);
            }
        }

        foreach (var buttonId in buttonsToRemoveFromInScene)
        {
            filteredButtonsInScene[buttonId].SetActive(false);
            filteredButtonsInScene.Remove(buttonId);
        }

        foreach (var buttonId in buttonsToRemoveFromStorage)
        {
            buttonsRemovedFromSearch[buttonId].SetActive(false);
            buttonsRemovedFromSearch.Remove(buttonId);
        }


        UpdateButtonPositions(filteredButtonsInScene);
        ActivateDestinations(currentPageNr, false);
        currentPageNr = 0;
        previousPageNr = 0;
        ActivateDestinations(currentPageNr, true);
    }
}