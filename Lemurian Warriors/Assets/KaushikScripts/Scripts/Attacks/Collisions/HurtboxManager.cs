using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HurtboxManager : MonoBehaviour
{
    public Hurtbox[] hurtboxes;
    public List<Hitbox> immune = new List<Hitbox>();
    public DamageController damCon;
    [SerializeField]
    bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        isPlayer = GeneralUtil.Has<NewPlayerController>(gameObject);

        hurtboxes = GetComponentsInChildren<Hurtbox>();
        foreach(Hurtbox h in hurtboxes)
        {
            h.isPlayer = isPlayer;
            h.manager = this;
        }
        if(damCon == null)
        {
            damCon = GetComponent<DamageController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public event Action<Hitbox> takeDamage;
    public void TakeDamage(Hitbox h)
    {
        Debug.Log(gameObject.name);
        takeDamage?.Invoke(h);
        //if it exists it'll tell the character to take damage.
    }
}
