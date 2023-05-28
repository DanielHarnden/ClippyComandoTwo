using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : MonoBehaviour
{
    public Transform bulletPrefab;
    private GameObject gunBarrel;

    [Header ("Minigun Stats")]
    // Default stats
    private int totalAmmo = 500;
    private int damage = 1;
    private int piercing = 1;
    private float warmUp = 1f;
    // Max stats
    private int MAXtotalAmmo = 1000;
    private int MAXdamage = 2;
    private int MAXpiercing = 2;
    private float MAXwarmUp = 0.2f;
    // Constant stats
    private float shootCooldown = 0.05f;
    private float bulletSpeed = 3000f;
    // Temp stats
    private float shootTimer;
    private bool canShoot = true;
    private bool shooting;
    private bool warmingUp;
    private bool warmedUp;
    private float warmUpTimer;

    [Header ("Audio Visual Effects")]
    //private Text ammoUI;
    public AudioClip emptyClipAudio;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    public Sprite gunOne;
    public Sprite gunTwo;

    private AudioSource thisAudio;
    private ParticleSystem thisParticles;


    void Start()
    {
        thisAudio = this.gameObject.GetComponent<AudioSource>();
        thisParticles = this.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        //ammoUI = GameObject.FindGameObjectWithTag("AmmoUI").GetComponent<Text>();
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
        } else {
            warmUpTimer = warmUp;
            warmedUp = false;
            shooting = false;
        }

        // Shoot cooldown timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        } else {
            canShoot = true;
        }

        if (shooting)
        {
            if (GetComponent<SpriteRenderer>().sprite == gunOne)
            {
                GetComponent<SpriteRenderer>().sprite = gunTwo;
            } else {
                GetComponent<SpriteRenderer>().sprite = gunOne;
            }

            /*if (!thisAudio.isPlaying)
            {
                thisAudio.Play();
            }*/
        } else {
            thisAudio.Stop();
        }
    }



    public void ShootCheck()
    {
        if (!warmedUp)
        {
            if (warmUpTimer > 0)
            {
                if (GetComponent<SpriteRenderer>().sprite == gunOne)
                {
                    GetComponent<SpriteRenderer>().sprite = gunTwo;
                } else {
                    GetComponent<SpriteRenderer>().sprite = gunOne;
                }

                warmUpTimer -= Time.deltaTime;
            } else {
                warmedUp = true;
            }
        } else {
            if (totalAmmo > 0)
            {
                if (canShoot)
                {
                    thisAudio.clip = shootAudio;
                    thisAudio.Play();

                    shooting = true;
                    canShoot = false;
                    shootTimer = 0.05f;
                    Shoot();
                }
            } else {
                shooting = false;
                thisAudio.clip = emptyClipAudio;
                thisAudio.Play();
            }
        }
    }

    void Shoot()
    {
        thisParticles.Play();

        Vector3 spreadPos = gunBarrel.transform.right;
        spreadPos.x += Random.Range(-0.1f, 0.1f);
        spreadPos.y += Random.Range(-0.1f, 0.1f);
        spreadPos.z = 0;

        Rigidbody2D newBul = Instantiate(bulletPrefab, gunBarrel.transform.position,   this.transform.rotation * Quaternion.Euler(0f, 0f, -90f)).GetComponent<Rigidbody2D>();

        newBul.GetComponent<Bullet>().InstantiateStats(1, 5, damage, piercing, false);

        newBul.AddForce(spreadPos * bulletSpeed);
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
                gunBarrel.transform.localPosition = new Vector3 (1.55f, 0.025f, 0f);
            }
        } else {
            this.GetComponent<SpriteRenderer>().flipY = false;
            if (barrelFlipped)
            {
                barrelFlipped = false;
                gunBarrel.transform.localPosition = new Vector3 (1.55f, -0.025f, 0f);
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

    public bool IncreaseDamage(int damageIncreased)
    {
        float tempIncreaseDamage = damage + damageIncreased;

        if (damage == MAXdamage)
        {
            return false;
        }
        else if (tempIncreaseDamage > MAXdamage)
        {
            damage = MAXdamage;
            return true;
        } 
        else {
            damage += damageIncreased;
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
