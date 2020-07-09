/* Code Created by: Collin Ilonummi
 * Code Created on: 5/3/2019
 * Purpose of Script: A placeholder to test functionality that can be added to the actual character controller later
 * Last Updated on: 5/25/19
 */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPlaceHolder : MonoBehaviour
{
    [Tooltip("Insert the characters hand game object, for an origin point of the spell")]
    public GameObject spellTransform;
    // Our array to hold the different spells the player currently has equipped
    public ActiveSpell[] spellSlots;
    // Spped for the character to move, currently unused
    public float speed;
    // The max amount of spells the player can have equipped
    private int maxSpells = 4;
    // If the user sets a value higher than 4 for the spell amount, it will revert to 4 and give a warning
    private void OnValidate()
    {
        if (spellSlots.Length > maxSpells)
        {
            Debug.LogWarning("Do not make more than 4 spells for the player to have!");
            Array.Resize(ref spellSlots, maxSpells);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
