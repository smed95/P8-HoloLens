using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public float X { get; private set; }
    public float Y { get; private set; }
    public int Id { get; private set; }
    public List<int> NeighborIds = new List<int>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Instantiate(float x, float y, int id)
    {
        X = x;
        Y = y;
        Id = id;
        transform.Translate(new Vector3(x, 0f, y));
    }
}
