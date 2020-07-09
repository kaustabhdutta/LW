using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FootageEnemy : EnemyBase
{
    [SerializeField]
    private Animator anim;
    private enum States { Idle, Melee, Hurt, Dead};
    private States state = States.Idle;
    [SerializeField]
    private float attackRange = 1;
    float walkSpeed = 3.5f;
    float sprintSpeed = 3.5f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        if(player == null)
        {
            player = FindObjectOfType<NewPlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(damCon.currentHealth <= 0)
        {
            state = States.Dead;
        }
        if (canAct)
        {
            switch (state)
            {
                case States.Idle:
                    navAgent.destination = player.transform.position;
                    if ((player.transform.position - transform.position).magnitude <= attackRange)
                    {
                        state = States.Melee;
                    }
                    break;
                case States.Melee:
                    CanActFalse();
                    anim.Play("Melee", -1, 0f);
                    break;
                case States.Hurt:

                    break;
                case States.Dead:

                    break;
            }
        }
        Debug.Log(state);
        anim.SetInteger("State", (int)state);
        anim.SetFloat("MovementCos", VectorMath.DistanceInDirection(navAgent.velocity.normalized, transform.forward) * navAgent.velocity.magnitude / sprintSpeed);
        anim.SetFloat("MovementSin", VectorMath.DistanceInDirection(navAgent.velocity.normalized, transform.right) * navAgent.velocity.magnitude / sprintSpeed);
    }
    protected override void TakeDamage(Hitbox hit)
    {
        if (state != States.Dead)
        {
            state = States.Hurt;
            anim.Play("Hurt", -1, 0f);
            base.TakeDamage(hit);
        }
    }
    new public void CanActTrue()
    {
        canAct = true;
        navAgent.speed = walkSpeed;
    }
    new public void CanActFalse()
    {
        canAct = false;
        navAgent.speed = 0;
        navAgent.velocity = Vector3.zero;
        navAgent.destination = transform.position;
    }
    public void BackToIdle()
    {
        state = States.Idle;
        CanActTrue();
    }
}
