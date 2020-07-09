using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellBehavior : SpellBehavior
{
    // Start is called before the first frame update
    void Start()
    {
        cSpellState = SpellState.cast;
        RB = GetComponent<Rigidbody>();
        /*if (AOECollider != null)
        {
            AOECollider.enabled = false;
        }
        if (Spell.straightLine)
        {
            Destroy(gameObject, Spell.lifetime);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (RB.velocity != Vector3.zero)
        {
            transform.LookAt(transform.position + RB.velocity);
            Debug.Log("Rotate");
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.collider.gameObject.layer);
        cSpellState = SpellState.hit;
        RB.velocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
        RB.useGravity = false;
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
