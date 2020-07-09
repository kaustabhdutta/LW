using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dealDamage : MonoBehaviour
{
    public float damage = -25;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<HealthBar1>().TakeDamage(damage);

    }
    public void damageEnemy() {

    }
}
