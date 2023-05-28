using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header ("Enemy Stats")]
    // 1 = Ruler, 2 = Gluer, 3 = Gunner
    public int enemyType = 1;
    public int health = 10;
    public int damage = 1;
    public float moveSpeed = 2f;
    public bool playerSeen = false;
    public float idleTime = 5f;

    public float friendAlertRadius = 5f;
    private bool dead;

    [Header ("Ruler Stats")]
    public float chargePrepTime = 2f;
    public float chargeTime = 1f;
    public int sharpness = 0;
    
    [Header ("Gluer Stats")]

    [Header ("Gunner Stats")]
    public float bulletSpeed = 500f;
    public float shootTime = 2.5f;




    [Header ("Audio Visual Effects")]
    private Text healthUI;
    //private AudioSource[] enemyAudio;
    //private ParticleSystem enemyParticles;
    //public GameObject[] bits;


    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().Damaged(damage);
        }
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!playerSeen && other.gameObject.tag == "Player")
        {
            AlertFriends();
        }
    }



    public void AlertFriends()
    {
        if (!playerSeen)
        {
            playerSeen = true;

            // TODO: Add sound effect

            Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(this.transform.position, friendAlertRadius);

            foreach(Collider2D friend in allOverlappingColliders)
            {
                Enemy friendStats = friend.GetComponentInParent<Enemy>();

                if (friendStats != null)
                {
                    friendStats.AlertFriends();
                    friendStats.playerSeen = true;
                }
            }
        }
    }







    
    

    void Start() 
    {
        healthUI = this.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        switch (enemyType)
        {
            case 1:
                this.gameObject.GetComponent<RulerAI>().UpdateStats(moveSpeed, idleTime, chargePrepTime, chargeTime, sharpness);
                break;

            default:
                Debug.Log("Enemy type assigned incorrectly");
                break;
        }
    }
    
    void Update()
    {
        healthUI.text = health.ToString();

       if (health <= 0)
       {
           KillEnemy();
       } 
    }

    public void Damaged(int damageInflicted)
    {
        playerSeen = true;
        health -= damageInflicted;

        switch(enemyType)
        {
            case 1:
                this.gameObject.GetComponent<RulerAI>().Damage();
                break;

            default:
                Debug.Log("Enemy type invalid.");
                break;
        }
    }





    // TODO: Fix this up
    void KillEnemy()
    {
        Destroy(this.gameObject);
        /*if (!dead)
        {
            dead = true;
            gameObject.tag = "Untagged";
            this.GetComponent<SpriteRenderer>().enabled = false;
            
            foreach(Transform child in transform)
            {
                if (child.GetComponent<ParticleSystem>() == null)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            //aiPath.enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;

            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().enemiesLeft -= 1;

            /*for (int i = 0; i < bits.Length; i++)
            {
                Instantiate(bits[i], this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0f), this.transform.rotation);
            }
        } else {
            /*if (!rulerAudio.isPlaying && !rulerParticles.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }*/
    }
}
