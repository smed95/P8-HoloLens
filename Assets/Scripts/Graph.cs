using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{

    int idCounter = 0;
    int MillimeterToMeter = 1000;

    List<Edge> edges = new List<Edge>();
    Dictionary<int, Node> nodes = new Dictionary<int, Node>();


    public GameObject NodePrefab;
    public GameObject EdgePrefab;

    public TextAsset NodesFile;
    public TextAsset EdgesFile;
    public TextAsset LinesFile;

    // Use this for initialization
    void Start()
    {
        InitNodes();

        InitPointsOfInterest();

        InitNeighbours();

        //FindShortestPath(3, 27);
    }

    private void InitPointsOfInterest()
    {
        string[] nodeSplit = NodesFile.text.Split('\n');
        for (int i = 1; i < nodeSplit.Length; i++)
        {
            string[] lineValues = nodeSplit[i].Split(',');
            float pointX = float.Parse(lineValues[1]) / MillimeterToMeter;
            float pointY = float.Parse(lineValues[2]) / MillimeterToMeter;
            string pointName = lineValues[3];
            string pointType = lineValues[4];

            foreach (var node in nodes.Values)
            {
                if (pointX == node.X && pointY == node.Y)
                {
                    node.Name = pointName;
                    node.Type = pointType;
                }
            }

        }
    }

    public void FirstPointHandler()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.y = 0;
        transform.position = pos;
    }

    public void SecondPointHandler()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = 0;
        transform.LookAt(camPos);
    }

    public void FindShortestPath(int startNodeId, int endNodeId)
    {
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

    void InitNodes()
    {
        string[] lineSplit = LinesFile.text.Split('\n');
        for (int i = 1; i < lineSplit.Length; i++)
        {
            string[] lineValues = lineSplit[i].Split(',');
            float nodeStartX = float.Parse(lineValues[1]) / MillimeterToMeter;
            float nodeStartY = float.Parse(lineValues[2]) / MillimeterToMeter;
            float nodeEndX = float.Parse(lineValues[3]) / MillimeterToMeter;
            float nodeEndY = float.Parse(lineValues[4]) / MillimeterToMeter;
 
            if (!nodes.Any(x => x.Value.X == nodeStartX && x.Value.Y == nodeStartY))
            {
                GameObject nodeObject1 = Instantiate(NodePrefab, transform);
                Node node1 = nodeObject1.GetComponent<Node>();
                node1.Instantiate(nodeStartX, nodeStartY, idCounter);
                nodes.Add(node1.Id, node1);
                idCounter++;
            }
            if (!nodes.Any(x => x.Value.X == nodeEndX && x.Value.Y == nodeEndY))
            {
                GameObject nodeObject2 = Instantiate(NodePrefab, transform);
                Node node2 = nodeObject2.GetComponent<Node>();
                node2.Instantiate(nodeEndX, nodeEndY, idCounter);
                nodes.Add(node2.Id, node2);
                idCounter++;
            }

            //Node nodeFrom = nodes[nodeIdStart];
            //Node nodeTo = nodes[nodeIdEnd];
            //GameObject edgeObject = Instantiate(EdgePrefab, transform);
            //Edge edge = edgeObject.GetComponent<Edge>();
            //edge.Instantiate(nodeFrom, nodeTo, dist);
            //nodeFrom.NeighborIds.Add(nodeTo.Id);
            //nodeTo.NeighborIds.Add(nodeFrom.Id);
            //edges.Add(edge);
        }
    }

    private void InitNeighbours()
    {
        string[] lineSplit = LinesFile.text.Split('\n');
        for (int i = 1; i < lineSplit.Length; i++)
        {
            string[] lineValues = lineSplit[i].Split(',');
            float nodeStartX = float.Parse(lineValues[1]) / MillimeterToMeter;
            float nodeStartY = float.Parse(lineValues[2]) / MillimeterToMeter;
            float nodeEndX = float.Parse(lineValues[3]) / MillimeterToMeter;
            float nodeEndY = float.Parse(lineValues[4]) / MillimeterToMeter;
            float dist = float.Parse(lineValues[5]) / MillimeterToMeter;

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
}


