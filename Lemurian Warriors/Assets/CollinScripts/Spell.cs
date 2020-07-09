using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public Sprite spellIcon;
    public enum SpellType { SingleTarget, AOE, SelfCast };
    public SpellType Type;
    public enum AOESpellOrigin { Self, MousePosition }
    public AOESpellOrigin origin;
    public DamageMaster.DamageTypes damType;
    public enum statEffected { Health, Speed, Attack, Defense }
    public statEffected stat;
    public float statMultiplier;
    public float baseDamage;
    public float cooldownTime;
    public GameObject VFX;
}
