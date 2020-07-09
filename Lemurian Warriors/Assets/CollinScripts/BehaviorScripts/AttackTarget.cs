using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackTarget : Action
{
    public SharedTransform target;
    public float attackRange;

    private Animator anim;


    public override void OnAwake()
    {
        anim = gameObject.GetComponent<Animator>();
        
    }

    public override void OnStart()
    {
        anim.SetBool("isAttack", true);
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, target.Value.position) > attackRange)
        {
            anim.SetBool("isAttack", false);
            return TaskStatus.Success;
        }
        else
            return TaskStatus.Running;
    }
}
