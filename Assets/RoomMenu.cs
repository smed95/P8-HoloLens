using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour {

    public Graph graph;
    private float _y = 0f;
    // A dictionary containing all the id of the node and the gameobject of the button
    Dictionary<int, GameObject> buttonsInScene = new Dictionary<int, GameObject>();
    // Prefab of the destination button
    GameObject destinationButtonPrefab;
    // Dictionary with the nodes, which are possible destination for the user
    Dictionary<int, Node> _destinationNodes = new Dictionary<int, Node>();

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // This method filters the destination buttons based on a parsed query, which is the tags found before
    public void FilterRooms(string query)
    {
        destinationButtonPrefab = (GameObject)Resources.Load("Prefabs/DestinationButton");

        _destinationNodes = graph.destinationNodes;

        SetButtonPositions(_destinationNodes);

        //ActivateDestinations(currentPageNr, false);
        //currentPageNr = 0;
        //previousPageNr = 0;
        //ActivateDestinations(currentPageNr, true);

        Dictionary<int, Node> nodesToShow = new Dictionary<int, Node>();
        Dictionary<int, GameObject> buttonsToReposition = new Dictionary<int, GameObject>();
        if (query == "")
        {
            nodesToShow = _destinationNodes;
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
                destinationButton.transform.SetParent(transform, true);
                destinationButton.transform.localPosition = new Vector3(x, _y, 0);
                destinationButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value.Name;
                x = 55;
                _y -= 30;
            }
            else
            {
                destinationButton.transform.SetParent(transform, true);
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
        int leftX = 5;
        int rightX = 155;
        int i = 0;
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

}
