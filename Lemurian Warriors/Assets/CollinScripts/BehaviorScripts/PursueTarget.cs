using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PursueTarget : Action
{
    public float speed;
    public SharedTransform target;
    public float stayInSightRadius;

    private NavMeshAgent agent;
    private Animator anim;

    public override void OnAwake()
    {
        anim = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    public override TaskStatus OnUpdate()
    {

        agent.SetDestination(target.Value.position);

        if (agent.velocity.magnitude >= 0.1f)
            anim.SetBool("isRun", true);
        else
            anim.SetBool("isRun", false);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetBool("isRun", false);
            Debug.Log("Done!");
            return TaskStatus.Success;
        }
        else if (agent.remainingDistance > stayInSightRadius)
        {
            agent.SetDestination(gameObject.transform.position);
            anim.SetBool("isRun", false);
            return TaskStatus.Failure;
        }

        transform.LookAt(transform.position, target.Value.position);
        return TaskStatus.Running;
        
    }
}
