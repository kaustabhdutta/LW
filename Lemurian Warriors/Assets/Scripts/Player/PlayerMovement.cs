using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    public NavMeshAgent mNavMeshAgent;
    public Rigidbody rigidBody;

    public Camera mainCam;
    public float walkSpeed;
    public float runSpeed;

    public float dodgeSpeed;
    

    private bool walking = false;
    private bool running = false;
    private bool dodging = false;
    //private bool canMove = true;

    private enum State
    {
        Normal,
        Dodge,
    }

    private State state;
    // added to have a seperate number available for handling the animation blend tree
    float currentSpeed;

    void Start()
    {
        anim = GetComponent<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        state = State.Normal;
    }


    void Update()
    {
        // If the player is moving, set our blend tree to either walk or run
        if (mNavMeshAgent.velocity.magnitude != 0)
        {
            // if we are running, set it to 1, if not, set it to 0.5f
            currentSpeed = ((running) ? 1f : 0.5f);
        }
        // if we are standing still, revert back to idle
        else
        {
            currentSpeed = 0f;
        }
        // Set the float paramater to our current speed
        anim.SetFloat("MoveSpeed", currentSpeed);

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {

            Walk();
            //anim.SetBool("isWalking", true);

            if (walking)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Run();

                    if (running)
                    {
                        running = true;
                        //anim.SetBool("isRunning", running);
                        anim.SetBool("isWalking", false);
                        anim.SetBool("isIdle", false);
                    }

                }
                else
                {
                    running = false;
                    //anim.SetBool("isWalking", walking);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", false);
                }

            }
            /*else
            {
                anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
            }*/
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                StartCoroutine(Dodge(hit));
            }
        }

    }

    void Walk()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            transform.LookAt(transform.position, hit.point);
            mNavMeshAgent.SetDestination(hit.point);
            mNavMeshAgent.speed = walkSpeed;
        }

        if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
        {
            walking = false;

        }
        else
        {
            walking = true;
        }
        anim.SetBool("isWalking", walking);
    }

    void Run()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            transform.LookAt(transform.position, hit.point);
            mNavMeshAgent.SetDestination(hit.point);
            mNavMeshAgent.speed = runSpeed;
        }

        if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
        {
            running = false;

        }
        else
        {
            running = true;
        }

        anim.SetBool("isRunning", running);

    }

   /* public void AvoidAttack(RaycastHit hit)
    {
        //To override the navmesh and perform dodge
        StartCoroutine(Dodge(hit));

    }*/

    IEnumerator Dodge(RaycastHit hit)
    {
        rigidBody.velocity = (hit.point - transform.position).normalized * dodgeSpeed;
        yield return new WaitForSeconds(0.5f);
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        mNavMeshAgent.destination = transform.position;

    }
}
