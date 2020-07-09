using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(Spell))]
public class SpellEditor : Editor
{
    public SerializedProperty
        spellNameProp, spellIconProp, TypeProp, originProp, damTypeProp,
        statProp, statMultiplierProp, baseDamageProp, cooldownTimeProp, VFXProp;
    // Start is called before the first frame update
    private void OnEnable()
    {
        spellNameProp = serializedObject.FindProperty("spellName");
        spellIconProp = serializedObject.FindProperty("spellIcon");
        TypeProp = serializedObject.FindProperty("Type");
        originProp = serializedObject.FindProperty("origin");
        damTypeProp = serializedObject.FindProperty("damType");
        statProp = serializedObject.FindProperty("stat");
        statMultiplierProp = serializedObject.FindProperty("statMultiplier");
        baseDamageProp = serializedObject.FindProperty("baseDamage");
        cooldownTimeProp = serializedObject.FindProperty("cooldownTime");
        VFXProp = serializedObject.FindProperty("VFX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(spellIconProp, new GUIContent("Spell Icon"));
        EditorGUILayout.PropertyField(spellNameProp, new GUIContent("Spell Name"));
        EditorGUILayout.PropertyField(TypeProp, new GUIContent("Spell Type"));

        Spell.SpellType t = (Spell.SpellType)TypeProp.enumValueIndex;

        switch (t)
        {
            case Spell.SpellType.SingleTarget:
                EditorGUILayout.PropertyField(damTypeProp, new GUIContent("Element"));
                EditorGUILayout.PropertyField(baseDamageProp, new GUIContent("Damage"));
                EditorGUILayout.PropertyField(VFXProp, new GUIContent("VFX"));
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cooldown Time"));
                break;
            case Spell.SpellType.AOE:
                EditorGUILayout.PropertyField(damTypeProp, new GUIContent("Element"));
                EditorGUILayout.PropertyField(originProp, new GUIContent("Origin"));
                EditorGUILayout.PropertyField(baseDamageProp, new GUIContent("Damage"));
                EditorGUILayout.PropertyField(VFXProp, new GUIContent("VFX"));
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cooldown Time"));
                break;
            case Spell.SpellType.SelfCast:
                EditorGUILayout.PropertyField(statProp, new GUIContent("Stat Effected"));
                EditorGUILayout.PropertyField(statMultiplierProp, new GUIContent("Stat Multiplier"));
                EditorGUILayout.PropertyField(VFXProp, new GUIContent("VFX"));
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cooldown Time"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

}
