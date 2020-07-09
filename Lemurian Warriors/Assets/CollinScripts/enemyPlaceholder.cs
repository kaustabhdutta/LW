/*Code created by: Collin Ilonummi
 *Code Created: 5/3/2019
 * Purpose of Script: Serve as a test for possible enemy functions, and also to ensure damage is working properly
 * Last Updated: 5/25/2019
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPlaceholder : MonoBehaviour
{
    // Set our enemies hp value
    public float health;
    // List possible elemental resistances in the inspector
    public DamageMaster.DamageTypes[] Resistances;
    // List possible elemental weaknesses in the inspector
    public DamageMaster.DamageTypes[] Weaknesses;
    [Tooltip("A value between 0 and 1 to multiply our damage by")]
    [Range(0,1)]
    public float ResistanceMultiplier = 1;
    [Tooltip("A value between 1 and 2 to multiply our damage by")]
    [Range(1,2)]
    public float WeaknessMultiplier = 1;

    // When called, take damage
    public void ApplyDamage(float damage)
    {
        health -= damage;
        Debug.Log("hit for " + damage + " damage!");
        // If dead, remove from sight
        if (health <= 0 && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
    }
}
