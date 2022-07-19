using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private float deathTimer = 5f;
    private int damage = 5;
    private float explosionRadius = 2f;
    private bool die = false;



    public void InstantiateStats(float tempDeathTimer, int tempDamage, float tempExplosionRadius)
    {
        deathTimer = tempDeathTimer;
        damage = tempDamage;
        explosionRadius = tempExplosionRadius;
    }

    // Timer to delete old bullets
    private void Update() 
    {
        if (deathTimer > 0f)
        {
            deathTimer -= Time.deltaTime;
        } else {
            die = true;
        }

        if (die)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius);

            foreach (Collider2D enemy in objects)
            {
                if (enemy.tag == "Enemy")
                {
                    enemy.gameObject.GetComponent<Enemy>().Damaged(damage);
                }
            }

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D colObj) 
    {
        die = true;
    }
}
