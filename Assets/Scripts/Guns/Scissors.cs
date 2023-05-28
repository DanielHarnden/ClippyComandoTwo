using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scissors : MonoBehaviour
{
    public Transform bulletPrefab;
    private GameObject gunBarrel;

    [Header ("Scissors Stats")]
    // Default stats
    private int totalAmmo = 1;
    private int damage = 9999;
    private int piercing = 5;
    // Max stats
    private int MAXtotalAmmo = 1000;
    private int MAXpiercing = 20;
    // Constant stats
    private float shootCooldown = 5f;
    private float bulletSpeed = 1000f;
    // Temp stats
    private float shootTimer;
    private bool canShoot = true;

    //[Header ("Audio Visual Effects")]
    //private Text ammoUI;
    //private AudioSource thisGun;
    //private AudioSource emptyClip;


    void Start()
    {
        //ammoUI = GameObject.FindGameObjectWithTag("AmmoUI").GetComponent<Text>();
        //thisGun = this.gameObject.GetComponent<AudioSource>();
        //emptyClip = this.transform.parent.GetComponent<AudioSource>();
    }

    // Not in start because if it was switching while false would softlock the gun.
    void OnEnable() 
    {
        canShoot = true;
        gunBarrel = GameObject.FindGameObjectWithTag("GunTip");
    }

    void Update()
    {
        AimGun();

        // Updates ammo UI
        //ammoUI.text = clipAmmo.ToString() + "/" + totalAmmo.ToString();

        // Shoot
        if (Input.GetKey(KeyCode.Mouse0))
        {
            ShootCheck();
        }

        // Shoot cooldown timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        } else {
            canShoot = true;
        }
    }



    public void ShootCheck()
    {
        if (totalAmmo > 0)
        {
            if (canShoot)
            {
                canShoot = false;
                shootTimer = shootCooldown;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        Rigidbody2D newBul = Instantiate(bulletPrefab, gunBarrel.transform.position,   this.transform.rotation * Quaternion.Euler(0f, 0f, 0f)).GetComponent<Rigidbody2D>();

        newBul.GetComponent<Bullet>().InstantiateStats(6, 5, damage, piercing, true);

        newBul.AddForce(gunBarrel.transform.right * bulletSpeed);
        totalAmmo -= 1;
    }



    // Aims the player's gun towards the mouse position
    private bool barrelFlipped;
    void AimGun()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f; // Distance between camera and object in world space
 
        Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
 
        float angle = (Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg) % 360;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (angle > 90 || angle < -90)
        {
            this.GetComponent<SpriteRenderer>().flipY = true;
            if (!barrelFlipped)
            {   
                barrelFlipped = true;
                gunBarrel.transform.localPosition = new Vector3 (0.575f, 0f, 0f);
            }
        } else {
            this.GetComponent<SpriteRenderer>().flipY = false;
            if (barrelFlipped)
            {
                barrelFlipped = false;
                gunBarrel.transform.localPosition = new Vector3 (0.575f, 0f, 0f);
            }
        }
    }



    public void InstantiateStats(int tempTotalAmmo, float tempShootCooldown, int tempDamage, int tempPiercing)
    {
        totalAmmo = tempTotalAmmo;
        shootCooldown = tempShootCooldown;
        damage = tempDamage;
        piercing = tempPiercing;
    }

    public float[] SaveStats()
    {
        float[] stats = {(float)totalAmmo, shootCooldown, (float)damage, (float)piercing};
        return stats;
    }



    public bool AddAmmo(int ammoAdded)
    {
        int tempAmmo = totalAmmo + ammoAdded;

        if (totalAmmo == MAXtotalAmmo)
        {
            return false;
        }
        else if (tempAmmo > MAXtotalAmmo)
        {
            totalAmmo = MAXtotalAmmo;
            return true;
        } 
        else {
            totalAmmo += ammoAdded;
            return true;
        }
    }

    public bool IncreasePiercing(int pericingIncreased)
    {
        float tempIncreasePiercing = piercing + pericingIncreased;

        if (piercing == MAXpiercing)
        {
            return false;
        }
        else if (tempIncreasePiercing > MAXpiercing)
        {
            piercing = MAXpiercing;
            return true;
        } 
        else {
            piercing += pericingIncreased;
            return true;
        }
    }
}
