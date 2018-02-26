using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public float X { get; private set; }
    public float Y { get; private set; }
    public int Id { get; private set; }
    public List<int> NeighborIds = new List<int>();
    public string Tag { get; private set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Instantiate(float x, float y, int id, string tag)
    {
        X = x;
        Y = y;
        Id = id;
        Tag = tag;
        transform.Translate(new Vector3(x, 0f, y));
        GetComponentInChildren<TextMesh>().text = tag;
    }
}
