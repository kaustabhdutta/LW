using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierScript : MonoBehaviour
{
    public Vector3[] points;

    public string PathName;

    public void Reset()
    {
        points = new Vector3[] { new Vector3(1f, 0, 0), new Vector3(2f, 0, 0), new Vector3(3f, 0, 0), new Vector3(4f, 0, 0) };
    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(BezierStatic.GrabPoint(points[0], points[1], points[2], points[3], t));

    }

    public Vector3 GetVelocity (float t)
    {
        return transform.TransformPoint(BezierStatic.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
    }
    
    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }
}
