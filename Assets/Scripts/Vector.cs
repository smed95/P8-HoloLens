using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Vector
{
    float[] data;
    public int Count
    {
        get { return data.Length; }
    }

    public Vector(float[] data)
    {
        this.data = data;
    }

    public Vector(int length)
    {
        data = new float[length];
    }

    public Vector Append(Vector other)
    {
        float[] newData = new float[this.Count + other.Count];
        for (int i = 0; i < this.Count; i++)
        {
            newData[i] = this[i];
        }
        for (int i = 0; i < other.Count; i++)
        {
            newData[i + this.Count] = other[i];
        }
        return new Vector(newData);
    }

    public float this[int key]
    {
        get
        {
            return data[key];
        }
        set
        {
            data[key] =  value;
        }
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            stringBuilder.Append(this[i]).Append("\t");
        }
        return stringBuilder.ToString();
    }
}
