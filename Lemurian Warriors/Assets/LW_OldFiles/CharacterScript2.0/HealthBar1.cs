using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar1 : MonoBehaviour
{
    public Image health;
    public Text ratioText;
    public float hitpoints = 2000;
    public float maxHitpoint = 2000;

   

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
    public void TakeDamage(float damage) {
        hitpoints -= damage;
        if (hitpoints<0)
        {
            hitpoints = 0;
            UpdateHealth();
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
        if (collision.gameObject.tag == "player")
        {
            hitpoints -= 20;
            if (hitpoints <= 0)
                hitpoints = 0;
            UpdateHealth();
        }
        else if
            (hitpoints == 0) {
            Destroy(gameObject);
        }
      
    }

    void Death() {
        
    }

}
