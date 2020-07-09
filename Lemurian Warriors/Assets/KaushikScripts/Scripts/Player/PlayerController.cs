using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(Rigidbody))]
//i'm not using this one anymore. See NewPlayerController.
public class PlayerController : CharacterBase
{
    bool firing;
    public NavMeshAgent NavAgent;
    public Camera mainCamera;
    public LayerMask CanMoveTo;
    public float walkSpeed = 3;
    public float runSpeed = 8;
    public float dodgeSpeed = 15;
    public float acceleration;
    public LayerMask IgnoreCollisions;
    public NewSpell[] Spells = new NewSpell[4];
    int currentSpell = -1;

    public float BaseHP = 3;
    public float BaseAtk = 1;
    public float BaseDef = 1;
    public float BaseSpd = 1;

    public string jointName;

    float HP;
    float Atk;
    float Def;
    float Spd;
    private Vector3 hideStuff = new Vector3(0, 0, -500);
    private GameObject[] targetObjects = new GameObject[4];

    private bool canActCache = true;
    private Vector3 movementBuffer;
    private int castBuffer;
    private Vector3 targetBuffer;
    private bool meleeBuffer;
    private bool bufferDodge;
    private Vector3 dodgeBuffer;
    Coroutine CurrentAtk;

    public Animator anim;
    public enum AnimStates { IdleRunSprint, Roll, Hurt, Melee, GroundPound, WaterWhip, StraightLine, Throw, SelfCast}
    public AnimStates animState;
    [System.NonSerialized]
    public bool bInHitStun;
    // Start is called before the first frame update

    void Start()
    {
        if (anim == null) { 
            anim = GetComponent<Animator>();
        }
        //RB = GetComponent<Rigidbody>();
        NavAgent = GetComponent<NavMeshAgent>();
        Physics.IgnoreLayerCollision(9, 11);

        HP = BaseHP;
        Atk = BaseAtk;
        Def = BaseDef;
        Spd = BaseSpd;

        ResetAimObjects();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", NavAgent.velocity.magnitude / runSpeed);
        anim.SetInteger("State", (int)animState);
        anim.SetInteger("Atk Num", attackNum);
        //Debug.Log("Can Act?: " + canAct);
        //Debug.Log("Can Buffer? " + canBuffer);
        if (canAct)
        {
            if (!canActCache)
            {
                //if can act and couldn't last frame
                if (movementBuffer != Vector3.zero)
                {
                    NavAgent.SetDestination(movementBuffer);
                    //move if the player buffered a movement
                }
                if (bufferDodge)
                {
                    //dodge if player buffered a dodge, cancel any attacks that are currently going.
                    EndAttack();
                    StartCoroutine(Dodge(transform.position + dodgeBuffer, 0.5f, 0.4f));
                }
                else if (bufferedMelee) 
                {
                    //if they buffered melee and did not buffer a dodge, do the melee attack
                    //CurrentAtk = StartCoroutine(DoAtk(hitboxes.frameData[attackNum]));
                }
                else
                {
                    //buffer spells
                    PlayerCastSpell(castBuffer, targetBuffer);
                }
            }
            //reset buffers
            bufferDodge = false;
            movementBuffer = Vector3.zero;
            castBuffer = -1;
            bufferedMelee = false;
        }
        //prepare for raycast
        Ray CameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mainCamera.transform.position, CameraRay.direction * 100);
        RaycastHit HitInfo;
        if (canAct)
        {
            //sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                NavAgent.speed = runSpeed;
            }
            else
            {
                NavAgent.speed = walkSpeed;
            }
            NavAgent.speed *= Spd;
        }
        //start spell input
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSpell = 0;
            HideAimObjects();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentSpell = 1;
            HideAimObjects();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentSpell = 2;
            HideAimObjects();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentSpell = 3;
            HideAimObjects();
        }
        //end spell input
        //do the cooldown for each spell
        for (int i = 0; i < 4; i++)
        {
            if (Spells[i] != null)
            {
                Spells[i].DoCooldown(Time.deltaTime);
            }
        }
        //check if spell is selected
        if (currentSpell >= 0 && targetObjects[currentSpell] != null)
        {
            //default target is self
            Vector3 target = transform.position;
            if (Spells[currentSpell].clickToCast)
            {
                //determine target for straight line projectiles
                if (Spells[currentSpell].straightLine)
                {
                    float rayLength;
                    Plane Ground = new Plane(Vector3.up, transform.position);
                    if (Ground.Raycast(CameraRay, out rayLength))
                    {
                        //set target
                        target = CameraRay.GetPoint(rayLength);
                    }
                }
                //determine target for parabolic projectiles
                else
                {
                    if (Physics.Raycast(CameraRay, out HitInfo, 100, CanMoveTo))
                    {
                        target = HitInfo.point;
                    }
                }
                //determine target position for self cast AOE spells
                if (Spells[currentSpell].Type == NewSpell.SpellType.AOE && Spells[currentSpell].origin == NewSpell.AOESpellOrigin.Self)
                {
                    target = transform.position + VectorMath.LocalToWorld(Spells[currentSpell].VFXSpawnPos, transform);
                }
                //move target object to target position
                targetObjects[currentSpell].transform.position = target;
            }
            //if you player can cast the spell and they pressed the button if they had to
            if ((Spells[currentSpell].clickToCast ? Input.GetMouseButton(0) : true) && Spells[currentSpell].canCast)
            {
                //cast spell if they can, buffer if they can't.
                if (canAct)
                {
                    PlayerCastSpell(currentSpell, target);
                }
                else
                {
                    castBuffer = canBuffer ? currentSpell : -1;
                    targetBuffer = target;
                }
            }
        }
        //if not casting a spell
        else
        {
            //left click
            if (Input.GetMouseButtonDown(0))
            {
                //if they can buffer but not act, and they used a melee attack
                if(canBuffer && !canAct && usedMelee && !bufferedMelee)//start sketchy code
                {
                    //on to the next move in the melee combo array
                    attackNum++;
                    //loop attacks
                    /*if(attackNum >= hitboxes.frameData.Length)
                    {
                        attackNum = 0;
                    }*/
                    bufferedMelee = true;
                }
                else
                {
                    //return to 0 attack
                    attackNum = 0;
                    //CurrentAtk = StartCoroutine(DoAtk(hitboxes.frameData[attackNum]));
                }//end sketchy code. this looks exploitable
            }
        }
        //deselecting spells
        if ((Input.GetKeyUp(KeyCode.Q) && currentSpell == 0) ||
            (Input.GetKeyUp(KeyCode.W) && currentSpell == 1) ||
            (Input.GetKeyUp(KeyCode.E) && currentSpell == 2) ||
            (Input.GetKeyUp(KeyCode.R) && currentSpell == 3))
        {
            currentSpell = -1;
            HideAimObjects();
        }
        //movement
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(CameraRay, out HitInfo, 100, CanMoveTo))
            {
                if (canAct)
                {
                    NavAgent.SetDestination(HitInfo.point);
                }
                else if (canBuffer)
                {
                    movementBuffer = HitInfo.point;
                }
                //Debug.Log(NavAgent.velocity.magnitude);
            }
        }
        //dodging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Plane Ground = new Plane(Vector3.up, transform.position);
            float rayLength;
            if (Ground.Raycast(CameraRay, out rayLength))
            {
                //set velocity of new projectile
                Vector3 target = CameraRay.GetPoint(rayLength);
                EndAttack();
                StartCoroutine(Dodge(target, 0.5f, 0.4f));
            }
        }
        canActCache = canAct;
    }
    void EndAttack()
    {
        //self explanatory
        if (CurrentAtk != null)
        {
            StopCoroutine(CurrentAtk);
        }
        canBuffer = false;
        canAct = true;
        //hitboxes.collider.enabled = false;
    }
    //end update
    public void PlayerCastSpell(int spell, Vector3 target = new Vector3())
    {
        //if the spell exists and can be cast
        if (spell >= 0 && spell < 4 && Spells[spell].canCast)
        {
            //if not a selfcast
            if (Spells[spell].Type != NewSpell.SpellType.SelfCast)
            {
                //cast the spell with delay and buffer, with the index, from the player position shifted up to the target position shifted up
                //CurrentAtk = StartCoroutine(CastDelayBuffer(spell, transform.position + Vector3.up * 0.5f, target));
            }
            //if it is a selfcast
            else
            {
                BuffStat(Spells[spell].stat, Spells[spell].buffDuration, Spells[spell].statMultiplier);
            }
        }
    }
    public void BuffStat(NewSpell.statEffected stat, float duration, float multiplier)
    {
        switch (stat)
        {
            case NewSpell.statEffected.Attack:
                Atk *= multiplier;
                break;
            case NewSpell.statEffected.Defense:
                Def *= multiplier;
                break;
            case NewSpell.statEffected.Speed:
                Spd *= multiplier;
                break;
            default:
                break;
        }
        RemoveBuff(duration, stat, multiplier);
    }
    public void Heal(float amount)
    {
        HP = Mathf.Clamp(HP + amount, -1, BaseHP);
    }
    void ResetAimObjects()
    {
        //for hard resets and stuff
        foreach(GameObject obj in targetObjects)
        {
            Destroy(obj);
        }
        for(int i = 0; i < targetObjects.Length; i++)
        {
            if (Spells[i].clickToCast)
            {
                targetObjects[i] = Instantiate(Spells[i].aimObject as GameObject);
                targetObjects[i].transform.position = hideStuff;
                if (Spells[i].Type == NewSpell.SpellType.AOE)
                {
                    //Debug.Log(Spells[i].AOERadius);
                    targetObjects[i].transform.localScale = new Vector3(Spells[i].AOERadius, Spells[i].AOERadius, Spells[i].AOERadius);
                }
                else
                {
                    targetObjects[i].transform.localScale = Spells[i].aimObjScale;
                }
            }
        }
    }
    void HideAimObjects()
    {
        //hide all the gameobjects
        foreach(GameObject obj in targetObjects)
        {
            obj.transform.position = hideStuff;
        }
    }
    IEnumerator RemoveBuff(float time, NewSpell.statEffected stat, float multiplier)
    {
        //I can change this to frames, not sure if I should.
        yield return new WaitForSeconds(time);
        switch (stat)
        {
            case NewSpell.statEffected.Attack:
                Atk /= multiplier;
                break;
            case NewSpell.statEffected.Defense:
                Def /= multiplier;
                break;
            case NewSpell.statEffected.Speed:
                Spd /= multiplier;
                break;
            default:
                break;
        }
    }
    IEnumerator Dodge(Vector3 point, float DodgeTime, float BufferTime)
    {
        //the player can't act, accelerates super fast, set the destination and speed
        canAct = false;
        NavAgent.acceleration = 100000;
        NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        //optional
        NavAgent.destination = transform.position + (point - transform.position).normalized * dodgeSpeed;
        //end optional
        NavAgent.speed = dodgeSpeed;
        //wait till they can buffer
        yield return new WaitForSeconds(DodgeTime - BufferTime);
        canBuffer = true;
        NavAgent.acceleration = acceleration;
        //wait till dodge is over, then stop moving.
        yield return new WaitForSeconds(BufferTime);
        NavAgent.SetDestination(transform.position);
        canAct = true;
        //optional
        NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        //end optional
    }
    IEnumerator EndLag(int frames)
    {
        yield return GeneralUtil.WaitForFrames(frames);
    }
    /*IEnumerator CastDelayBuffer(int spell, Vector3 CastFrom, Vector3 Target)
    {
        //stop moving
        canAct = false;
        canBuffer = false;
        NavAgent.speed = 0;
        animState = Spells[spell].Animation;
        //if not a player position AOE, look at the target.
        if (!(Spells[spell].Type == NewSpell.SpellType.AOE && Spells[spell].origin == NewSpell.AOESpellOrigin.Self)) {
            transform.rotation = Quaternion.LookRotation(Target - transform.position -
            Vector3.up * VectorMath.DistanceInDirection(Target - transform.position, Vector3.up), Vector3.up);
        }
        //can't cast it anymore
        Spells[spell].canCast = false;
        //change order of delays based on the length of endlag and buffer time.
        //there's only startup and cooldown, so if buffer time is longer than endlag, player can buffer before the attack comes out.
        if(Spells[spell].bufferTime >= Spells[spell].endLag)
        {
            yield return GeneralUtil.WaitForFrames(Spells[spell].totalCastTime - Spells[spell].bufferTime);
            canBuffer = true;
            yield return GeneralUtil.WaitForFrames(Spells[spell].castDelay - Spells[spell].totalCastTime + Spells[spell].bufferTime);
            //Debug.Log("Cast " + spell);
            Spells[spell].CastSpell(CastFrom + VectorMath.LocalToWorld(Spells[spell].VFXSpawnPos, transform), Target);
            yield return GeneralUtil.WaitForFrames(Spells[spell].endLag);
            canAct = true;
        }
        else
        {
            yield return GeneralUtil.WaitForFrames(Spells[spell].castDelay);
            Spells[spell].CastSpell(CastFrom + VectorMath.LocalToWorld(Spells[spell].VFXSpawnPos, transform), Target);
            yield return GeneralUtil.WaitForFrames(Spells[spell].endLag - Spells[spell].bufferTime);
            canBuffer = true;
            yield return GeneralUtil.WaitForFrames(Spells[spell].bufferTime);
            canAct = true;
        }
        animState = AnimStates.IdleRunSprint;
    }*/
    public IEnumerator DoHitStun(float hitStunTime)
    {
        if (!bInHitStun)
        {
            bInHitStun = true;
            canAct = false;
            yield return new WaitForSeconds(hitStunTime);
            bInHitStun = false;
            canAct = true;
        }
    }
}