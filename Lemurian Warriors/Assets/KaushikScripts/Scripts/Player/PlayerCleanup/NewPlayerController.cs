//using BehaviorDesigner.Runtime.Tasks;
//using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using TreeEditor;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(InputController3rdP))]
public class NewPlayerController : CharacterBase
{
    bool canMove;
    bool canStun = true;
    bool invincible = false;
    bool canActCache;
    public LayerMask canMoveTo;
    [SerializeField]
    float acceleration;
    public float walkSpeed;
    public float sprintSpeed;
    public float rollSpeed;
    bool rolling;
    bool bufferedRoll;
    bool sprinting;
    bool bufferedLClick;
    int lClickBuffer;
    Vector3 lClickBufferTarget;
    Vector3 movementBuffer;
    int toCast = -1;
    int toCast3rd = 0;
    int casting;
    Vector3 castTarget;
    private Vector3 controlledMovement;
    public NewSpell[] spells = new NewSpell[4];
    int currentMelee;
    //public AnimationClip[] meleeAttacks = new AnimationClip[2];
    [SerializeField]
    private Animator anim;
    public enum AnimStates { IdleRunSprint, Roll, Hurt, Melee, GroundPound, WaterWhip, StraightLine, Throw, SelfCast, Death }
    AnimStates state = AnimStates.IdleRunSprint;
    Ray cameraRay;
    RaycastHit hitInfo;
    public CameraFollowPlayer cam;
    public Transform camLookAt;
    public Vector3 defaultCamLookAt;
    public Transform aimingCamLookAt;
    [System.NonSerialized]
    public bool aiming;
    public bool topDown = true;
    private GameObject[] aimObjects = new GameObject[4];
    public Vector3 hideStuff;

    // Start is called before the first frame update
    new void Start()
    {
        cam = Camera.main.GetComponent<CameraFollowPlayer>();
        defaultCamLookAt = camLookAt.transform.localPosition;
        controlledMovement = Vector3.zero;
        InputController.current.enabled = topDown;
        InputController3rdP.current.enabled = !topDown;
        damCon = GetComponent<DamageController>();
        hurtboxManager = GetComponent<HurtboxManager>();
        hurtboxManager.takeDamage += TakeDamage;
        //ignore collisions with player projectiles.
        Physics.IgnoreLayerCollision(gameObject.layer, 11);
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
        toCast = -1;
        if(navAgent == null)
        {
            //requires component so this should always work.
            navAgent = GetComponent<NavMeshAgent>();
        }
        ResetAimObjects();
        if (InputController.current != null)
        {
            //hopefully the player will never be destroyed at runtime, but if it is, you're going to need to unsubscribe everything in OnDestroy. Can't be that hard.
            InputController.current.onRightClick = RightClick;
            InputController.current.onLeftClick = LeftClick;
            InputController.current.onSelectSpell = SelectSpell;
            InputController.current.onSprint = Sprint;
            InputController.current.onSpace = Roll;

        }
        if(InputController3rdP.current != null)
        {
            InputController3rdP.current.NextSpell = NextSpell;
            InputController3rdP.current.PrevSpell = PrevSpell;
            InputController3rdP.current.LeftClick = LeftClick;
            InputController3rdP.current.RightClick = RClick3rd;
            InputController3rdP.current.RightClickUp = RClickUp3rd;
            InputController3rdP.current.Movement = Move3rd;
            InputController3rdP.current.Sprint = Sprint;
        }
        if (hurtboxManager == null)
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
            canMove = true;
            //can act if idle
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            CameraModeToggle();
        }
        //Debug.Log(canAct);
        if (topDown)
        {
            anim.SetFloat("MovementCos", VectorMath.DistanceInDirection(navAgent.velocity, transform.right));
            anim.SetFloat("MovementSin", VectorMath.DistanceInDirection(navAgent.velocity, transform.forward));
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            anim.SetFloat("MovementCos", VectorMath.DistanceInDirection(controlledMovement, transform.right));
            anim.SetFloat("MovementSin", VectorMath.DistanceInDirection(controlledMovement, transform.forward));
        }
        HideAimObjects();
        
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
        if (!canMove)
        {
            navAgent.speed = 0;
            navAgent.acceleration = 10000;
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
                if (bufferedRoll)
                {
                    Roll();
                }
            }
            canActCache = canAct;
        }
        else
        {
            if (rolling)
            {
                navAgent.Move(transform.forward * rollSpeed * Time.deltaTime);
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
        if (topDown)
        {
            
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
                    else if (spells[toCast].Type == NewSpell.SpellType.SingleTarget)
                    {
                        tempAimPos = GetMousePosition();
                    }
                    //determine target position for self cast AOE spells
                    else if (spells[toCast].Type == NewSpell.SpellType.AOE && spells[toCast].origin == NewSpell.AOESpellOrigin.Self)
                    {
                        tempAimPos = transform.position + VectorMath.LocalToWorld(spells[toCast].VFXSpawnPos, transform);
                    }
                    //move target object to target position
                }
            }
        }
        else
        {
            if (toCast >= 0 && aimObjects[toCast] != null)
            {
                if (spells[toCast].clickToCast)
                {
                    //determine target for straight line projectiles
                    if (!spells[toCast].straightLine)
                    {
                        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out hitInfo, 100, cam.blocksProjectiles))
                        {
                            tempAimPos = hitInfo.point;
                        }
                        else
                        {
                            tempAimPos = cam.transform.position + 100 * cam.transform.forward;
                        }
                    }
                    else if (spells[toCast].Type == NewSpell.SpellType.SingleTarget)
                    {
                        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out hitInfo, 100, cam.blocksProjectiles))
                        {
                            tempAimPos = hitInfo.point;
                        }
                    }
                    //determine target position for self cast AOE spells
                    else if (spells[toCast].Type == NewSpell.SpellType.AOE && spells[toCast].origin == NewSpell.AOESpellOrigin.Self)
                    {
                        tempAimPos = transform.position + VectorMath.LocalToWorld(spells[toCast].VFXSpawnPos, transform);
                    }
                    //move target object to target position
                }
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
        if (canAct)
        {
            canAct = false;
            canBuffer = false;
            navAgent.acceleration = 100000000;
            navAgent.speed = rollSpeed;
            navAgent.isStopped = true;
            transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(GetMousePosition() - transform.position), Vector3.up);
            rolling = true;
            anim.SetInteger("State", (int)AnimStates.Roll);
            //Invoke("EndRoll", 0.5f);
        }
        else if (canBuffer)
        {
            bufferedRoll = true;
        }
    }
    void EndRoll()
    {
        rolling = false;
        navAgent.speed = sprinting ? sprintSpeed : walkSpeed;
        navAgent.destination = transform.position;
        navAgent.isStopped = false;
    }
    protected override void TakeDamage(Hitbox hit)
    {
        if (!invincible)
        {
            if (rolling)
            {
                EndRoll();
            }
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
                    canMove = spells[casting].canMoveDuring;
                }
                if (!spells[casting].canMoveDuring) controlledMovement = Vector3.zero;
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(VectorMath.ZeroY(target - transform.position), Vector3.up);
            canMove = false;
            anim.SetInteger("State", (int)AnimStates.Melee);
            anim.SetInteger("Atk Num", currentMelee);
            controlledMovement = Vector3.zero;
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
        if (canStun)
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
        canMove = true;
        if (rolling)
        {
            EndRoll();
        }
        CanBufferFalse();
    }
    new void CanActFalse()
    {
        if (rolling)
        {
            rolling = false;
        }
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
        if (sprinting)
        {
            navAgent.speed = sprintSpeed;
        }
        else
        {
            navAgent.speed = walkSpeed;
        }
        navAgent.acceleration = 10;
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
    void CameraModeToggle()
    {
        topDown = !topDown;
        if (topDown)
        {
            Camera.main.transform.LookAt(transform);
        }
        else
        {
        }
        navAgent.isStopped = !topDown;
        InputController.current.enabled = topDown;
        InputController3rdP.current.enabled = !topDown;
    }
    void NextSpell()
    {
        toCast3rd++;
        if (toCast3rd > 3) toCast3rd = 0;
    }
    void PrevSpell()
    {
        toCast3rd--;
        if (toCast3rd < 0) toCast3rd = 3;
    }
    void LClick3rd()
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
    void RClick3rd()
    {
        toCast = toCast3rd;
        aiming = !(spells[toCast].Type == NewSpell.SpellType.SelfCast || (spells[toCast].Type == NewSpell.SpellType.AOE && spells[toCast].origin == NewSpell.AOESpellOrigin.Self));
    }
    void RClickUp3rd()
    {
        toCast = -1;
        aiming = false;
        if(anim.GetInteger("State") == 0)
        {
            GoToIdle();
        }
    }
    void Move3rd(Vector3 movement, float time)
    {
        if (canAct)
        {
            movement *= (sprinting ? sprintSpeed : walkSpeed);
            if (VectorMath.DistanceInDirection(movement, controlledMovement) == -movement.magnitude && movement.magnitude != 0)
            {
                controlledMovement += Vector3.Cross(controlledMovement, Vector3.up)/5;
            }
            else if(controlledMovement.magnitude <= .01 && movement.magnitude > 0)
            {
                controlledMovement += transform.forward;
            }
            controlledMovement += Vector3.ClampMagnitude((movement - controlledMovement).normalized * acceleration * time, (movement - controlledMovement).magnitude);
            controlledMovement = Vector3.ClampMagnitude(controlledMovement, (sprinting ? sprintSpeed : walkSpeed));
            //Debug.Log(movement.z);
            navAgent.Move(controlledMovement * time);
            if (!aiming)
            {
                if (controlledMovement.magnitude > 0.01) transform.rotation = Quaternion.LookRotation(controlledMovement);
            }
            else
            {
                transform.LookAt(transform.position + cam.transform.forward);
            }
        }
    }
    void PrintA()
    {
        print("a");
    }
    void PrintB()
    {
        print("b");
    }
}
