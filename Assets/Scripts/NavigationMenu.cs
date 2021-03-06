﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationMenu : MonoBehaviour
{
    // An instance of the Graph class
    public Graph graph;
    // The GameObject containing the entire navigation menu
    public GameObject navigationMenu;
    //
    public GameObject roomMenuCanvas;
    // Dictionary with the nodes, which are possible destination for the user
    public Dictionary<int, Node> destinationNodes = new Dictionary<int, Node>();
    // Boolean to check if the destination nodes have been initialized
    bool destinationNodesInitialized = false;
    // The Canvas object, which the navigation menu ui is made upon
    public Canvas navigationMenuCanvas;
    // Prefab of the sort button
    GameObject filterButtonPrefab;
    // A dictionary containing all the id of the node and the gameobject of the button
    Dictionary<int, GameObject> buttonsInScene = new Dictionary<int, GameObject>();
    // Different tags, which is to be used to make the different filter functionalities.
    List<string> differentTypes = new List<string>();
    //list for filterButtons.
    List<GameObject> filterButtons = new List<GameObject>();
    // A Button object for the next and previous button these has to be set in unity
    // They are found by clicking the circle next to the empty field and selecting them in the scene tab
    public Button prevButton;
    public Button nextButton;

    private float _y = -47.5f;
    private float _yAfterSortButtons;
    private int currentPageNr = 0;
    private int previousPageNr = 0;
    private int destinationsPrPage = 12;

    // Use this for initialization
    void Start()
    {
        filterButtonPrefab = (GameObject)Resources.Load("Prefabs/FilterButton");
    }

    // Update is called once per frame
    void Update()
    {
        // If the Graph class have initialized its nodes and the destination nodes have not yet been initialized
        if(graph.isNodesInitialized && !destinationNodesInitialized)
        {
            // Get the destination nodes from the Graph class
            destinationNodes = graph.destinationNodes;

            // Getting the different tags needed from the destinations nodes
            foreach (var dn in destinationNodes)
            {
                if (!differentTypes.Contains(dn.Value.Type))
                {
                    differentTypes.Add(dn.Value.Type);
                }
            }

            // Sort the list alphabetically
            differentTypes.Sort();

            //float height = CalculateCanvasHeight(destinationNodes.Count, differentTypes.Count + 1);
            //
            //// Setting the height of the canvas, to adjust to the amount of buttons needed in the UI
            //RectTransform rt = navigationMenuCanvas.GetComponent<RectTransform>();
            //rt.sizeDelta = new Vector2(300, height);
            //
            //// Setting the height of the panel(called Background), to adjust to the amount of buttons needed in the UI
            //RectTransform rectT = GetComponent<RectTransform>();
            //rectT.sizeDelta = new Vector2(300, height);
            //
            //BoxCollider bc = navigationMenu.GetComponent<BoxCollider>();
            //bc.size = new Vector3(300, height, 0.01f);
            //bc.center = new Vector3(0, 0, 0.025f);

            SetFilterButtons();


            UpdateButtons();
            InitialPrevAndNextButtonCheck();
            destinationNodesInitialized = true;
        }
    }

    // Method used to calculate the needed height for the canvas and the panel, set in the update method
    //float CalculateCanvasHeight(int nodeCount, int tagCount)
    //{
    //    float nodesHeight = 0;
    //    if(nodeCount < 9)
    //        nodesHeight = (Mathf.Ceil(nodeCount / 2) * 35);
    //    else
    //        nodesHeight = 175;
    //    // Added 85 to get the right y placement, for the first buttons.
    //    float filterHeight = (Mathf.Ceil(tagCount / 3) * 30) + 85 + 25;
    //    return nodesHeight + filterHeight;
    //}

    // Method to set the positions of the destination buttons.
    //void SetButtonPositions(Dictionary<int, Node> destNodes)
    //{
    //    int leftX = 5;
    //    int rightX = 155;
    //    int i = 0;

    //    foreach (var dn in destNodes)
    //    {
    //        GameObject destinationButton = Instantiate(destinationButtonPrefab);
    //        if (i % 2 == 0)
    //        {
    //            destinationButton.transform.SetParent(transform, true);
    //            destinationButton.transform.localPosition = new Vector3(leftX, _y, 0);
    //            destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
    //            destinationButton.transform.localScale = new Vector3(1, 1, 1);
    //            destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
    //        }
    //        else if (i % 2 == 1)
    //        {
    //            destinationButton.transform.SetParent(transform, true);
    //            destinationButton.transform.localPosition = new Vector3(rightX, _y, 0);
    //            destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
    //            destinationButton.transform.localScale = new Vector3(1, 1, 1);
    //            destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
    //            _y -= 35;
    //        }
    //        buttonsInScene.Add(dn.Key, destinationButton);
    //        i++;
    //    }
    //    UpdateButtons();
    //    prevButton.transform.localPosition = new Vector3(110, _y, 0);
    //    nextButton.transform.localPosition = new Vector3(160, _y, 0);
    //}

    // Method to update the positions of the destination buttons, e.g. used in the SortDestinations method
    void UpdateButtonPositions(List<GameObject> filterButtons)
    {
        int x = 55;
        int i = 0;
        _y = -47.5f;
        foreach (var dn in filterButtons)
        {
            if (i % 3 == 2)
            {
                dn.transform.localPosition = new Vector3(x, _y, 0);
                x = 55;
                _y -= 30;
            }
            else
            {
                dn.transform.localPosition = new Vector3(x, _y, 0);
                x += 95;
            }
            i++;
        }
    }

    // This method filters the destination buttons based on a parsed query, which is the tags found before
    //public void FilterDestinations(string query)
    //{
    //    ActivateDestinations(currentPageNr, false);
    //    currentPageNr = 0;
    //    previousPageNr = 0;
    //    ActivateDestinations(currentPageNr, true);
    // 
    //    Dictionary<int, Node> nodesToShow = new Dictionary<int, Node>();
    //    Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();
    //    if (query == "")
    //    {
    //        nodesToShow = destinationNodes;
    //    }
    //    else
    //    {
    //        foreach (var dn in destinationNodes)
    //        {
    //            if (dn.Value.Type != query)
    //            {
    //                buttonsInScene[dn.Key].SetActive(false);
    //            }
    //            else
    //            {
    //                nodesToShow.Add(dn.Key, dn.Value);
    //                buttonsToReposition.Add(dn.Key, buttonsInScene[dn.Key]);
    //            }
    //        }
    //    }
    //
    //    UpdateButtonPositions(buttonsToReposition);
    //
    //    foreach (var nts in nodesToShow)
    //    {
    //        buttonsInScene[nts.Key].SetActive(true);
    //    }
    //}

    public void FilterDestinations(string query)
    {
        // Disable NavigationMenuCanvas
        this.transform.parent.gameObject.SetActive(false);

        // Activate the RoomMenuCanvas found on the NavigationMenu object
        Transform roomMenuCanvas = this.transform.parent.parent.Find("RoomMenuCanvas");
        roomMenuCanvas.gameObject.SetActive(true);

        // Find the RoomMenu and activate the filterscript to with the selected catagory filter
        roomMenuCanvas.GetChild(0).GetComponent<RoomMenu>().FilterRooms(query);
    }

    // Used to clear the filtering
    public void ClearFilter()
    {
        ActivateDestinations(currentPageNr, false);
        currentPageNr = 0;
        previousPageNr = 0;
        ActivateDestinations(currentPageNr, true);
    }

    // Used to activate the needed destination buttons, for the pagination of the destination buttons. 
    void ActivateDestinations(int pageNr, bool activate = true)
    {
        List<GameObject> buttonsToReposition = new List<GameObject>();
        int startIndex = pageNr * destinationsPrPage;
        int endIndex = (pageNr + 1) * destinationsPrPage;
        if (endIndex > filterButtons.Count)
            endIndex = filterButtons.Count;

        int i = 0;
        foreach (var btn in filterButtons)
        {
            if (i >= startIndex && i < endIndex)
            {
                btn.SetActive(activate);
                buttonsToReposition.Add(btn);
            }
            else
                btn.SetActive(!activate);
            i++;
        }
        UpdateButtonPositions(buttonsToReposition);
    }

    // Used to go to the next page of the destination buttons, if possible
    public void NextPage()
    {
        if ((currentPageNr + 2) * destinationsPrPage >= differentTypes.Count)
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


    // Used to update the updarte the destination buttons, setting them either active or not, based on what is needed.
    void UpdateButtons()
    {
        ActivateDestinations(previousPageNr, false);
        ActivateDestinations(currentPageNr, true);
    }

    // Used to position the different sorting buttons, one button for each tag. A static button for "Clear" is already set.
    void SetFilterButtons()
    {
        int x = 55;
        int i = 0;
        foreach (var type in differentTypes)
        {
            if(i == 0)
            {
                // To make room for clear button
                //x += 95;
                //i++;
            }
     
            GameObject filterButton = Instantiate(filterButtonPrefab);
            if (i % 3 == 2)
            {
                filterButton.transform.SetParent(transform);
                filterButton.transform.localPosition = new Vector3(x, _y, 0);
                filterButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                filterButton.transform.localScale = new Vector3(1, 1, 1);
                filterButton.GetComponentInChildren<Text>().text = type;
                x = 55;
                _y -= 30;
            }
            else
            {
                filterButton.transform.SetParent(transform);
                filterButton.transform.localPosition = new Vector3(x, _y, 0);
                filterButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                filterButton.transform.localScale = new Vector3(1, 1, 1);
                filterButton.GetComponentInChildren<Text>().text = type;
                x += 95;
            }
            filterButtons.Add(filterButton);
            i++;
        }
        _y -= 25;
        _yAfterSortButtons = _y;
    }

    // Used to start the navigation to an end point. Calls the "FindShortestPath" form the Graph class.
    /*public void Navigate(string nodeName)
    {
        int nodeId = 0;
        foreach (var dn in destinationNodes)
        {
            //Change to Name property later
            if (dn.Value.Name == nodeName)
            {
                nodeId = dn.Key;
                break;
            }
        }
        graph.FindShortestPath(nodeId);
        CloseNavigationMenu();
    }*/

    // Opens the navigation menu game object
    public void OpenNavigaitonMenu()
    {
        navigationMenu.gameObject.SetActive(true);
        navigationMenuCanvas.gameObject.SetActive(true);
        roomMenuCanvas.gameObject.SetActive(false);
        foreach (var node in destinationNodes)
        {
            node.Value.gameObject.SetActive(false);
        }

        graph.setEdgesInactive();

        graph.setNodesInactive();

    }

    // Closes the navigation menu game object
    /*public void CloseNavigationMenu()
    {
        navigationMenu.gameObject.SetActive(false);
    }*/

    public void FilterAllRooms()
    {
        FilterDestinations("");
    }


    private void InitialPrevAndNextButtonCheck()
    {
        prevButton.gameObject.SetActive(false);
        if (filterButtons.Count < destinationsPrPage)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }
}
