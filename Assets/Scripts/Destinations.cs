using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destinations : MonoBehaviour
{
    public Graph graph;
    Dictionary<int, Node> destinationNodes = new Dictionary<int, Node>();
    bool destinationNodesInitialized = false;
    public Canvas navigationMenuCanvas;
    GameObject destinationButtonPrefab;
    GameObject sortButtonPrefab;
    Dictionary<int, GameObject> buttonsInScene = new Dictionary<int, GameObject>();
    List<string> differentTags = new List<string>();
    private float _y = -47.5f;
    private float _yAfterSortButtons;
    private int currentPageNr = 0;
    private int previousPageNr = 0;
    private int destinationsPrPage = 10;
    public Button prevButton;
    public Button nextButton;

    // Use this for initialization
    void Start()
    {
        destinationButtonPrefab = (GameObject)Resources.Load("Prefabs/DestinationButton");
        sortButtonPrefab = (GameObject)Resources.Load("Prefabs/SortButton");
    }

    // Update is called once per frame
    void Update()
    {
        if(graph.isNodesInitialized && !destinationNodesInitialized)
        {
            destinationNodes = graph.destinationNodes;

            foreach (var dn in destinationNodes)
            {
                if (!differentTags.Contains(dn.Value.Tag))
                {
                    differentTags.Add(dn.Value.Tag);
                }
            }

            RectTransform rt = navigationMenuCanvas.GetComponent<RectTransform>();
            //rect.height = CalculateCanvasHeight(destinationNodes.Count, differentTags.Count);
            rt.sizeDelta = new Vector2(300, CalculateCanvasHeight(destinationNodes.Count, differentTags.Count + 1));

            RectTransform rectT = GetComponent<RectTransform>();
            rectT.sizeDelta = new Vector2(300, CalculateCanvasHeight(destinationNodes.Count, differentTags.Count + 1));

            SetSortButtons();

            SetButtonPositions(destinationNodes);

            destinationNodesInitialized = true;
        }
    }

    float CalculateCanvasHeight(int nodeCount, int tagCount)
    {
        float nodesHeight = 0;
        if(nodeCount < 9)
            nodesHeight = (Mathf.Ceil(nodeCount / 2) * 35);
        else
            nodesHeight = 175;
        // Added 85 to get the right y placement, for the first buttons.
        float sortHeight = (Mathf.Ceil(tagCount / 3) * 30) + 85;
        return nodesHeight + sortHeight;
    }

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
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Tag;
            }
            else if (i % 2 == 1)
            {
                destinationButton.transform.SetParent(transform, true);
                destinationButton.transform.localPosition = new Vector3(rightX, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Tag;
                _y -= 35;
            }
            buttonsInScene.Add(dn.Key, destinationButton);
            i++;
        }
        prevButton.transform.localPosition = new Vector3(110, _y, 0);
        nextButton.transform.localPosition = new Vector3(160, _y, 0);
        UpdateButtons();
    }

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

    public void SortDestinations(string query)
    {
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
                if (dn.Value.Tag != query)
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

    void UpdateButtons()
    {
        ActivateDestinations(previousPageNr, false);
        ActivateDestinations(currentPageNr, true);
    }

    void SetSortButtons()
    {
        int x = 55;
        int i = 0;
        foreach (var tag in differentTags)
        {
            if(i == 0)
            {
                // To make room for clear button
                x += 95;
                i++;
            }
     
            GameObject sortButton = Instantiate(sortButtonPrefab);
            if (i % 3 == 2)
            {
                sortButton.transform.SetParent(transform);
                sortButton.transform.localPosition = new Vector3(x, _y, 0);
                sortButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                sortButton.transform.localScale = new Vector3(1, 1, 1);
                sortButton.GetComponentInChildren<Text>().text = tag;
                x = 55;
                _y -= 30;
            }
            else
            {
                sortButton.transform.SetParent(transform);
                sortButton.transform.localPosition = new Vector3(x, _y, 0);
                sortButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                sortButton.transform.localScale = new Vector3(1, 1, 1);
                sortButton.GetComponentInChildren<Text>().text = tag;
                x += 95;
            }
            i++;
        }
        _yAfterSortButtons = _y;
    }

    public void Navigate(string nodeName)
    {
        int nodeId = 0;
        foreach (var dn in destinationNodes)
        {
            //Change to Name property later
            if (dn.Value.Tag == nodeName)
            {
                nodeId = dn.Key;
                break;
            }
        }
        graph.FindShortestPath(nodeId);
    }
}
