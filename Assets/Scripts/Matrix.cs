using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Matrix
{
    float[,] data;
    public int RowCount
    {
        get { return data.GetLength(0); }
    }
    public int ColumnCount
    {
        get { return data.GetLength(1); }
    }

    public Matrix(int rows, int columns)
    {
        data = new float[rows, columns];
    }

    public Matrix(float[,] data)
    {
        this.data = data;
    }

    public Matrix Stack(Matrix lowerMatrix)
    {
        if (this.ColumnCount == lowerMatrix.ColumnCount)
        {
            float[,] newData = new float[this.RowCount + lowerMatrix.RowCount, this.ColumnCount];
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    newData[i, j] = this[i, j];
                }
            }
            for (int i = 0; i < lowerMatrix.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    newData[i + this.RowCount, j] = lowerMatrix[i, j];
                }
            }
            return new Matrix(newData);
        }
        else
        {
            throw new ArrayTypeMismatchException("can not stack matrices with different column count");
        }


    }

    public void SetRow(int index, float[] rowData)
    {
        for (int i = 0; i < ColumnCount; i++)
        {
            data[index, i] = rowData[i];
        }
    }

    public Matrix Transpose()
    {
        float[,] newData = new float[ColumnCount, RowCount];
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                newData[j, i] = data[i, j];
            }
        }
        return new Matrix(newData);
    }

    public Matrix Multiply(Matrix other)
    {
        if(this.ColumnCount != other.RowCount)
            throw new ArrayTypeMismatchException("can not multiply matrices when column count of first operand does not match row count of second");
        
        float[,] newData = new float[this.RowCount, other.ColumnCount];
        for (int i = 0; i < this.RowCount; i++)
        {
            for (int j = 0; j < other.ColumnCount; j++)
            {
                float res = 0;
                for (int k = 0; k < this.ColumnCount; k++)
                {
                    res += this[i, k] * other[k, j];
                }
                newData[i, j] = res;
            }
        }
        return new Matrix(newData);
    }

    public Vector Multiply(Vector other)
    {
        if (ColumnCount != other.Count)
            throw new ArrayTypeMismatchException("can not multiply matrix and vector when column count of matrix does not match count of vector");

        float[] newData = new float[RowCount];
        for (int i = 0; i < RowCount; i++)
        {
            float res = 0;
            for (int j = 0; j < ColumnCount; j++)
            {
                res += this[i, j] * other[j];
            }
            newData[i] = res;
        }
        return new Vector(newData);
    }

    public Matrix Cholesky()
    {
        if (RowCount != ColumnCount)
            throw new DataMisalignedException("Matrix must be symmetric");
        float[,] ret = new float[RowCount, ColumnCount];
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                if (j == i)
                {
                    float sum = 0;
                    for (int k = 0; k < j; k++)
                    {
                        sum += ret[j, k] * ret[j, k];
                    }
                    ret[j, j] = Mathf.Sqrt(this[j, j] - sum);
                }
                else
                {
                    float sum = 0;
                    for (int k = 0; k < j; k++)
                        sum += ret[i, k] * ret[j, k];
                    ret[i, j] = 1.0f / ret[j, j] * (this[i, j] - sum);
                }
            }
        }
        return new Matrix(ret);
    }

    public Vector ForwardSubstitution(Vector b)
    {
        float[] res = new float[b.Count];

        for (int i = 0; i < b.Count; i++)
        {
            float sum = 0;
            for (int j = 0; j < i; j++)
            {
                sum += this[i, j] * res[j];
            }
            res[i] = (b[i] - sum) / this[i, i];
        }

        return new Vector(res);
    }

    public Vector BackwardSubstitution(Vector b)
    {
        float[] res = new float[b.Count];

        for (int i = b.Count-1; i >= 0; i--)
        {
            float sum = 0;
            for (int j = b.Count-1; j > i; j--)
            {
                sum += this[i, j] * res[j];
            }
            res[i] = (b[i] - sum) / this[i, i];
        }

        return new Vector(res);
    }

    public float this[int rowIndex, int columnIndex]
    {
        get { return data[rowIndex, columnIndex]; }
        set { data[rowIndex, columnIndex] = value; }
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                stringBuilder.Append(this[i, j]).Append("\t");
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

}

