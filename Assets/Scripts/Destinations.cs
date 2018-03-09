using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destinations : MonoBehaviour
{
    public Graph graph;
    Dictionary<int, string> destinationNodes = new Dictionary<int, string>();
    bool destinationNodesInitialized = false;
    public Canvas navigationMenuCanvas;
    GameObject destinationButtonPrefab;

    // Use this for initialization
    void Start()
    {
        destinationButtonPrefab = (GameObject)Resources.Load("Prefabs/DestinationButton");
    }

    // Update is called once per frame
    void Update()
    {
        if(graph.isNodesInitialized && !destinationNodesInitialized)
        {
            destinationNodes = graph.destinationNodes;

            foreach (var dn in destinationNodes)
            {
                Debug.Log(dn.Key);
            }

            Rect rect = navigationMenuCanvas.GetComponent<RectTransform>().rect;
            rect.height = CalculateCanvasHeight(destinationNodes.Count);

            SetButtonPositions();

            destinationNodesInitialized = true;
        }
    }

    float CalculateCanvasHeight(int nodeCount)
    {
        float height = (Mathf.Ceil(nodeCount / 2) * 35) + 75;
        return height;
    }

    void SetButtonPositions()
    {
        int y = 75;
        int leftX = -75;
        int rightX = 75;
        int i = 0;

        foreach (var dn in destinationNodes)
        {
            GameObject destinationButton = Instantiate(destinationButtonPrefab);
            if (i % 2 == 0)
            {
                //RectTransform rt = destinationButton.GetComponent<RectTransform>();
                //rt.position
                destinationButton.transform.parent = transform;
                destinationButton.transform.position = new Vector3(leftX, y, 0);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value;
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (i % 2 == 1)
            {
                destinationButton.transform.parent = transform;
                destinationButton.transform.position = new Vector3(rightX, y, 0);
                destinationButton.GetComponentInChildren<Text>().text = dn.Value;
                destinationButton.transform.localScale = new Vector3(1, 1, 1);
                y -= 35;
            }
            i++;
        }
    }

}
