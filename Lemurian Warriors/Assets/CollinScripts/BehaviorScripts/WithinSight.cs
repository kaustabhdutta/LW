using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WithinSight : Conditional
{
    public float sightRadius;
    public string targetTag;
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        if (InSight(target.Value.transform, sightRadius))
        { 
            Debug.Log("triggered");
            return TaskStatus.Success;
        }
        else
            return TaskStatus.Failure;
    }
    public bool InSight(Transform targetTransform, float radius)
    {
        Debug.Log(Vector3.Distance(transform.position, targetTransform.position) < radius);
        return Vector3.Distance(transform.position, targetTransform.position) < radius;
    }
}
