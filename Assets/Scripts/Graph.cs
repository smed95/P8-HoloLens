using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

public class Graph : MonoBehaviour
{
    // Used for assigning Nodes with unique IDs
    int idCounter = 0;
    int MillimeterToMeter = 1000;

    List<Edge> edges = new List<Edge>();
    // Contains ID for a node and the node reference
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    float[,] dist;
    int[][] next;

    // 
    Dictionary<int, GameObject> initializedNodes = new Dictionary<int, GameObject>();
    // Dictionary with the id of the node and the name of the node
    public Dictionary<int, Node> destinationNodes = new Dictionary<int, Node>();
    // VuMark locations and rotation in the graph
    public Dictionary<int, RTTransform> VuMarkLocations = new Dictionary<int, RTTransform>();
    public bool isNodesInitialized = false;

    // Unity bonds to the prefab instances used for instansiation
    public GameObject NodePrefab;
    public GameObject EdgePrefab;
    public GameObject SpherePrefab;

    // The data files used to create the navigation graph
    public TextAsset NodesFile;
    public TextAsset LinesFile;
    public TextAsset VuMarksFile;

    // Set when the offset in the data files has been found
    private float xOffset = 0;
    private float yOffset = 0;

    // Use this for initialization
    void Start()
    {
        FindOffset();

        InitEdges();

        InitPointsOfInterest();

        InitShortestPaths();
        
        InitVuMarks();

        //FindShortestPath(33);
    }

    void InitShortestPaths()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(int[][]));
        if (!File.Exists(Application.streamingAssetsPath + "/next.xml"))
        {
            Debug.Log("calculating all pairs shortest path");
            dist = new float[nodes.Count, nodes.Count];
            next = new int[nodes.Count][];
            for (int i = 0; i < nodes.Count; i++)
            {
                next[i] = new int[nodes.Count];
                for (int j = 0; j < nodes.Count; j++)
                {
                    dist[i, j] = Mathf.Infinity;
                    next[i][j] = -1;
                }
            }

            foreach (var e in edges)
            {
                dist[e.node1.Id, e.node2.Id] = e.dist;
                dist[e.node2.Id, e.node1.Id] = e.dist;
                next[e.node1.Id][e.node2.Id] = e.node2.Id;
                next[e.node2.Id][ e.node1.Id] = e.node1.Id;
            }

            for (int k = 0; k < nodes.Count; k++)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            var v = next[i][ k];
                            next[i][ j] = v;
                        }
                    }
                }
            }
            
            FileStream nextFile = File.Create(Application.streamingAssetsPath + "/next.xml");
            serializer.Serialize(nextFile, next);
            nextFile.Dispose();
        }
        else
        {
            Debug.Log("loading all pairs shortest path");
            Byte[] bytes = UnityEngine.Windows.File.ReadAllBytes(Application.streamingAssetsPath + "/next.xml");
            string result = Encoding.ASCII.GetString(bytes);
            next = (int[][])serializer.Deserialize(new StringReader(result));
        }
        
    }

    public void FindShortestPath(int endNodeId)
    {

        //use closest node as startNode
        int currentNodeId = FindClosestNode();

        if(next[currentNodeId][ endNodeId] == -1)
        {
            Debug.Log("Fejl");
        }
        else
        {
            while (currentNodeId != endNodeId)
            {
                nodes[currentNodeId].gameObject.SetActive(true);
                int nextId = next[currentNodeId][ endNodeId];
                edges.Find(e => e.IsMatch(currentNodeId, nextId)).gameObject.SetActive(true);
                currentNodeId = nextId;
            }
            nodes[currentNodeId].gameObject.SetActive(true);

        }
    }

    void InitEdges()
    {
        // This method could be improved by having a class containing a set of coordinates
        // and a hash value used to collect each coordinate instead of searching each time
        string[] lineSplit = LinesFile.text.Split('\n');
        for (int i = 1; i < lineSplit.Length; i++)
        {
            string[] lineValues = lineSplit[i].Split(',');
            float nodeStartX = (float.Parse(lineValues[1]) / MillimeterToMeter) - xOffset;
            float nodeStartY = (float.Parse(lineValues[2]) / MillimeterToMeter) - yOffset;
            float nodeEndX = (float.Parse(lineValues[3]) / MillimeterToMeter) - xOffset;
            float nodeEndY = (float.Parse(lineValues[4]) / MillimeterToMeter) - yOffset;
            float dist = float.Parse(lineValues[5]) / MillimeterToMeter;

            // This should be made into a method since it is just repeated 
            Node nodeFrom = FindOrInstantiateNode(nodeStartX, nodeStartY);

            Node nodeTo = FindOrInstantiateNode(nodeEndX, nodeEndY);

            GameObject edgeObject = Instantiate(EdgePrefab, transform);
            Edge edge = edgeObject.GetComponent<Edge>();
            edge.Instantiate(nodeFrom, nodeTo, dist);
            edges.Add(edge);
        }


        isNodesInitialized = true;
    }

    private Node FindOrInstantiateNode(float xCoordinate, float yCoordinate)
    {
        Node n = nodes.Values.FirstOrDefault(node => node.X == xCoordinate && node.Y == yCoordinate);
        if(n != null)
        {
            return n;
        }
        else
        {
            GameObject nodeObject = Instantiate(NodePrefab, transform);
            Node node = nodeObject.GetComponent<Node>();
            node.Instantiate(xCoordinate, yCoordinate, idCounter);
            nodes.Add(node.Id, node);
            initializedNodes.Add(node.Id, node.gameObject);
            idCounter++;
            return node;
        }
    }

    // This method searches for the home point and calculates the offset from origo
    private void FindOffset()
    {
        // The nodes data is stored in CSV format
        string[] nodeSplits = NodesFile.text.Split('\n');

        // The first line is the labels
        for (int i = 1; i < nodeSplits.Length; i++)
        {
            string[] lineValues = nodeSplits[i].Split(',');
            string pointType = lineValues[4];
            if(pointType.Contains("Home"))
            {
                xOffset = float.Parse(lineValues[1]) / MillimeterToMeter;
                yOffset = float.Parse(lineValues[2]) / MillimeterToMeter;
            }
        }
    }

    // This method instansiates the nodes with labeled information
    private void InitPointsOfInterest()
    {
        // The first line is labels
        string[] nodeSplit = NodesFile.text.Split('\n');
        for (int i = 1; i < nodeSplit.Length; i++)
        {
            // The offset is subtracted from the points
            string[] lineValues = nodeSplit[i].Split(',');
            float pointX = (float.Parse(lineValues[1]) / MillimeterToMeter) - xOffset;
            float pointY = (float.Parse(lineValues[2]) / MillimeterToMeter) - yOffset;
            string pointName = lineValues[3];
            string pointType = lineValues[4].TrimEnd('\r');

            /* If a point exsist on the coordinates of the POI then it is reachable.
             * The additional label info is then added to the point and added to the list of
             * destination nodes.
            */
            foreach (var node in nodes.Values)
            {
                if (pointX == node.X && pointY == node.Y)
                {
                    node.Name = pointName;
                    node.Type = pointType;
                    destinationNodes.Add(node.Id, node);
                }
            }
        }
    }
    

    // This method adds VuMark locations to the list of VuMarks
    private void InitVuMarks()
    {
        // The first line contain labels
        string[] vuMarkSplit = VuMarksFile.text.Split('\n');
        for (int i = 1; i < vuMarkSplit.Length; i++)
        {
            string[] vuMarkValues = vuMarkSplit[i].Split(',');
            float nodeX = (float.Parse(vuMarkValues[1]) / MillimeterToMeter) - xOffset;
            float nodeY = (float.Parse(vuMarkValues[2]) / MillimeterToMeter) - yOffset;
            float angle = float.Parse(vuMarkValues[5]);
            int vuMarkId = int.Parse(vuMarkValues[4]);

            VuMarkLocations.Add(vuMarkId,new RTTransform { x0 = nodeX, y0 = nodeY, rotation = angle});
        }
        //add the vumarks to the anchorpointsmanager
        AnchorPointsManager.InitModelPoints(VuMarkLocations);
    }

    //finds the closest node to the users current location by calculationg the distance to every node in the graph
    private int FindClosestNode()
    {
        int shortestDistanceNodeId = 0;
        float currentShortestDistance = 100000000;
        Vector3 currentPos = Camera.main.transform.position;

        foreach (var node in initializedNodes)
        {
            float dist = Vector3.Distance(node.Value.transform.position, currentPos);
            if (dist < currentShortestDistance)
            {
                currentShortestDistance = dist;
                shortestDistanceNodeId = node.Key;
            }
        }
        return shortestDistanceNodeId;
    }
}
