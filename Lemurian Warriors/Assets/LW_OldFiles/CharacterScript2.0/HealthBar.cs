using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Image health;
    public Text ratioText;
    public float hitpoints = 200;
    public float maxHitpoint = 200;
   // public int damage = 30;

    private void Start()
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        float ratio = hitpoints / maxHitpoint;
        health.rectTransform.localScale = new Vector3(ratio, 1, 1);
        ratioText.text = (ratio * 100).ToString("0") + '%';
    }
    private void TakeDamage(float damage) {
        hitpoints -= damage;
        if (hitpoints<0)
        {
            hitpoints = 0;
            Debug.Log("you died");
        }
        if (hitpoints > maxHitpoint)
            hitpoints = maxHitpoint;
        UpdateHealth();
    }
    private void HealDamge(float heal)
    {
        hitpoints += heal;
        if (hitpoints > maxHitpoint)
            hitpoints = maxHitpoint;
       UpdateHealth();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DamagePlayer")
        {
            hitpoints -= 10;
            if (hitpoints <=0)
                hitpoints = 0;
            UpdateHealth();
        }
    }
}
