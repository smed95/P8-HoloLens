using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class AnchorPointsManager : Singleton<AnchorPointsManager>
{

    //reference to the graph
    public Graph graph;

    //Dictionary with the local positions and rotations of the vumarks in the graph
    static Dictionary<int, RTTransform> modelPoints = new Dictionary<int, RTTransform>();

    public static void InitModelPoints(Dictionary<int, RTTransform> points)
    {
        modelPoints = points;
    }

    //function called when a vumark is detected
    public static void AddActualPoint(int id, Transform vumarkTransform)
    {
        //if the vumark id is in the graph model
        if (modelPoints.ContainsKey(id))
        {
            //**rotation part**
            //angle of the normalvector of the vumark in the local X/Z plane of the graph
            var modelAngle = modelPoints[id].rotation;
            //the global vector along the local x axis of the graph
            var graphForward = Instance.graph.transform.forward;
            //keep the vector in the X/Z plane
            graphForward.y = 0;
            //rotate the global forward vector to the angle of the model to get the normalvector of the vumark in global space 
            var modelNormalVector = Quaternion.Euler(0, modelAngle, 0) * graphForward;
            //the normalvector of the detected vumark
            var vumarkNormalVector = vumarkTransform.up;
            //keep the vector in the X/Z plane
            vumarkNormalVector.y = 0;
            //rotate the graph such that the angle of the model vumark and detected vumark's normalvectors match
            Instance.graph.transform.rotation *= Quaternion.FromToRotation(modelNormalVector, vumarkNormalVector);

            //**position part**
            //position of detected vumark
            var vumarkPosition = vumarkTransform.position;
            //ignore y direction (up/down)
            vumarkPosition.y = 0;
            //calculate global position of model vumark using the graph's transform
            var modelPoint3d = Instance.graph.transform.TransformPoint(new Vector3(modelPoints[id].x0, 0, modelPoints[id].y0));
            //ignore y direction (up/down)
            modelPoint3d.y = 0;
            //difference in position
            var diff = vumarkPosition - modelPoint3d;
            Debug.Log(diff);
            //calculate difference in hight to fit the graph 1.5 meters below eye level
            var heightDiff = (Camera.main.transform.position.y - 1.5f) - Instance.graph.transform.position.y;
            //move the graph to the correct position
            Instance.graph.transform.position += new Vector3(diff.x, heightDiff, diff.z);
        }
        else
        {
            Debug.LogError("No point with id: " + id);
        }
    }
}

//small class to save position and rotation
public class RTTransform
{
    public float rotation = 0;
    public float x0 = 0;
    public float y0 = 0;
}
