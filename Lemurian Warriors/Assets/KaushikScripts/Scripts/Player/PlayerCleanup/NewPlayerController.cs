using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NewPlayerController : CharacterBase
{
    [System.NonSerialized]
    new public bool canAct = true;
    bool canGetStunned = true;
    bool canActCache;
    public LayerMask canMoveTo;
    public float walkSpeed;
    public float sprintSpeed;
    public float rollSpeed;
    bool rolling;
    public NewSpell[] spells = new NewSpell[4];
    int toCast = -1;
    int casting;
    public AnimationClip[] meleeAttacks = new AnimationClip[2];
    int currentMelee;
    Vector3 movementBuffer;
    Vector3 castTarget;
    public enum AnimStates { IdleRunSprint, Roll, Hurt, Melee, GroundPound, WaterWhip, StraightLine, Throw, SelfCast, Death }
    AnimStates state = AnimStates.IdleRunSprint;
    public Animator anim;
    bool bufferedLClick;
    int lClickBuffer;
    Vector3 lClickBufferTarget;
    Ray cameraRay;
    RaycastHit hitInfo;
    public Vector3 hideStuff;
    private GameObject[] aimObjects = new GameObject[4];
    bool sprinting;
    [SerializeField]
    bool invincible = false;
    [SerializeField]
    bool canStun = true;
    // Start is called before the first frame update
    void Start()
    {
        damCon = GetComponent<DamageController>();
        hurtboxManager = GetComponent<HurtboxManager>();
        hurtboxManager.takeDamage += TakeDamage;
        //ignore collisions with player projectiles.
        Physics.IgnoreLayerCollision(this.gameObject.layer, 11);
        if(navAgent == null)
        {
            //requires component so this should always work.
            navAgent = GetComponent<NavMeshAgent>();
        }
        ResetAimObjects();
        if (InputController.current != null)
        {
            //hopefully the player will never be destroyed at runtime, but if it is, you're going to need to unsubscribe everything in OnDestroy. Can't be that hard.
            InputController.current.onRightClick += RightClick;
            InputController.current.onLeftClick += LeftClick;
            InputController.current.onSelectSpell += SelectSpell;
            InputController.current.onSprint += Sprint;
            InputController.current.onSpace += Roll;
        }
        if(hurtboxManager == null)
        {
            hurtboxManager = GetComponent<HurtboxManager>();
        }
        foreach(Attack a in attacks)
        {
            foreach(HitboxData h in a.hitboxes)
            {
                h.hitbox.bPlayer = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetInteger("State") == 0)
        {
            canAct = true;
            //can act if idle
        }
        //Debug.Log(canAct);
        anim.SetFloat("MovementCos", VectorMath.DistanceInDirection(navAgent.velocity, transform.right));
        anim.SetFloat("MovementSin", VectorMath.DistanceInDirection(navAgent.velocity, transform.forward));
        HideAimObjects();
        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 tempAimPos = transform.position;
        //default target is self
        float rayLength;
        Plane Ground = new Plane(Vector3.up, transform.position);
        if (Ground.Raycast(cameraRay, out rayLength))
        {
            //set target
            tempAimPos = cameraRay.GetPoint(rayLength);
        }
        if (toCast >= 0 && aimObjects[toCast] != null)
        {
            aimObjects[toCast].transform.position = spells[toCast].Type == NewSpell.SpellType.AOE && spells[toCast].origin == NewSpell.AOESpellOrigin.Self ? transform.position + VectorMath.LocalToWorld(spells[toCast].VFXSpawnPos, transform) : GetAimPos();
        }
        if (canAct)
        {
            navAgent.speed = sprinting ? sprintSpeed : walkSpeed;
            //the following assumes that the player can't have multiple of the same spell in their list.
            foreach(NewSpell spell in spells)
            {
                spell.DoCooldown(Time.deltaTime);
            }
            if (!canActCache)
            {
                if (bufferedLClick)
                {
                    //do whatever attack/spell the player buffered.
                    StartCast(lClickBuffer, lClickBufferTarget);
                    bufferedLClick = false;
                    Debug.Log("buffer lclick " + lClickBuffer);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        
    }
    Vector3 GetAimPos()
    {
        Plane Ground = new Plane(Vector3.up, transform.position);
        Vector3 tempAimPos;
        float rayLength;
        Ground.Raycast(cameraRay, out rayLength);
        tempAimPos = cameraRay.GetPoint(rayLength);
        if (toCast >= 0 && aimObjects[toCast] != null)
        {
            if (spells[toCast].clickToCast)
            {
                //determine target for straight line projectiles
                if (!spells[toCast].straightLine)
                {
                    if (Physics.Raycast(cameraRay, out hitInfo, 100, canMoveTo))
                    {
                        tempAimPos = hitInfo.point;
                    }
                }
                //determine target position for self cast AOE spells
                /*if (spells[toCast].Type == NewSpell.SpellType.AOE && spells[toCast].origin == NewSpell.AOESpellOrigin.Self)
                {
                    tempAimPos = transform.position + VectorMath.LocalToWorld(spells[toCast].VFXSpawnPos, transform);
                }*/
                //move target object to target position
            }
        }
        return tempAimPos;
    }
    //returns the position of the mouse on the vertical plane that the player is on
    Vector3 GetMousePosition()
    {
        Plane Ground = new Plane(Vector3.up, transform.position);
        float rayLength;
        Ground.Raycast(cameraRay, out rayLength);
        return cameraRay.GetPoint(rayLength);
    }
    void LeftClick()
    {
        if (canAct)
        {
            StartCast(toCast, GetAimPos());
        }
        else if (canBuffer)
        {
            bufferedLClick = true;
            lClickBuffer = toCast;
            lClickBufferTarget = GetAimPos();

        }
    }
    void RightClick()
    {
        if (Physics.Raycast(cameraRay, out hitInfo, 100, canMoveTo))
        {
            if (canAct)
            {
                navAgent.SetDestination(hitInfo.point);
            }
            else if (canBuffer)
            {
                movementBuffer = hitInfo.point;
            }
        }
    }
    void Sprint(bool sprint)
    {
        sprinting = sprint;
    }
    void Roll()
    {
        if (!rolling)
        {
            rolling = true;
            navAgent.acceleration = 100000000;
            navAgent.speed = rollSpeed;
            navAgent.destination = GetAimPos();
            transform.rotation = Quaternion.LookRotation(GetAimPos());
            CanActFalse();
            anim.SetInteger("State", (int)AnimStates.Roll);
            Invoke("EndRoll", 0.5f);
        }
    }
    void EndRoll()
    {
        rolling = false;
        navAgent.speed = sprinting ? sprintSpeed : walkSpeed;
        navAgent.destination = transform.position;
        CanActTrue();
    }
    protected override void TakeDamage(Hitbox hit)
    {
        if (!invincible)
        {
            if (canStun)
            {
                state = AnimStates.Hurt;
                anim.SetInteger("State", (int)AnimStates.Hurt);
                Debug.Log("Go To Hurt");
            }
            base.TakeDamage(hit);
            if(damCon.currentHealth <= 0)
            {
                Die();
            }
        }
    }
    void Die()
    {
        state = AnimStates.Death;
        anim.SetInteger("State", (int)state);
    }
    void StartCast(int spellIndex, Vector3 target)
    {
        //so the spell animation event knows where the player aimed
        castTarget = target;
        casting = spellIndex;
        if (spellIndex >= 0 && spellIndex < spells.Length)
        {
            if (spells[spellIndex].canCast)
            {
                if (spells[spellIndex].canHitTarget(transform.position + VectorMath.LocalToWorld(spells[spellIndex].VFXSpawnPos, transform), target)
                    || spells[spellIndex].Type == NewSpell.SpellType.SelfCast || (spells[spellIndex].Type == NewSpell.SpellType.AOE && spells[spellIndex].origin == NewSpell.AOESpellOrigin.Self))
                {
                    NewSpell spell = spells[casting];
                    if (spell.Type != NewSpell.SpellType.SelfCast)
                    {
                        transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(target - transform.position), Vector3.up);
                    }
                    casting = spellIndex;
                    anim.SetInteger("State", (int)spells[spellIndex].Animation);
                }
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(target - transform.position), Vector3.up);
            anim.SetInteger("State", (int)AnimStates.Melee);
            anim.SetInteger("Atk Num", currentMelee);
        }
    }
    //animation event, apparently there's no bool field for anim events.
    void HurtboxToggle(int True)
    {
        if (hurtboxManager != null)
        {
            foreach (Hurtbox hurtbox in hurtboxManager.hurtboxes)
            {
                foreach (Collider col in hurtbox.colliders)
                {
                    col.enabled = True != 0;
                }
            }
        }
    }
    void Hurt()
    {
        //play the hurt animation no matter where you were.
        if (canGetStunned)
        {
            anim.Play("Hurt", -1, 0);
        }
    }
    void SelectSpell(int spell)
    {
        toCast = spell;
    }
    void CastSpell()
    {
        Debug.Log("Casting Spell " + casting);
        spells[casting].CastSpell(transform.position + VectorMath.LocalToWorld(spells[casting].VFXSpawnPos, transform), castTarget);
    }
    
    new void CanActTrue()
    {
        anim.SetInteger("State", (int)AnimStates.IdleRunSprint);
        canAct = true;
        CanBufferFalse();
    }
    new void CanActFalse()
    {
        canAct = false;
        CanBufferFalse();
    }
    void CanBufferTrue()
    {
        canBuffer = true;
    }
    void CanBufferFalse()
    {
        canBuffer = false;
    }
    void GoToIdle()
    {
        state = AnimStates.IdleRunSprint;
        anim.SetInteger("State", (int)state);
        canStun = true;
        CanActTrue();
    }
    void ResetAimObjects()
    {
        //for hard resets and stuff
        foreach (GameObject obj in aimObjects)
        {
            Destroy(obj);
        }
        for (int i = 0; i < aimObjects.Length; i++)
        {
            if (spells[i].clickToCast)
            {
                aimObjects[i] = Instantiate(spells[i].aimObject as GameObject);
                aimObjects[i].transform.position = hideStuff;
                if (spells[i].Type == NewSpell.SpellType.AOE)
                {
                    //Debug.Log(spells[i].AOERadius);
                    aimObjects[i].transform.localScale = new Vector3(spells[i].AOERadius, spells[i].AOERadius, spells[i].AOERadius);
                }
                else
                {
                    aimObjects[i].transform.localScale = spells[i].aimObjScale;
                }
            }
        }
    }
    void HideAimObjects()
    {
        //hide all the gameobjects
        foreach (GameObject obj in aimObjects)
        {
            obj.transform.position = hideStuff;
        }
    }
}
