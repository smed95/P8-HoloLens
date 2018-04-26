﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // 
    Dictionary<int, GameObject> initializedNodes = new Dictionary<int, GameObject>();
    // Dictionary with the id of the node and the name of the node
    public Dictionary<int, Node> destinationNodes = new Dictionary<int, Node>();
    public Dictionary<int, Vector2> VuMarkLocations = new Dictionary<int, Vector2>();
    public bool isNodesInitialized = false;
    
    GameObject object1;
    GameObject object2;

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

        InitNeighbours();

        InitAnchorPoints();
        InitVuMarks();
        //Vector2 point1 = new Vector2(-2.5f, 1.2f);
        //Vector2 point2 = new Vector2(0.7f, 0.4f);
        //AnchorPointsManager.AddActualPoint(4, point1);
        //AnchorPointsManager.AddActualPoint(5, point2);
        ////transform.SetPositionAndRotation(new Vector3(-2.5f, 0, 1.2f), Quaternion.Euler(0, -165.5f, 0));
        //Debug.Log(object1.transform.position);
        //Debug.Log(object2.transform.position);
        //FindShortestPath(62, 32);
    }

    public void FindShortestPath(/*int startNodeId,*/ int endNodeId)
    {
        int startNodeId = FindClosestNode();
        List<int> closedSet = new List<int>();
        List<int> openSet = new List<int>();
        openSet.Add(startNodeId);
        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        Dictionary<int, float> gScore = new Dictionary<int, float>();
        Dictionary<int, float> fScore = new Dictionary<int, float>();
        foreach (int nodeId in nodes.Keys)
        {
            gScore.Add(nodeId, Mathf.Infinity);
            fScore.Add(nodeId, Mathf.Infinity);
        }
        gScore[startNodeId] = 0;
        fScore[startNodeId] = Vector3.Distance(nodes[startNodeId].transform.position, nodes[endNodeId].transform.position);

        while (openSet.Count != 0)
        {
            float minFScore = Mathf.Infinity;
            int currentId = -1;
            foreach (int id in openSet)
            {
                if (fScore[id] < minFScore)
                {
                    minFScore = fScore[id];
                    currentId = id;
                }
            }

            if (currentId == endNodeId)
            {
                while (cameFrom.Keys.Contains(currentId))
                {
                    nodes[currentId].gameObject.SetActive(true);
                    int cameFromId = cameFrom[currentId];
                    edges.Find(e => e.isMatch(currentId, cameFromId)).gameObject.SetActive(true);
                    currentId = cameFromId;
                }
                nodes[currentId].gameObject.SetActive(true);
                return;
            }

            openSet.Remove(currentId);
            closedSet.Add(currentId);
            foreach (int neighborId in nodes[currentId].NeighborIds)
            {
                if (closedSet.Contains(neighborId))
                    continue;

                if (!openSet.Contains(neighborId))
                    openSet.Add(neighborId);

                float newGScore = gScore[currentId] + Vector3.Distance(nodes[currentId].transform.position, nodes[neighborId].transform.position);
                if (newGScore >= gScore[neighborId])
                    continue;

                cameFrom[neighborId] = currentId;
                gScore[neighborId] = newGScore;
                fScore[neighborId] = newGScore + Vector3.Distance(nodes[neighborId].transform.position, nodes[endNodeId].transform.position);
            }
        }

        Debug.Log("FEJL");
    }

    void InitAnchorPoints()
    {
        
        Vector3 astronautpos = object1.transform.position;
        VuMarkLocations.Add(4, new Vector2(astronautpos.x, astronautpos.z));
        Vector3 dronepos = object2.transform.position;
        VuMarkLocations.Add(5, new Vector2(dronepos.x, dronepos.z));
    }

    public void AdjustGraph(RTTransform rtt)
    {
        transform.SetPositionAndRotation(new Vector3(rtt.x0, transform.position.y, rtt.y0), Quaternion.Euler(0, -rtt.rotation, 0));
    }

    void InitEdges()
    {
        string[] lineSplit = LinesFile.text.Split('\n');
        for (int i = 1; i < lineSplit.Length; i++)
        {
            string[] lineValues = lineSplit[i].Split(',');
            float nodeStartX = (float.Parse(lineValues[1]) / MillimeterToMeter) - xOffset;
            float nodeStartY = (float.Parse(lineValues[2]) / MillimeterToMeter) - yOffset;
            float nodeEndX = (float.Parse(lineValues[3]) / MillimeterToMeter) - xOffset;
            float nodeEndY = (float.Parse(lineValues[4]) / MillimeterToMeter) - yOffset;
 
            if (!nodes.Any(x => x.Value.X == nodeStartX && x.Value.Y == nodeStartY))
            {
                GameObject nodeObject1 = Instantiate(NodePrefab, transform);
                Node node1 = nodeObject1.GetComponent<Node>();
                node1.Instantiate(nodeStartX, nodeStartY, idCounter);
                nodes.Add(node1.Id, node1);
                initializedNodes.Add(node1.Id, node1.gameObject);
                idCounter++;
            }
            if (!nodes.Any(x => x.Value.X == nodeEndX && x.Value.Y == nodeEndY))
            {
                GameObject nodeObject2 = Instantiate(NodePrefab, transform);
                Node node2 = nodeObject2.GetComponent<Node>();
                node2.Instantiate(nodeEndX, nodeEndY, idCounter);
                nodes.Add(node2.Id, node2);
                initializedNodes.Add(node2.Id, node2.gameObject);
                idCounter++;
            }
        }

        //anchor points
        object1 = Instantiate(SpherePrefab, transform);
        object2 = Instantiate(SpherePrefab, transform);
        float X = (60559f / MillimeterToMeter) - xOffset;
        float Y = (47889f / MillimeterToMeter) - yOffset;
        Vector3 dronePoint = new Vector3(X - 0.9f, 1.55f, Y);
        object2.transform.Translate(dronePoint);

        isNodesInitialized = true;
    }

    // This method searches for the two home points and calculate the offset from origo
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

    /* This method creates the edges between nodes if there exsists two nodes 
     * according to the lines file.
    */ 
    private void InitNeighbours()
    {
        // The first line is labels
        string[] lineSplit = LinesFile.text.Split('\n');
        for (int i = 1; i < lineSplit.Length; i++)
        {
            // Each line contains two set of coordinates corresponding to each end point
            string[] lineValues = lineSplit[i].Split(',');
            float nodeStartX = (float.Parse(lineValues[1]) / MillimeterToMeter) - xOffset;
            float nodeStartY = (float.Parse(lineValues[2]) / MillimeterToMeter) - yOffset;
            float nodeEndX = (float.Parse(lineValues[3]) / MillimeterToMeter) - xOffset;
            float nodeEndY = (float.Parse(lineValues[4]) / MillimeterToMeter) - yOffset;
            float dist = float.Parse(lineValues[5]) / MillimeterToMeter;

            /* The nodes list is searched for each of the two points
             * If they both exsists then an edge is instansiated and added to the neighbor list
             */
            foreach (var nodeStart in nodes.Values)
            {
                if (nodeStart.X == nodeStartX && nodeStart.Y == nodeStartY)
                {
                    foreach (var nodeEnd in nodes.Values)
                    {
                        if (nodeEnd.X == nodeEndX && nodeEnd.Y == nodeEndY && nodeStart != nodeEnd)
                        {
                            Node nodeFrom = nodes[nodeStart.Id];
                            Node nodeTo = nodes[nodeEnd.Id];
                            GameObject edgeObject = Instantiate(EdgePrefab, transform);
                            Edge edge = edgeObject.GetComponent<Edge>();
                            edge.Instantiate(nodeFrom, nodeTo, dist);
                            nodeFrom.NeighborIds.Add(nodeTo.Id);
                            nodeTo.NeighborIds.Add(nodeFrom.Id);
                            edges.Add(edge);
                        }
                    }
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
            int vuMarkId = int.Parse(vuMarkValues[4]);

            VuMarkLocations.Add(vuMarkId,new Vector2(nodeX, nodeY));
        }
        AnchorPointsManager.InitModelPoints(VuMarkLocations);
    }

    //void InitNodes()
    //{
    //    string[] nodeLines = NodesFile.text.Split('\n');
    //    for (int i = 1; i < nodeLines.Length; i++)
    //    {
    //        if (nodeLines[i].Length < 5)
    //            continue;
    //        string[] lineValues = nodeLines[i].Split(';');
    //        int id = int.Parse(lineValues[0]);
    //        string tag = lineValues[1];
    //        float x = float.Parse(lineValues[2]) / MillimeterToMeter;
    //        float y = float.Parse(lineValues[3]) / MillimeterToMeter;
    //        GameObject nodeObject = Instantiate(NodePrefab, transform);
    //        Node node = nodeObject.GetComponent<Node>();
    //        node.Instantiate(x, y, id, tag);
    //        nodes.Add(node.Id, node);
    //    }
    //}

//    void InitEdges()
//    {
//        string[] edgeLines = EdgesFile.text.Split('\n');
//        for (int i = 1; i < edgeLines.Length; i++)
//        {
//            if (edgeLines[i].Length < 5)
//                continue;

//            int idFrom = int.Parse(edgeLines[i].Split(';')[1]);
//            Node nodeFrom = nodes[idFrom];
//            int idTo = int.Parse(edgeLines[i].Split(';')[2]);
//            Node nodeTo = nodes[idTo];
//            GameObject edgeObject1 = Instantiate(EdgePrefab, transform);
//            Edge edge = edgeObject1.GetComponent<Edge>();
//            edge.Instantiate(nodeFrom, nodeTo, 0);
//            nodeFrom.NeighborIds.Add(nodeTo.Id);
//            nodeTo.NeighborIds.Add(nodeFrom.Id);
//            edges.Add(edge);
//        }
//    }

    private int FindClosestNode()
    {
        int shortestDistanceNodeId = 0;
        float currentShortestDistance = 100000000;
        Vector3 currentPos = Camera.main.transform.position;

        foreach (var node in initializedNodes)
        {
            Vector3 nodePos = new Vector3(node.Value.transform.position.x, node.Value.transform.position.y, node.Value.transform.position.z);
            float dist = Vector3.Distance(nodePos, currentPos);
            if (dist < currentShortestDistance)
            {
                currentShortestDistance = dist;
                shortestDistanceNodeId = node.Key;
            }
        }
        return shortestDistanceNodeId;
    }

}


