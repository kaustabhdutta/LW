 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemy : EnemyBase
{
    public static MeleeEnemy DefaultValues;
    //NavMeshAgent navAgent;
    public enum States { Idle, Chase, Attack, Lost };
    States state = States.Idle;
    public SphereCollider hitBox;
    public Transform Head;
    [Range(1,500)]
    public float sightRange;
    [Range(0, 180)]
    public float sightAngle;
    public float attackRange;
    public float TimeToIdle = 2;
    private float IdleTimer;
    
    Vector3 lastPlayerPos;
    //public DamageController damCon;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = attackRange * 0.8f;
        DefaultValues = GameObject.FindGameObjectWithTag("Base_Melee_Enemy").GetComponent<MeleeEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(Head.position, Head.position + Head.forward);
        //Debug.Log(state);
        //Debug.Log(canSeePlayer());
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetValues();
        }
        switch (state)
        {
            case States.Idle:
                if(canSeePlayer())
                {
                    state = States.Chase;
                }
                break;
            case States.Chase:
                navAgent.SetDestination(lastPlayerPos);
                IdleTimer = TimeToIdle;
                if (navAgent.remainingDistance <= attackRange * 0.8)
                {
                    state = States.Attack;
                }
                if (!canSeePlayer())
                {
                    state = States.Lost;
                }
                break;
            case States.Attack:
                if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
                {
                    state = States.Chase;
                }
                break;
            case States.Lost:
                IdleTimer -= Time.deltaTime;
                if(IdleTimer <= 0)
                {
                    state = States.Idle;
                }
                if (canSeePlayer())
                {
                    state = States.Chase;
                }
                break;
        }
    }
    public bool canSeePlayer()
    {
        //check if close enough, then linecast, then check if angle is less than sight angle
        //might remove sight angle stuff later and just make it range based. depends on designers
        bool canSee =  (player.transform.position - Head.position).magnitude <= sightRange &&
            !Physics.Linecast(Head.position, player.transform.position) &&
            VectorMath.RadiansToVector(Head.forward, (player.transform.position - Head.position)) * 180 / Mathf.PI < sightAngle;
        if (canSee)
        {
            lastPlayerPos = player.transform.position;
        }
        return canSee;
    }
    void ResetValues()
    {
        //this is an attempt but i'm not sure it will work. haven't tested in a while.
        System.Reflection.FieldInfo[] fields = DefaultValues.GetType().GetFields();
        foreach(System.Reflection.FieldInfo field in fields)
        {
            if (field.FieldType != player.GetType() && 
                field.GetValue(this) != Head)
            {
                field.SetValue(this, field.GetValue(DefaultValues));
            }
        }
        navAgent = GetComponent<NavMeshAgent>();
        fields = DefaultValues.navAgent.GetType().GetFields();
        //Debug.Log(fields);
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(navAgent, field.GetValue(DefaultValues));
        }
        enabled = true;
        Debug.Log("Reset");
    }
}
