/* Code Created by: Collin Ilonummi
 * Created on: 5/6/2019
 * Purpose of Script: Hold a function to be called and handle resistances based on the type of attack
 * Last Updated On: 5/25/2019
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMaster : MonoBehaviour
{
    // An enum used to determine what type of damage will be dealt or resisted
    public enum DamageTypes { Fire, Lightning, Poison, Water, Basic }
    // Check if the enemy is resistant or weak to the type of attack they were struck with. This will be called whenever we call ApplyDamage() on the enemy.
    public static float Damaged(float baseDam, DamageTypes damType, enemyPlaceholder target)
    {
        // A local variable to store our damage data
        float TrueDamage = baseDam;
        // Check if the target is resistant to the type of damage
        if (target.Resistances.Length >= 1)
            for (int i = 0; i < target.Resistances.Length; i++) 
            {

                if (target.Resistances[i] == damType)
                {
                    TrueDamage = baseDam * target.ResistanceMultiplier;
                }
            
            }
        // Check if the enemy is weak to the type of damage
        if (target.Weaknesses.Length >= 1)
            for (int w = 0; w < target.Weaknesses.Length; w++)
            {
                if (target.Weaknesses[w] == damType)
                {
                        TrueDamage = baseDam * target.WeaknessMultiplier;
                }
            }
        return TrueDamage;
    }

    public void AreaDamage(Vector3 launchLocation, Vector3 selfLocation, float Radius, float damage, Spell.AOESpellOrigin origin, DamageTypes element)
    {
        Vector3 location = Vector3.zero;
        // Depending on the origin set in the inspector, set begin position
        switch (origin)
        {
            case Spell.AOESpellOrigin.MousePosition:
                location = launchLocation;
                break;
            case Spell.AOESpellOrigin.Self:
                location = selfLocation;
                break;
        }
        // Create a sphere based on origin and the radius variable, and check for colliders
        Collider[] objectsInRadius = Physics.OverlapSphere(location, Radius);
        foreach (Collider col in objectsInRadius)
        {
            // If the collider has an enemy script attached to it, then apply the damage to the enemy
            enemyPlaceholder enemy = col.GetComponent<enemyPlaceholder>();
            if (enemy != null)
            {
                /* This will calculate the distance between the epicenter of the attack and the enemy, and lower the damage output based on that distance.
                float proximity = (location - enemy.transform.position).magnitude;
                float distanceMultiplier = 1 - (proximity / radius);
                */
                enemy.ApplyDamage(Damaged(damage, element, enemy));
            }
        }
    }

    public static void TargetAttack(Vector3 startTransform, Vector3 targetLocation, float maxDistFromMouse, float Damage, GameObject VFX, DamageTypes element)
    {
        enemyPlaceholder target = null;
        float distance = 10000f;
        Collider[] objectsInRadius = Physics.OverlapSphere(targetLocation, maxDistFromMouse);
        // Check for enemies next to the mouse, but only target the closest enemy
        foreach (Collider col in objectsInRadius)
        {
            enemyPlaceholder enemy = col.GetComponent<enemyPlaceholder>();
            if (enemy != null && distance > Vector3.Distance(targetLocation, enemy.transform.position))
                target = enemy;
        }
        // If we find a target, set the beginning and end of our beam attack, and start the delay
        if (target != null)
        {
            target.ApplyDamage(Damaged(Damage, element, target));
        }
        else
        {
            Debug.Log("No Applicable Target");
        }

    }
}
