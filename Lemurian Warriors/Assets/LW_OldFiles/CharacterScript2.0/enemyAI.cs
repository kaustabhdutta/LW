using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    Transform FollowPlayer;
    public float facePlayer;
    public float speed;
    public float inSight;
    public float Stopping;
    public bool Giz=true;
    // Start is called before the first frame update
    void Start()
    {
        FollowPlayer = GameObject.FindGameObjectWithTag ("player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(FollowPlayer.position, transform.position);

        if (distance <= inSight)
        {

            //facePlayer
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(FollowPlayer.position - transform.position), facePlayer * Time.deltaTime);
            //moveToPlayer
            transform.position += transform.forward * speed * Time.deltaTime;
        }


        else

       if (distance <= 2)
        {
           
            speed = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, inSight);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Stopping);

    }
}
