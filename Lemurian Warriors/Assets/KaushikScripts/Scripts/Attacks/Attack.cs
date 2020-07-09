using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//this allows grouping of hitboxes. Each hitbox can be changed individually.
public class Attack
{
    public HitboxData[] hitboxes;
    public bool canMultiHit;//checks if multiple hitboxes can hit for the same attack. Best to leave false, since it's difficult to determine which hitbox will collide first.
    public bool bPlayer = true;//checks if this is a player's attack.
    public Attack(HitboxData[] hitboxes, bool canMultiHit)
    {
        this.hitboxes = hitboxes;
        this.canMultiHit = canMultiHit;
    }
    public void StartAttack()
    { 
        foreach(HitboxData h in hitboxes)
        {
            h.DoTheDo(this);
        }
    }
    public void EndAttack()
    {
        foreach (HitboxData h in hitboxes)
        {
            h.UndoTheDo();
        }
    }
}
[System.Serializable]
public struct HitboxData
{
    public float damage;
    public AttackStuff.DamageTypes element;
    public Hitbox hitbox;
    public void DoTheDo(Attack atk)
    {
        if (hitbox != null)
        {
            foreach (Collider col in hitbox.colliders)
            {
                col.enabled = true;
            }
            hitbox.attack = atk;
            hitbox.element = element;
            hitbox.damage = damage;
            hitbox.bPlayer = atk.bPlayer;
            //basically overwrites the stuff on the hitbox.
        }
    }
    public void UndoTheDo()
    {
        if (hitbox != null)
        {
            hitbox.FinishMove();
            //basically tell anything it hit that the attack is over and to remove it from the list of hitboxes it's immune to.
            foreach (Collider col in hitbox.colliders)
            {
                col.enabled = false;
            }
        }
    }
}