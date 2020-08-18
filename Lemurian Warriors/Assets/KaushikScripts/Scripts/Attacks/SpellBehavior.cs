using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class SpellBehavior : MonoBehaviour
{
    public NewSpell Spell;
    public enum SpellState { none, casting, cast, hit};
    public SpellState cSpellState = SpellState.none;//this is for object grouping and animation. Might not matter rn, but will later.
    public Animator anim;
    public HitboxData projectile;
    public Attack AOE;
    public float AOERadius = 1;
    protected Rigidbody RB;

    // Start is called before the first frame update
    void Start()
    {
        //make sure that the projectile hitbox has all the right info and starts off disabled/
        projectile.DoTheDo(new Attack(new HitboxData[] { projectile }, false));
        if (projectile.hitbox != null && projectile.hitbox.startEnabled)
        {
            projectile.UndoTheDo();
        }
        transform.position += Vector3.zero;
        cSpellState = SpellState.cast;
        RB = GetComponent<Rigidbody>();
        //Debug.Log(RB);
        if (Spell != null)
        {
            if (Spell.straightLine)
            {
                Destroy(gameObject, Spell.lifetime);
            }
            if (Spell.Type == NewSpell.SpellType.AOE && Spell.origin == NewSpell.AOESpellOrigin.Self)
            {
                DoAOE();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RB.velocity != Vector3.zero)
        {
            transform.LookAt(transform.position + RB.velocity);
            //Debug.Log("Rotate");
        }
        if (Spell != null)
        {
            if (Spell.straightLine)
            {
                Destroy(gameObject, Spell.lifetime);
            }
            if (Spell.Type == NewSpell.SpellType.AOE && Spell.origin == NewSpell.AOESpellOrigin.Self)
            {
                DoAOE();
            }
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        //Debug.Log(col.collider.gameObject.tag);
        cSpellState = SpellState.hit;
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
        RB.useGravity = false;
        if (Spell != null)
        {
            switch (Spell.Type)
            {
                case NewSpell.SpellType.AOE:
                    DoAOE();
                    break;
                case NewSpell.SpellType.SelfCast:
                    //Do nothing
                    break;
                case NewSpell.SpellType.SingleTarget:
                    Destroy(this.gameObject, Spell.hitDuration);
                    break;
            }
        }
    }
    protected virtual void DoAOE()
    {

        AOE.StartAttack();
        //AOECollider.radius = Spell.AOERadius/(transform.localScale.x + transform.localScale.y + transform.localScale.z)*3;
        transform.position += Vector3.zero;
        Destroy(gameObject, Spell.AOEDuration);
    }
}
