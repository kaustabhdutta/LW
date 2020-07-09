using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DistanceCloser : IComparer
{
    private Transform compareTransform;

    public DistanceCloser(Transform compTransform) {
        compareTransform = compTransform;
    }

    public int Compare(object x, object y) {
        Collider xCollider = x as Collider;
        Collider yCollider = y as Collider;

        Vector3 offset = xCollider.transform.position - compareTransform.position;
        float yDistance = offset.sqrMagnitude;
        float xDistance = offset.sqrMagnitude;

        return xDistance.CompareTo(yDistance);
    }
}
