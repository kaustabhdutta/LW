using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Hurtbox))]
[RequireComponent(typeof(DamageController))]
public class RangedEnemy : EnemyBase
{
    public static RangedEnemy DefaultValues;
    //public NavMeshAgent navAgent;
    public float sprintSpeed;
    public float walkSpeed;
    public enum animStates { IdleMove, Shoot, ChargeShot, Melee, Hurt, Death};
    public enum States { Idle, Chase, Flee, Shoot, ChargeShot, Melee, Dead  }
    States behaviorState = States.Idle;
    animStates animState = animStates.IdleMove;
    [Tooltip("Red")]
    public float alertRange;
    [Tooltip("Magenta")]
    public float loseSightRange;
    [Tooltip("Does nothing. use chargeShotRange instead")]
    public float chargeShotRange;
    [Tooltip("Blue")]
    public float shootRange;
    [Tooltip("Yellow, < shootRange")]
    public float runRange;
    [Tooltip("Green, < runRange")]
    public float meleeRange;
    Vector3 lastPlayerPos;
    public NewSpell basicProjectile;
    public NewSpell chargeShot;
    public Transform firingHand;
    [System.NonSerialized]
    public bool canMove = true;
    [System.NonSerialized]
    public bool canChangeStates = true;
    public Animator anim;
    public Hurtbox hurtbox;
    //public DamageController damCon;
    protected override void Start()
    {
        canMove = true;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        anim.SetInteger("State", 0);
        behaviorState = States.Idle;
        animState = animStates.IdleMove;
        damCon = GetComponent<DamageController>();
        base.Start();
    }
    void Update()
    {
        /*
         * Calculate distance to player
         * if outside of lose sight range, return to idle
         * if it can move, it has some states.
         */
        Debug.Log(1 / Time.fixedDeltaTime);
        float DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        basicProjectile.DoCooldown(Time.deltaTime);
        if(damCon.currentHealth <= 0)
        {
            //die
            animState = animStates.Death;
            navAgent.destination = transform.position;
            navAgent.speed = 0;
            navAgent.velocity = Vector3.zero;
            canChangeStates = false;
            canAct = false;
            anim.SetInteger("State", (int)animState);
        }
        if (canChangeStates)
        {
            if (DistanceToPlayer >= loseSightRange && animState != animStates.Death)
            {
                //if lost sight and is not dead, go to idle
                behaviorState = States.Idle;
            }
            //using finite state machine cuz it's all we really need.
            switch (behaviorState)
            {
                case States.Idle:
                    //all transitions out of idle.
                    navAgent.speed = walkSpeed;
                    animState = animStates.IdleMove;
                    if (DistanceToPlayer <= alertRange)
                    {
                        if (DistanceToPlayer <= meleeRange)
                        {
                            behaviorState = States.Melee;
                        }
                        else if (DistanceToPlayer <= runRange)
                        {
                            behaviorState = States.Flee;
                        }
                        else if(DistanceToPlayer <= shootRange)
                        {
                            behaviorState = States.Shoot;
                        }
                        else if(DistanceToPlayer <= chargeShotRange)
                        {
                            behaviorState = States.ChargeShot;
                        }
                        else
                        {
                            behaviorState = States.Flee;
                        }
                    }
                    break;
                case States.Chase:
                    //check if it needs to transition, otherwise move to player location.
                    navAgent.speed = walkSpeed;
                    if (DistanceToPlayer < chargeShotRange)
                    {
                        behaviorState = States.ChargeShot;
                        animState = animStates.ChargeShot;
                    }
                    else
                    {
                        navAgent.destination = player.transform.position;
                    }
                    break;
                case States.Flee:
                    //run directly away from player
                    animState = animStates.IdleMove;
                    Vector3 dest = transform.position + (transform.position - player.transform.position).normalized * (shootRange - (transform.position - player.transform.position).magnitude);
                    navAgent.destination = dest - VectorMath.DistanceInDirection(dest, Vector3.up) * Vector3.up;
                    navAgent.speed = sprintSpeed;
                    animState = animStates.IdleMove;
                    //transitions out of fleeing
                    if(DistanceToPlayer >= (runRange + shootRange) / 2)
                    {
                        animState = animStates.Shoot;
                        behaviorState = States.Shoot;
                    }
                    if(DistanceToPlayer > shootRange)
                    {
                        behaviorState = States.ChargeShot;
                        animState = animStates.ChargeShot;
                    }
                    if(DistanceToPlayer > chargeShotRange)
                    {
                        behaviorState = States.Chase;
                        animState = animStates.IdleMove;
                    }
                    break;
                case States.Shoot:
                    animState = animStates.Shoot;
                    //transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(Player.transform.position - transform.position), Vector3.up);
                    //if player gets too close
                    if (DistanceToPlayer <= runRange)
                    {
                        if (!player.canAct)
                        {
                            //run if the player can't move
                            behaviorState = States.Flee;
                            animState = animStates.IdleMove;
                        }
                        else if(DistanceToPlayer <= meleeRange)
                        {
                            //smack the player if they can move to put them in hitstun and make it so they can't
                            animState = animStates.Melee;
                            behaviorState = States.Melee;
                        }
                    }
                    else if (DistanceToPlayer > shootRange)
                    {
                        behaviorState = States.ChargeShot;
                    }
                    break;
                case States.ChargeShot:
                    animState = animStates.ChargeShot;
                    //transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(Player.transform.position - transform.position), Vector3.up);
                    if (DistanceToPlayer > chargeShotRange)
                    {
                        behaviorState = States.Chase;
                    }
                    else if (DistanceToPlayer <= runRange)
                    {
                        behaviorState = States.Flee;
                    }
                    else if(DistanceToPlayer < shootRange)
                    {
                        behaviorState = States.Shoot;
                    }
                    else
                    {
                        //StartCoroutine(ChargeShot(Player.transform.position));
                    }
                    break;
                case States.Melee:
                    behaviorState = States.Flee;
                    /*if (!Player.canAct || DistanceToPlayer > meleeRange)
                    {
                        behaviorState = States.Flee;
                    }
                    else
                    {
                        DoAtk(hitboxes.frameData[0]);
                    }*/
                    break;
            }
            if(animState == animStates.IdleMove)
            {
                transform.forward.Normalize();
                transform.right.Normalize();
                anim.SetFloat("Speed", navAgent.velocity.magnitude);
                anim.SetFloat("MovementCos", VectorMath.DistanceInDirection(navAgent.velocity.normalized, transform.forward) * navAgent.velocity.magnitude / sprintSpeed);
                anim.SetFloat("MovementSin", VectorMath.DistanceInDirection(navAgent.velocity.normalized, transform.right) * navAgent.velocity.magnitude / sprintSpeed);
            }
            anim.SetInteger("State", (int)animState);
            //Debug.Log(behaviorState);
            
        }
        if (canMove)
        {
            switch (behaviorState)
            {
                case States.Idle:
                    navAgent.speed = walkSpeed;
                    if(navAgent.velocity.magnitude != 0)
                    {
                        transform.rotation = Quaternion.LookRotation(navAgent.velocity);
                    }
                    animState = animStates.IdleMove;
                    break;
                case States.Chase:
                    navAgent.destination = player.transform.position;
                    break;
                case States.Flee:
                    Vector3 dest = transform.position + (transform.position - player.transform.position).normalized * (shootRange - (transform.position - player.transform.position).magnitude);
                    navAgent.destination = dest - VectorMath.DistanceInDirection(dest, Vector3.up) * Vector3.up;
                    navAgent.speed = sprintSpeed;
                    animState = animStates.IdleMove; 
                    break;
                case States.Shoot:
                    animState = animStates.Shoot;
                    transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(player.transform.position - transform.position), Vector3.up);
                    //Debug.Log("Look At Player");
                    break;
                case States.ChargeShot:
                    animState = animStates.ChargeShot;
                    transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(player.transform.position - transform.position), Vector3.up);
                    break;
                case States.Melee:
                    animState = animStates.Melee;
                    break;
            }
            anim.SetInteger("State", (int)animState);
        }
    }
    protected override void TakeDamage(Hitbox hit)
    {
        animState = animStates.Hurt;
        anim.SetInteger("State", (int)animState);
        base.TakeDamage(hit);
    }
    void StartFlinch()
    {
        navAgent.speed = 0;
        canMove = false;
        canAct = false;
        canChangeStates = false;
    }
    void EndFlinch()
    {
        navAgent.speed = walkSpeed;
        canMove = true;
        canAct = true;
        canChangeStates = true;
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
    //i was using coroutines but then switched to animation events because it's easier. keeping this just in case.
    /*
    public IEnumerator Shoot(Vector3 Target)
    {
        animState = animStates.Shoot;
        Debug.Log("Pew Pew");
        canChangeStates = false;
        navAgent.destination = transform.position;
        navAgent.speed = 0;
        yield return GeneralUtil.WaitForFrames(basicProjectile.castDelay);
        canMove = false;
        basicProjectile.CastSpell(firingHand == null ? transform.position + VectorMath.LocalToWorld(basicProjectile.VFXSpawnPos, transform) : firingHand.transform.position, Target);
        yield return GeneralUtil.WaitForFrames(basicProjectile.endLag);
        canMove = true;
        navAgent.speed = walkSpeed;
        canChangeStates = true;

    }
    public IEnumerator ChargeShot(Vector3 Target)
    {
        canMove = false;
        canChangeStates = false;
        animState = animStates.ChargeShot;
        yield return GeneralUtil.WaitForFrames(chargeShot.castDelay);
        Debug.Log("Boom Boom");
        chargeShot.CastSpell(transform.position, Target);
        yield return 0;
        canMove = true;
        canChangeStates = true;

    }
    protected override IEnumerator DoAtk(FrameData data)
    {
        canChangeStates = false;
        canMove = false;
        base.DoAtk(data);
        canChangeStates = true;
        canMove = true;
        yield return 0;
    }*/
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chargeShotRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

}
