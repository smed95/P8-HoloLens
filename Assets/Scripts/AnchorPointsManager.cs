using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class AnchorPointsManager : Singleton<AnchorPointsManager> {

    public Graph graph;

    static Dictionary<int, Vector2> modelPoints = new Dictionary<int, Vector2>();
    static Dictionary<int, Vector2> actualPoints = new Dictionary<int, Vector2>();
    static Dictionary<int, int> RowIdexes = new Dictionary<int, int>();
    static Matrix A;
    static Vector b;

    protected override void Awake()
    {
        base.Awake();
    }

    public static void InitModelPoints(Dictionary<int, Vector2> points)
    {
        modelPoints = points;
    }

    public static void AddActualPoint(int id, Vector2 point)
    {
        if (modelPoints.ContainsKey(id))
        {
            if (actualPoints.ContainsKey(id))
            {
                actualPoints[id] = point;

                float[] axArray = { -modelPoints[id].y, modelPoints[id].x, 1, 0 };
                A.SetRow(RowIdexes[id], axArray);

                float[] ayArray = { modelPoints[id].x, modelPoints[id].y, 0, 1 };
                A.SetRow(RowIdexes[id]+1, ayArray);

                b[RowIdexes[id]] = actualPoints[id].x;
                b[RowIdexes[id] + 1] = actualPoints[id].y;

            }
            else
            {
                float[] axArray = { -modelPoints[id].y, modelPoints[id].x, 1, 0 };

                float[] ayArray = { modelPoints[id].x, modelPoints[id].y, 0, 1 };

                actualPoints.Add(id, point);

                if (actualPoints.Count == 1)
                {
                    A = new Matrix(2, 4);
                    b = new Vector(2);
                    RowIdexes[id] = 0;
                    A.SetRow(0, axArray);
                    A.SetRow(1, ayArray);
                    b[0] = actualPoints[id].x;
                    b[1] = actualPoints[id].y;
                }
                else
                {
                    RowIdexes[id] = A.RowCount;
                    Matrix appendMatrix = new Matrix(2, 4);
                    appendMatrix.SetRow(0, axArray);
                    appendMatrix.SetRow(1, ayArray);
                    A = A.Stack(appendMatrix);
                    Vector appendVector = new Vector(2);
                    appendVector[0] = actualPoints[id].x;
                    appendVector[1] = actualPoints[id].y;
                    b = b.Append(appendVector);
                }
                
            }

            if (actualPoints.Count > 1)
                CalcTransform();
            
        }
        else
        {
            Debug.LogError("No point with id: " + id);
        }
    }

    public static int GetActualPointCount()
    {
        return actualPoints.Count;
    }

    public static void CalcTransform()
    {
        Matrix ATrans = A.Transpose();

        Matrix ATransA = ATrans.Multiply(A);
        Matrix CholeskyLower = ATransA.Cholesky();
        Matrix CholeskyUpper = CholeskyLower.Transpose();

        Vector z = CholeskyLower.ForwardSubstitution(ATrans.Multiply(b));
        Vector c = CholeskyUpper.BackwardSubstitution(z);
        var sin = c[0];
        var cos = c[1];
        var angle = Mathf.Atan(sin / cos);
        var res = new RTTransform();
        res.rotation = Mathf.Rad2Deg * angle;
        res.x0 = c[2];
        res.y0 = c[3];
        Debug.Log(res.rotation + ", " + res.x0 + ", " + res.y0);
        Instance.graph.AdjustGraph(res);
    }
	
}

public class RTTransform
{
    public float rotation = 0;
    public float x0 = 0;
    public float y0 = 0;


}
