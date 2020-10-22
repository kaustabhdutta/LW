using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DamageController))]
[RequireComponent(typeof(HurtboxManager))]
//[RequireComponent(typeof(SphereCollider))]
public class CharacterBase : MonoBehaviour
{
    public bool canAct = true;
    public bool canBuffer;
    public Attack[] attacks;
    public HurtboxManager hurtboxManager;
    protected bool usedMelee = false;
    protected bool bufferedMelee = false;
    protected int attackNum = 0;
    public NavMeshAgent navAgent;
    protected DamageController damCon;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (hurtboxManager == null)
        {
            hurtboxManager = GetComponent<HurtboxManager>();
        }

        hurtboxManager.takeDamage += TakeDamage;
        damCon = GetComponent<DamageController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //this will probably never be used cuz i'm switching to animation events
    /*protected virtual IEnumerator DoAtk(FrameData data)
    {
        //assumes that BufferFrames <= TotalTime - Startup
        //other than that pretty self explanatory
        yield return StartCoroutine(GeneralUtil.WaitForFrames(data.Startup));
        hitboxes.collider.enabled = true;
        if (data.BufferFrames > data.Cooldown)
        {
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.Total - data.BufferFrames - data.Startup));
            usedMelee = true;
            canBuffer = true;
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.Startup + data.ActiveFrames - (data.Total - data.BufferFrames)));
            hitboxes.collider.enabled = false;
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.Cooldown));
            canBuffer = false;
            canAct = true;
            usedMelee = false;
        }
        else
        {
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.ActiveFrames));
            hitboxes.collider.enabled = false;
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.Cooldown - data.BufferFrames));
            usedMelee = true;
            canBuffer = true;
            yield return StartCoroutine(GeneralUtil.WaitForFrames(data.BufferFrames));
            canAct = true;
            canBuffer = false;
            usedMelee = false;
        }
    }*/
    protected virtual void TakeDamage(Hitbox hit)
    {
        hurtboxManager.damCon.TakeDamage(hit.damage, hit.element);
    }
    //here are my animation events
    public void UsedMeleeTrue()
    {
        usedMelee = true;
    }
    public void UsedMeleeFalse()
    {
        usedMelee = false;
    }
    public void CanActTrue()
    {
        canAct = true;
    }
    public void CanActFalse()
    {
        canAct = false;
    }
    public virtual void ActivateAttack(int attack)
    {
        if (attack < attacks.Length && attack >= 0)
        {
            attacks[attack].StartAttack();
        }
    }
    public virtual void DeactivateAttack(int attack)
    {
        if (attack < attacks.Length && attack >= 0)
        {
            attacks[attack].EndAttack();
        }
    }
}
