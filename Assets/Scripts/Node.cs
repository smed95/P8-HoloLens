using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public float X { get; private set; }
    public float Y { get; private set; }
    public int Id { get; private set; }
    public List<int> NeighborIds = new List<int>();
    public string Type { get; set; }
    public string Name
    {
        get { return name; }
        set
        {
            this.name = value;
            GetComponentInChildren<TextMesh>().text = name;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Instantiate(float x, float y, int id, string type = "", string name = "")
    {
        X = x;
        Y = y;
        Id = id;
        Type = type;
        Name = name;
        transform.Translate(new Vector3(x, 0f, y));
        GetComponentInChildren<TextMesh>().text = name;
    }
}
