using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Hitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider[] colliders = new Collider[1];
    public AttackStuff.DamageTypes element;
    public float damage;
    [System.NonSerialized]
    public Attack attack;
    public bool bPlayer;
    [SerializeField]
    private bool shouldBeTrigger;
    public bool startEnabled;
    Rigidbody RB;
    void Start()
    {
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                collider.isTrigger = shouldBeTrigger;
                collider.enabled = startEnabled;
            }
        }
        RB = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public event Action<Hitbox> finishMove;
    public void FinishMove()
    {
        if (finishMove != null)
        {
            finishMove.Invoke(this);
            Delegate[] subscribers = finishMove.GetInvocationList();
            foreach (var d in subscribers)
            {
                finishMove -= (d as Action<Hitbox>);
            }
        }
    }
}
