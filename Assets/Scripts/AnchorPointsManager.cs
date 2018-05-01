using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class AnchorPointsManager : Singleton<AnchorPointsManager> {

    public Graph graph;

    static Dictionary<int, RTTransform> modelPoints = new Dictionary<int, RTTransform>();
    static Vector3 CenterPoint;
    static Vector3 DirectionPoint;
    //static Dictionary<int, Vector2> actualPoints = new Dictionary<int, Vector2>();
    //static Dictionary<int, int> RowIdexes = new Dictionary<int, int>();
    //static Matrix A;
    //static Vector b;

    protected override void Awake()
    {
        base.Awake();
    }

    public static void InitModelPoints(Dictionary<int, RTTransform> points)
    {
        modelPoints = points;
    }

    public static void AddActualPoint(int id, Transform vumarkTransform)
    {
        if (modelPoints.ContainsKey(id))
        {
            
            var modelAngle = modelPoints[id].rotation;
            var graphForward = Instance.graph.transform.forward;
            graphForward.y = 0;
            var modelNormalVector = Quaternion.Euler(0, modelAngle, 0) * graphForward;
            var vumarkNormalVector = vumarkTransform.up;
            vumarkNormalVector.y = 0;
            Instance.graph.transform.rotation *= Quaternion.FromToRotation(modelNormalVector, vumarkNormalVector);

            var vumarkPosition = vumarkTransform.position;
            vumarkPosition.y = 0;
            var modelPoint3d = Instance.graph.transform.TransformPoint(new Vector3(modelPoints[id].x0, 0, modelPoints[id].y0));
            modelPoint3d.y = 0;
            var diff = vumarkPosition - modelPoint3d;
            Debug.Log(diff);
            var heightDiff = (Camera.main.transform.position.y - 1.5f) - Instance.graph.transform.position.y;
            Instance.graph.transform.position += new Vector3(diff.x, heightDiff, diff.z);

            //switch (id)
            //{
            //    case 4:

            //        Instance.graph.transform.position = new Vector3(vumarkTransform.x, vumarkTransform.y, vumarkTransform.z);
            //        vumarkTransform.y = 0;
            //        CenterPoint = vumarkTransform;
            //        if (DirectionPoint != null)
            //        {
            //            CalcAndSetRotation();
            //        }
            //        break;
            //    case 5:
            //        vumarkTransform.y = 0;
            //        DirectionPoint = vumarkTransform;
            //        if(CenterPoint != null)
            //        {
            //            CalcAndSetRotation();
            //        }
            //        break;
            //    default:
            //        var modelPoint = modelPoints[id];
            //        var modelPoint3d = Instance.graph.transform.TransformPoint(new Vector3(modelPoint.x, 0, modelPoint.y));
            //        modelPoint3d.y = 0;
            //        vumarkTransform.y = 0;
            //        var diff = vumarkTransform - modelPoint3d;
            //        Debug.Log(diff);
            //        Instance.graph.transform.position += new Vector3(diff.x, 0, diff.z);
            //        break;
            //}
            //if (actualPoints.ContainsKey(id))
            //{
            //    actualPoints[id] = point;

            //    float[] axArray = { -modelPoints[id].y, modelPoints[id].x, 1, 0 };
            //    A.SetRow(RowIdexes[id], axArray);

            //    float[] ayArray = { modelPoints[id].x, modelPoints[id].y, 0, 1 };
            //    A.SetRow(RowIdexes[id]+1, ayArray);

            //    b[RowIdexes[id]] = actualPoints[id].x;
            //    b[RowIdexes[id] + 1] = actualPoints[id].y;

            //}
            //else
            //{
            //    float[] axArray = { -modelPoints[id].y, modelPoints[id].x, 1, 0 };

            //    float[] ayArray = { modelPoints[id].x, modelPoints[id].y, 0, 1 };

            //    actualPoints.Add(id, point);

            //    if (actualPoints.Count == 1)
            //    {
            //        A = new Matrix(2, 4);
            //        b = new Vector(2);
            //        RowIdexes[id] = 0;
            //        A.SetRow(0, axArray);
            //        A.SetRow(1, ayArray);
            //        b[0] = actualPoints[id].x;
            //        b[1] = actualPoints[id].y;
            //    }
            //    else
            //    {
            //        RowIdexes[id] = A.RowCount;
            //        Matrix appendMatrix = new Matrix(2, 4);
            //        appendMatrix.SetRow(0, axArray);
            //        appendMatrix.SetRow(1, ayArray);
            //        A = A.Stack(appendMatrix);
            //        Vector appendVector = new Vector(2);
            //        appendVector[0] = actualPoints[id].x;
            //        appendVector[1] = actualPoints[id].y;
            //        b = b.Append(appendVector);
            //    }

            //}

            //if (actualPoints.Count > 1)
            //    CalcTransform();

        }
        else
        {
            Debug.LogError("No point with id: " + id);
        }
    }

    //static void CalcAndSetRotation()
    //{
    //    var modelPoint = modelPoints[5];
    //    var modelPoint3d = Instance.graph.transform.TransformPoint(new Vector3(modelPoint.x, 0, modelPoint.y));
    //    modelPoint3d.y = 0;
    //    Vector3 v1 = modelPoint3d - CenterPoint;
    //    Vector3 v2 = DirectionPoint - CenterPoint;
    //    Instance.graph.transform.rotation *= Quaternion.FromToRotation(v1, v2);
    //}

    //public static int GetActualPointCount()
    //{
    //    return actualPoints.Count;
    //}

    //public static void CalcTransform()
    //{
    //    Matrix ATrans = A.Transpose();

    //    Matrix ATransA = ATrans.Multiply(A);
    //    Matrix CholeskyLower = ATransA.Cholesky();
    //    Matrix CholeskyUpper = CholeskyLower.Transpose();

    //    Vector z = CholeskyLower.ForwardSubstitution(ATrans.Multiply(b));
    //    Vector c = CholeskyUpper.BackwardSubstitution(z);
    //    var sin = c[0];
    //    var cos = c[1];
    //    var angle = Mathf.Atan2(sin, cos);
    //    var res = new RTTransform
    //    {
    //        rotation = Mathf.Rad2Deg * angle,
    //        x0 = c[2],
    //        y0 = c[3]
    //    };
    //    Debug.Log(res.rotation + ", " + res.x0 + ", " + res.y0);
    //    Instance.graph.AdjustGraph(res);
    //}
	
}

public class RTTransform
{
    public float rotation = 0;
    public float x0 = 0;
    public float y0 = 0;
}
