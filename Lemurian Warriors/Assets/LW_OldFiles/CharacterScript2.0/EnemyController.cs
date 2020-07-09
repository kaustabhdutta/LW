using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float inSight;
    public float facePlayer;

    Transform target;

    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        target = TargetPlayer.instance.Player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= inSight)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance) {
                //attack
                //face target

            }
        }
    }

    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime* facePlayer);


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, inSight);
    }
}
