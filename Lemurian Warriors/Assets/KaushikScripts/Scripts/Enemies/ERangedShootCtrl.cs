using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ERangedShootCtrl : MonoBehaviour
{
    public RangedEnemy ERanged;
    public Transform firingHand;
    //this is just for animation events
    void Die()
    {
        Destroy(this.gameObject);
    }
    public void MakeCantMove()
    {
        ERanged.canMove = false;
        ERanged.navAgent.destination = ERanged.transform.position;
    }
    public void MakeCanMove()
    {
        ERanged.canMove = true;
    }
    //animation event
    public void MakeCantChangeState()
    {
        ERanged.canChangeStates = false;
    }
    //animation event
    public void MakeCanChangeState()
    {
        ERanged.canChangeStates = true;
    }
    //animation event
    public void Shoot()
    {
        ERanged.basicProjectile.CastSpell(firingHand == null ? transform.position + VectorMath.LocalToWorld(ERanged.basicProjectile.VFXSpawnPos, transform) : firingHand.transform.position, ERanged.player.transform.position);
    }
    //animation event
    public void ChargeShot()
    {
        ERanged.chargeShot.CastSpell(firingHand == null ? transform.position + VectorMath.LocalToWorld(ERanged.chargeShot.VFXSpawnPos, transform) : firingHand.transform.position, ERanged.player.transform.position);
    }
    public void Melee()
    {

    }
    public void EnableHitbox()
    {

    }
    public void DisableHitbox()
    {

    }
    public void EnableHurtbox()
    {

    }
    public void DisableHurtbox()
    {

    }
}
