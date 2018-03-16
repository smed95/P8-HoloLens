using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationMenu : MonoBehaviour
{
    // An instance of the Graph class
    public Graph graph;
    // The GameObject containing the entire navigation menu
    public GameObject navigationMenu;
    // Dictionary with the nodes, which are possible destination for the user
    Dictionary<int, Node> destinationNodes = new Dictionary<int, Node>();
    // Boolean to check if the destination nodes have been initialized
    bool destinationNodesInitialized = false;
    // The Canvas object, which the navigation menu ui is made upon
    public Canvas navigationMenuCanvas;
    // Prefab of the destination button
    GameObject destinationButtonPrefab;
    // Prefab of the sort button
    GameObject filterButtonPrefab;
    // A dictionary containing all the id of the node and the gameobject of the button
    Dictionary<int, GameObject> buttonsInScene = new Dictionary<int, GameObject>();
    // Different tags, which is to be used to make the different filter functionalities.
    List<string> differentTypes = new List<string>();
    // A Button object for the next and previous button
    public Button prevButton;
    public Button nextButton;

    private float _y = -47.5f;
    private float _yAfterSortButtons;
    private int currentPageNr = 0;
    private int previousPageNr = 0;
    private int destinationsPrPage = 10;

    // Use this for initialization
    void Start()
    {
        destinationButtonPrefab = (GameObject)Resources.Load("Prefabs/DestinationButton");
        filterButtonPrefab = (GameObject)Resources.Load("Prefabs/FilterButton");
    }

    // Update is called once per frame
    void Update()
    {
        // If the Graph class have initialized its nodes and the destination nodes have not yet been initialized
        if(graph.isNodesInitialized && !destinationNodesInitialized)
        {
            // Get the destination ndoes from the Graph class
            destinationNodes = graph.destinationNodes;

            // Getting the different tags needed from the destinations nodes
            foreach (var dn in destinationNodes)
            {
                if (!differentTypes.Contains(dn.Value.Type))
                {
                    differentTypes.Add(dn.Value.Type);
                }
            }
            
            // Setting the height of the canvas, to adjust to the amount of buttons needed in the UI
            RectTransform rt = navigationMenuCanvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, CalculateCanvasHeight(destinationNodes.Count, differentTypes.Count + 1));

            // Setting the height of the panel(called Background), to adjust to the amount of buttons needed in the UI
            RectTransform rectT = GetComponent<RectTransform>();
            rectT.sizeDelta = new Vector2(300, CalculateCanvasHeight(destinationNodes.Count, differentTypes.Count + 1));

            SetFilterButtons();

            SetButtonPositions(destinationNodes);

            destinationNodesInitialized = true;
        }
    }

    // Method used to calculate the needed height for the canvas and the panel, set in the update method
    float CalculateCanvasHeight(int nodeCount, int tagCount)
    {
        float nodesHeight = 0;
        if(nodeCount < 9)
            nodesHeight = (Mathf.Ceil(nodeCount / 2) * 35);
        else
            nodesHeight = 175;
        // Added 85 to get the right y placement, for the first buttons.
        float filterHeight = (Mathf.Ceil(tagCount / 3) * 30) + 85 + 25;
        return nodesHeight + filterHeight;
    }

    // Method to set the positions of the destination buttons.
    void SetButtonPositions(Dictionary<int, Node> destNodes)
    {
        int leftX = 5;
        int rightX = 155;
        int i = 0;

        foreach (var dn in destNodes)
        {
            GameObject destinationButton = Instantiate(destinationButtonPrefab);
            if (i % 2 == 0)
            {
                destinationButton.transform.SetParent(transform, true);
                destinationButton.transform.localPosition = new Vector3(leftX, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
            }
            else if (i % 2 == 1)
            {
                destinationButton.transform.SetParent(transform, true);
                destinationButton.transform.localPosition = new Vector3(rightX, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
                _y -= 35;
            }
            buttonsInScene.Add(dn.Key, destinationButton);
            i++;
        }
        UpdateButtons();
        prevButton.transform.localPosition = new Vector3(110, _y, 0);
        nextButton.transform.localPosition = new Vector3(160, _y, 0);
    }

    // Method to update hte positions of the destination buttons, e.g. used in the SortDestinations method
    void UpdateButtonPositions(Dictionary<int, GameObject> destNodes)
    {
        int leftX = 5;
        int rightX = 155;
        int i = 0;
        _y = _yAfterSortButtons;
        foreach (var dn in destNodes)
        {
            if (i % 2 == 0)
            {
                dn.Value.transform.localPosition = new Vector3(leftX, _y, 0);
            }
            else
            {
                dn.Value.transform.localPosition = new Vector3(rightX, _y, 0);
                _y -= 35;
            }
            i++;
        }
    } 

    // This method sorts the destination buttons based on a parsed query, which is the tags found before
    public void FilterDestinations(string query)
    {
        ActivateDestinations(currentPageNr, false);
        currentPageNr = 0;
        previousPageNr = 0;
        ActivateDestinations(currentPageNr, true);
     
        Dictionary<int, Node> nodesToShow = new Dictionary<int, Node>();
        Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();
        if (query == "")
        {
            nodesToShow = destinationNodes;
        }
        else
        {
            foreach (var dn in destinationNodes)
            {
                if (dn.Value.Type != query)
                {
                    buttonsInScene[dn.Key].SetActive(false);
                }
                else
                {
                    nodesToShow.Add(dn.Key, dn.Value);
                    buttonsToReposition.Add(dn.Key, buttonsInScene[dn.Key]);
                }
            }
        }

        UpdateButtonPositions(buttonsToReposition);

        foreach (var nts in nodesToShow)
        {
            buttonsInScene[nts.Key].SetActive(true);
        }
    }

    // Used to clear the sorting
    public void ClearSort()
    {
        ActivateDestinations(currentPageNr, false);
        currentPageNr = 0;
        previousPageNr = 0;
        ActivateDestinations(currentPageNr, true);
    }

    // Used to activate the needed destination buttons, for the pagination of the destination buttons. 
    void ActivateDestinations(int pageNr, bool activate = true)
    {
        Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();
        int startIndex = pageNr * destinationsPrPage;
        int endIndex = (pageNr + 1) * destinationsPrPage;
        if (endIndex > buttonsInScene.Count)
            endIndex = buttonsInScene.Count;

        int i = 0;
        foreach (var btn in buttonsInScene)
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
        if ((currentPageNr + 1) * destinationsPrPage > buttonsInScene.Count)
        {
            Debug.Log("Disabled next");
            return;
        }
        else
        {
            previousPageNr = currentPageNr;
            currentPageNr += 1;
            UpdateButtons();
        }
    }

    // Used to go to the previous page of the destination buttons, if possible
    public void PrevPage()
    {
        if ((currentPageNr - 1) * destinationsPrPage < 0)
        {
            Debug.Log("Disabled prev");
            return;
        }
        else
        {
            previousPageNr = currentPageNr;
            currentPageNr -= 1;
            UpdateButtons();
        }
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
                x += 95;
                i++;
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
            i++;
        }
        _y -= 25;
        _yAfterSortButtons = _y;
    }

    // Used to start the navigation to an end point. Calls the "FindShortestPath" form the Graph class.
    public void Navigate(string nodeName)
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
    }

    // Opens the navigation menu game object
    public void OpenNavigaitonMenu()
    {
        navigationMenu.gameObject.SetActive(true);
    }

    // Closes the navigation menu game object
    public void CloseNavigationMenu()
    {
        navigationMenu.gameObject.SetActive(false);
    }
}
