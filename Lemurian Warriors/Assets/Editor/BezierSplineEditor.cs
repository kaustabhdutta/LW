using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;
    private const float dScale = 0.5f;
    private const int stepsPerCurve = 10;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;
    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ViewPoint(0);
        for (int i = 1; i < spline.points.Length; i += 3)
        {
            Vector3 p1 = ViewPoint(1);
            Vector3 p2 = ViewPoint(2);
            Vector3 p3 = ViewPoint(3);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            
            Handles.DrawBezier(p0, p3, p1, p2, Color.red, null, 2f);
            p0 = p3;
        }
        ShowDirections();
    }

    private Vector3 ViewPoint (int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.points[index]);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.white;
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.points[index] = handleTransform.InverseTransformPoint(point);
            }
        }
        return point;
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * dScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 0; i < steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)lineSteps) * dScale);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        spline = target as BezierSpline;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }
}
