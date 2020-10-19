using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Hurtbox : MonoBehaviour
{
    public Collider[] colliders = new Collider[1];
    public bool isPlayer;
    private List<Hitbox> immune  = new List<Hitbox>();
    public HurtboxManager manager;
    // Start is called before the first frame update
    void Start()
    {
        if(colliders == null)
        {
            colliders = GetComponents<Collider>();
        }
        if(manager == null)
        {
            manager = transform.root.GetComponent<HurtboxManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public event Action<float, AttackStuff.DamageTypes> takeDamage;
    void RegisterHit(Hitbox hit)
    {
        //prevent this hitbox from dealing damage to other hurtboxes.
        AvoidDoubleHits(hit);
        //if it's not something that can multihit, prevent damage from other hitboxes in the same attack.
        if (hit.attack != null && !hit.attack.canMultiHit)
        {
            foreach(HitboxData thing in hit.attack.hitboxes)
            {
                AvoidDoubleHits(thing.hitbox);
            }
        }
        //this should always have a manager but just in case it doesn't exist, it shouldn't cause an error.
        if (manager != null)
        {
            manager.TakeDamage(hit);
        }
    }
    void AvoidDoubleHits(Hitbox hit)
    {
        if (manager != null)
            manager.immune.Add(hit);
        hit.finishMove += RemoveImmunity;
    }
    void RemoveImmunity(Hitbox hitbox)
    {
        if (manager != null)
            manager.immune.Remove(hitbox);
    }
    private void OnCollisionEnter(Collision col)
    {
        //first check to see if the collision was on a hurtbox is potentially unnecessary but it's here just to be safe.
        if (hurtboxesContains(col.GetContact(0).thisCollider))
        {
            Debug.Log("Collision With Hitbox");
            if (GeneralUtil.Has<Hitbox>(col.gameObject))
            {
                Hitbox hit = null;
                //it's possible for there to be multiple hitboxes on the same object
                foreach (Hitbox h in col.gameObject.GetComponents<Hitbox>())
                {
                    //if the was in the hitbox's collider list and this and the hitbox aren't on the same team, and the manager isn't immune to it. Goes in order in the editor.
                    if (h.colliders.Contains<Collider>(col.collider) && h.bPlayer != isPlayer && (manager != null ? !manager.immune.Contains<Hitbox>(h) : true))
                    {
                        hit = h;
                    }
                }
                if(hit != null)
                {
                    RegisterHit(hit);
                }
            }
        }
    }
    //basically the same as the collision, but with triggers and there's no collision so i can't check if it hit an actual hurtbox.
    //for this reason, please make sure that the hurtbox colliders are the only ones on any given gameobject. 
    //You can make children of bones to put hurtboxes on them without changing animations.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with Hurtbox");
        Hitbox hit = null;
        foreach (Hitbox h in other.GetComponents<Hitbox>())
        {
            if (h.colliders.Contains<Collider>(other) && h.bPlayer != isPlayer && (manager != null ? !manager.immune.Contains<Hitbox>(h) : true))
            {
                hit = h;
            }
        }
        if (hit != null)
        {
            Debug.Log(gameObject.name + " hit by " + other.gameObject.name);
            RegisterHit(hit);
        }
    }
    bool hurtboxesContains(Collider col)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i] == col)
            {
                return true;
            }
        }
        return false;
    }
}
