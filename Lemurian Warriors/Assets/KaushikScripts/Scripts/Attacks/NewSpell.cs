using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Spell kp", menuName = "Legit Spell")]//names subject to change when we get rid of things.
public class NewSpell : ScriptableObject
{
    public string spellName;//all
    public Sprite spellIcon;//all
    public enum SpellType { SingleTarget, AOE, SelfCast };
    public SpellType Type;//all
    public PlayerController.AnimStates Animation;

    public enum AOESpellOrigin { Self, MousePosition }
    public AOESpellOrigin origin = AOESpellOrigin.MousePosition;//AOE
    public float AOERadius; //AOE
    public float AOEDuration;//AOE

    public float hitDuration;//how long the collider lasts after hit.

    public enum statEffected { Health, Speed, Attack, Defense }
    public statEffected stat;//SelfCast
    public float statMultiplier;//selfcast
    public float buffDuration;//selfcast

    public float  cooldownTime;//all
    float cooldown;

    public GameObject VFX;//all

    public bool clickToCast = true;//all 
    public float lifetime = 3;//all

    public bool canMoveDuring = false;//all
    public bool resetDestinationAfter = false;//!canMoveDuring

    //public Vector3 RelativeSpawnPos;//!SelfCast
    
    public bool canCast = true;
    public bool straightLine = false;
    public float launchSpeed = 10;
    public float gravScale = 9.81f;
    public float minAngle = 0;
    public Vector3 VFXSpawnPos = new Vector3();
    public Vector3 aimObjScale = new Vector3(1, 1, 1);

    public GameObject aimObject;//!selfCast

    private Rigidbody RB;

    public void DoCooldown(float time)
    {
        if (cooldown > 0)
        {
            cooldown -= time;
        }
        canCast = cooldown <= 0;
        //Debug.Log(spellName + canCast);
    }
    public void CastSpell(Vector3 castFrom = new Vector3(), Vector3 target = new Vector3())
    {
        GameObject testProj;
        switch (Type)
        {
            case SpellType.AOE:
                //if self cast, set position to castfrom position, destroy it after AOE duration
                if (origin == AOESpellOrigin.Self)
                {
                    testProj = Instantiate(VFX as GameObject);
                    testProj.transform.position = castFrom;
                    Destroy(testProj, AOEDuration);
                    if (testProj.GetComponent<SpellBehavior>() != null)
                    {
                        testProj.GetComponent<SpellBehavior>().Spell = this;
                        float rad = testProj.GetComponent<SpellBehavior>().AOERadius;
                        testProj.transform.localScale = new Vector3(AOERadius / rad, AOERadius / rad, AOERadius / rad);
                    }
                }
                //if projectile, create it, disable the AOE collider
                else
                {
                    if (canHitTarget(castFrom, target))
                    {
                        testProj = Instantiate(VFX as GameObject);
                        if (testProj.GetComponent<SpellBehavior>() != null)
                        {
                            testProj.GetComponent<SpellBehavior>().Spell = this;
                            testProj.GetComponent<SpellBehavior>().AOE.EndAttack();
                        }
                        RB = testProj.GetComponent<Rigidbody>();
                        if (origin == AOESpellOrigin.MousePosition)
                        {
                            Launch(castFrom, target);
                        }
                    }
                    else
                    {
                        //probably change the aim object or something idk

                    }
                }
                break;
            case SpellType.SelfCast:
                //need to make it create the vfx object
                break;
            case SpellType.SingleTarget:
                if (canHitTarget(castFrom, target))
                {
                    if (VFX != null)
                    {
                        testProj = Instantiate(VFX as GameObject);
                        if (testProj.GetComponent<SpellBehavior>() != null)
                        {
                            testProj.GetComponent<SpellBehavior>().Spell = this;
                            testProj.GetComponent<SpellBehavior>().AOE.EndAttack();
                        }
                        RB = testProj.GetComponent<Rigidbody>();
                        Launch(castFrom, target);
                    }
                }
                break;
        }
        cooldown = cooldownTime;
        canCast = false;
    }
    public bool canHitTarget(Vector3 castFrom, Vector3 target)
    {
        //I honestly forgot this math. basically check if a value is not infinite
        if (!straightLine)
        {
            float yDiff = target.y - castFrom.y;
            float xDiff = (target - castFrom - Vector3.up * yDiff).magnitude;
            return !float.IsNaN(Mathf.Atan((Mathf.Pow(launchSpeed, 2) + Mathf.Sqrt(Mathf.Pow(launchSpeed, 4) - gravScale * (gravScale * Mathf.Pow(xDiff, 2) + 2 * yDiff * Mathf.Pow(launchSpeed, 2)))) / gravScale / xDiff));
        }
        //if it's a straight line it will hit.
        return true;
    }
    void Launch(Vector3 castFrom, Vector3 target)
    {
        //will probably do some stuff with gravity scale later, but it doesn't seem necessary right now.
        //Debug.Log(castFrom);
        RB.gameObject.transform.position = castFrom;
        if (RB != null)
        {
            if (straightLine)
            {
                RB.useGravity = false;
                RB.velocity = (target - castFrom)/(target-castFrom).magnitude * launchSpeed;
            }
            else if(canHitTarget(castFrom, target))
            {
                RB.useGravity = true;
                //get x and y differences
                float yDiff = (target.y - castFrom.y);
                float xDiff = (target - castFrom - Vector3.up * yDiff).magnitude;
                //calculate angle
                float angle = Mathf.Atan((Mathf.Pow(launchSpeed, 2) - Mathf.Sqrt(Mathf.Pow(launchSpeed, 4) - gravScale * (gravScale * Mathf.Pow(xDiff, 2) + 2*yDiff*Mathf.Pow(launchSpeed, 2))))/gravScale / xDiff);
                //set velocity to straight line velocity, unnormalized
                Vector3 vel = (target - castFrom - Vector3.up * yDiff);
                //add the tangent of the launch angle in the up direction
                vel += Vector3.up * Mathf.Tan(angle) * vel.magnitude;
                //set magnitude of the vector.
                RB.velocity = vel.normalized * launchSpeed;
            }
        }
    }
}
/*
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

        newSpell.Type = (NewSpell.SpellType) EditorGUILayout.EnumPopup("Spell Type", newSpell.Type);
        newSpell.Animation = (PlayerController.AnimStates) EditorGUILayout.EnumPopup("Animation", newSpell.Animation);

        //properties that are only there for selfcast.
        if(newSpell.Type == NewSpell.SpellType.SelfCast)
        {
            newSpell.stat = (NewSpell.statEffected)EditorGUILayout.EnumPopup("Stat Effected", newSpell.stat);
            newSpell.statMultiplier = EditorGUILayout.FloatField("Stat Multiplier",newSpell.statMultiplier);
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
        if(!(newSpell.Type == NewSpell.SpellType.SelfCast || (newSpell.Type == NewSpell.SpellType.AOE && newSpell.origin == NewSpell.AOESpellOrigin.Self)))
        {
            newSpell.clickToCast = true;
        }
        newSpell.cooldownTime = EditorGUILayout.FloatField("Reload Time", newSpell.cooldownTime);
        newSpell.VFX = (GameObject)EditorGUILayout.ObjectField("VFX Object", newSpell.VFX, typeof(GameObject), allowSceneObjects: false);
        if(newSpell.Type == NewSpell.SpellType.SingleTarget)
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
    
}*/