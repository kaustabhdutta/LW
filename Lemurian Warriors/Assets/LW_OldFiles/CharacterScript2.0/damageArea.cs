using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageArea : MonoBehaviour
{
    public bool isDamaging;
    public float damage = 10;

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "player")
            col.SendMessage((isDamaging) ? "TakeDamage":"TakeHeal", Time.deltaTime * damage);
    }
}
