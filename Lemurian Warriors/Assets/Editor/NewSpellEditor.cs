using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NewSpell))]
public class NewSpellEditor : Editor
{
    /// <summary>
    /// basically i'm setting all of the variables that matter when creating the spell
    /// start with name and icon, then move on to type, with different properties for each type
    /// kinda disorganized, everything is in a somewhat reasonable order.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var newSpell = target as NewSpell;

        newSpell.spellName = EditorGUILayout.TextField("Spell Name: ", newSpell.spellName);
        newSpell.spellIcon = (Sprite)EditorGUILayout.ObjectField("Sprite", newSpell.spellIcon, typeof(Sprite), allowSceneObjects: false);

        newSpell.Type = (NewSpell.SpellType)EditorGUILayout.EnumPopup("Spell Type", newSpell.Type);
        newSpell.Animation = (PlayerController.AnimStates)EditorGUILayout.EnumPopup("Animation", newSpell.Animation);

        //properties that are only there for selfcast.
        if (newSpell.Type == NewSpell.SpellType.SelfCast)
        {
            newSpell.stat = (NewSpell.statEffected)EditorGUILayout.EnumPopup("Stat Effected", newSpell.stat);
            newSpell.statMultiplier = EditorGUILayout.FloatField("Stat Multiplier", newSpell.statMultiplier);
            newSpell.buffDuration = EditorGUILayout.FloatField("Buff Duration", newSpell.buffDuration);
        }
        //properties that are not there for selfcast
        else
        {
            //properties only there for AOE
            if (newSpell.Type == NewSpell.SpellType.AOE)
            {
                newSpell.origin = (NewSpell.AOESpellOrigin)EditorGUILayout.EnumPopup("Origin", newSpell.origin);
                newSpell.AOERadius = EditorGUILayout.FloatField("Radius: ", newSpell.AOERadius);
                newSpell.AOEDuration = EditorGUILayout.FloatField("AOE Duration: ", newSpell.AOEDuration);
            }
            //properties only there for single target
            else
            {
                newSpell.hitDuration = EditorGUILayout.FloatField("hit Duration: ", newSpell.hitDuration);
            }
            if (!(newSpell.Type == NewSpell.SpellType.AOE && newSpell.origin == NewSpell.AOESpellOrigin.Self))
            {
                newSpell.straightLine = GUILayout.Toggle(newSpell.straightLine, "Straight Line");
                newSpell.launchSpeed = EditorGUILayout.FloatField("Launch Speed: ", newSpell.launchSpeed);
                if (!newSpell.straightLine)
                {
                    newSpell.gravScale = EditorGUILayout.FloatField("Gravity", newSpell.gravScale);
                    newSpell.minAngle = EditorGUILayout.Slider("Min Launch Angle:", newSpell.minAngle, 0, 90);
                }
                else
                {
                    newSpell.lifetime = EditorGUILayout.FloatField("Lifetime", newSpell.lifetime);
                }
            }
            newSpell.aimObject = (GameObject)EditorGUILayout.ObjectField("Aim Object", newSpell.aimObject, typeof(GameObject), allowSceneObjects: false);
        }
        if (!(newSpell.Type == NewSpell.SpellType.SelfCast || (newSpell.Type == NewSpell.SpellType.AOE && newSpell.origin == NewSpell.AOESpellOrigin.Self)))
        {
            newSpell.clickToCast = true;
        }
        newSpell.cooldownTime = EditorGUILayout.FloatField("Reload Time", newSpell.cooldownTime);
        newSpell.VFX = (GameObject)EditorGUILayout.ObjectField("VFX Object", newSpell.VFX, typeof(GameObject), allowSceneObjects: false);
        if (newSpell.Type == NewSpell.SpellType.SingleTarget)
        {
            newSpell.aimObjScale = EditorGUILayout.Vector3Field("Aim Object Scale", newSpell.aimObjScale);
        }
        newSpell.VFXSpawnPos = EditorGUILayout.Vector3Field("local VFX spawn", newSpell.VFXSpawnPos);

        if (newSpell.Type != NewSpell.SpellType.SelfCast && newSpell.VFX != null && newSpell.VFX.GetComponent<SpellBehavior>() == null)
        {
            newSpell.VFX = null;
            Debug.Log("Object must have spell behavior script attached.");
        }

        newSpell.clickToCast = GUILayout.Toggle(newSpell.clickToCast, "Click to Cast");
        newSpell.canMoveDuring = GUILayout.Toggle(newSpell.canMoveDuring, "Can move during");
    }

}
