using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : MonoBehaviour
{
    
   // public Transform enemy;
    public float sight;

    Animator anim;

    Transform target;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        target = Manager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //float distance = Vector3.Distance(target.position, transform.position);
      //  if (Vector3.Distance(target.position, this.transform.position) < sight)
       // {

       //     Vector3 direction = target.position - this.transform.position;
       //     direction.y = 0;

        //    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), .1f);

       //     if (direction.magnitude > stop)
        //    {
               // this.transform.Translate(0, 0, speed*Time.deltaTime);
        //    }
        //  }
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= sight)
        {
            agent.SetDestination(target.position);
            anim.SetBool("isRun", true);
          
          

            if (distance <= agent.stoppingDistance)
            {
                anim.SetBool("isRun", false);
                anim.SetBool("isAttack", true);
                faceTarget();
            }
            
            else anim.SetBool("isAttack", false);
        }
       else anim.SetBool("isRun", false);
    }
    void faceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRoatation, Time.deltaTime * 5f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sight);
    }
   
}
