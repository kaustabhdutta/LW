using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    [Range(0.5f, 2)]
    public float FireMult = 1;
    [Range(0.5f, 2)]
    public float WaterMult = 1;
    [Range(0.5f, 2)]
    public float EarthMult = 1;
    [Range(0.5f, 2)]
    public float AirMult = 1;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage, AttackStuff.DamageTypes type)
    {
        Debug.Log("Take Damage");
        float taken = 1;
        switch (type)
        {
            case AttackStuff.DamageTypes.Air:
                taken = AirMult;
                break;
            case AttackStuff.DamageTypes.Earth:
                taken = EarthMult;
                break;
            case AttackStuff.DamageTypes.Fire:
                taken = FireMult;
                break;
            case AttackStuff.DamageTypes.Water:
                taken = WaterMult;
                break;
            case AttackStuff.DamageTypes.None:
                taken = 1;
                break;
        }
        Debug.Log(damage);
        Debug.Log(type);
        currentHealth -= damage * taken;
    }
    public void Heal(float health)
    {
        currentHealth += health;
    }
}
