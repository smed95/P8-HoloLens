﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {


    public Node node1;
    public Node node2;
    public float dist;

    public void Instantiate(Node node1, Node node2, float dist)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.dist = dist;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 startPos = node1.transform.position;
        Vector3 endPos = node2.transform.position;
        dist = Vector3.Distance(startPos, endPos);
        Vector3 scale = transform.localScale;
        //radius
        scale.y = dist / 2;
        transform.localScale = scale;
        Vector3 deltaVector = endPos - startPos;
        Vector3 middlePoint = startPos + (deltaVector * 0.5f);
        // move edge to middlepoint
        transform.position = middlePoint;
        // rotate to look at endPos
        transform.LookAt(endPos);
        // rotate down to connect points
        transform.Rotate(90f, 0f, 0f);
    }

    public bool IsMatch(int nodeId1, int nodeId2)
    {
        if ((node1.Id == nodeId1 && node2.Id == nodeId2) ||
            (node2.Id == nodeId1 && node1.Id == nodeId2))
            return true;
        else
            return false;
    }
}
