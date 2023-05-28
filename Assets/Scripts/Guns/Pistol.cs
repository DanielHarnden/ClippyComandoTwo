using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public Transform bulletPrefab;
    private GameObject gunBarrel;

    [Header ("Pistol Stats")]
    // Default stats
    private int totalAmmo = 56;
    private int clipAmmo = 8;
    private int clipSize = 8;
    private float shootCooldown = 0.5f;
    private int damage = 1;
    private int piercing = 1;
    private float reloadTime = 2f;
    // Max stats
    private int MAXtotalAmmo = 512;
    private int MAXclipSize = 32;
    private float MAXshootCooldown = 0.0f;
    private int MAXdamage = 4;
    private int MAXpiercing = 4;
    private float MAXreloadTime = 0.25f;
    // Constant stats
    private float bulletSpeed = 1500f;
    // Temp stats
    private float shootTimer;
    private float reloadTimer;
    private bool canShoot = true;
    private bool reloading = false;

    [Header ("Audio Visual Effects")]
    //private Text ammoUI;
    public AudioClip emptyClipAudio;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

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
        reloading = false;
        gunBarrel = GameObject.FindGameObjectWithTag("GunTip");
    }

    void Update()
    {
        AimGun();

        // Updates ammo UI
        //ammoUI.text = clipAmmo.ToString() + "/" + totalAmmo.ToString();

        // Shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
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

        Reload();
    }



    public void Reload()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && clipAmmo < (clipSize / 2))
        {
            if (!reloading)
            {
                reloading = true;
                reloadTimer = reloadTime;
            }
        }

        if (reloading)
        {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = reloadAudio;
                thisAudio.Play();
            }

            if (reloadTimer > 0f)
            {
                reloadTimer -= Time.deltaTime;
            } else {
                reloading = false;
                // Doesn't reload if remaining ammo is less than 0.
                if (totalAmmo - (clipSize - clipAmmo) >= 0)
                {
                    totalAmmo -= (clipSize - clipAmmo);
                    clipAmmo = clipSize;
                } else {
                    clipAmmo += totalAmmo;
                    totalAmmo = 0;
                }
            }
        }
    }



    public void ShootCheck()
    {
        if (clipAmmo > 0)
        {
            if (canShoot)
            {
                if (reloading)
                {
                    reloading = false;
                }

                thisAudio.clip = shootAudio;
                thisAudio.Play();
                canShoot = false;
                shootTimer = shootCooldown;
                Shoot();
            }
        } else if (!reloading) {
            thisAudio.clip = emptyClipAudio;
            thisAudio.Play();
        }
    }

    void Shoot()
    {
        thisParticles.Play();

        Rigidbody2D newBul = Instantiate(bulletPrefab, gunBarrel.transform.position,   this.transform.rotation * Quaternion.Euler(0f, 0f, -90f)).GetComponent<Rigidbody2D>();

        newBul.GetComponent<Bullet>().InstantiateStats(2, 5, damage, piercing, false);

        newBul.AddForce(gunBarrel.transform.right * bulletSpeed);
        clipAmmo -= 1;
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
                FlipY(gunBarrel.transform, gunBarrel.transform.localPosition.y);
            }
        } else {
            this.GetComponent<SpriteRenderer>().flipY = false;
            if (barrelFlipped)
            {
                barrelFlipped = false;
                FlipY(gunBarrel.transform, gunBarrel.transform.localPosition.y);
            }
        }
    }

    void FlipY(Transform inputtedTransform, float tempY)
    {
        inputtedTransform.localPosition = new Vector3(inputtedTransform.localPosition.x, tempY *= -1f, inputtedTransform.localPosition.z);
    }



    public void InstantiateStats(int tempTotalAmmo, int tempClipAmmo, int tempClipSize, float tempShootCooldown, int tempDamage, int tempPiercing)
    {
        totalAmmo = tempTotalAmmo;
        clipAmmo = tempClipAmmo;
        clipSize = tempClipSize;
        shootCooldown = tempShootCooldown;
        damage = tempDamage;
        piercing = tempPiercing;
    }

    public float[] SaveStats()
    {
        float[] stats = {(float)totalAmmo, (float)clipAmmo, (float)clipSize, shootCooldown, (float)damage, (float)piercing};
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

    public bool AddClipSize(int clipSizeAdded)
    {
        int tempClipSize = clipSize + clipSizeAdded;

        if (clipSize == MAXclipSize)
        {
            return false;
        }
        else if (tempClipSize > MAXclipSize)
        {
            clipSize = MAXclipSize;
            return true;
        } 
        else {
            clipSize += clipSizeAdded;
            return true;
        }
    }

    public bool ReduceCooldown(float cooldownReduced)
    {
        float tempShootCooldown = shootCooldown - cooldownReduced;

        if (shootCooldown <= MAXshootCooldown)
        {
            return false;
        }
        else if (tempShootCooldown <= MAXshootCooldown)
        {
            shootCooldown = MAXshootCooldown;
            return true;
        } 
        else {
            shootCooldown -= cooldownReduced;
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

    public bool ReduceReload(float reloadReduced)
    {
        float tempReloadCooldown = reloadTime - reloadReduced;

        if (reloadTime <= MAXreloadTime)
        {
            return false;
        }
        else if (tempReloadCooldown <= MAXreloadTime)
        {
            reloadTime = MAXreloadTime;
            return true;
        } 
        else {
            reloadTime -= reloadReduced;
            return true;
        }
    }
}