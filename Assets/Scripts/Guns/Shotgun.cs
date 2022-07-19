using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Transform bulletPrefab;
    private GameObject gunBarrel;

    [Header ("Shotgun Stats")]
    // Default stats
    private int totalAmmo = 28;
    private int clipAmmo = 4;
    private int clipSize = 4;
    private float shootCooldown = 1.5f;
    private int damage = 1;
    private int piercing = 1;
    private float spreadFactor = 0.5f;
    private int pelletCount = 4;
    // Max stats
    private int MAXtotalAmmo = 512;
    private int MAXclipSize = 32;
    private float MAXshootCooldown = 0.0f;
    private int MAXdamage = 4;
    private int MAXpiercing = 4;
    private float MAXspreadFactor = 0.05f;
    private int MAXpelletCount = 8;
    // Constant stats
    private float bulletSpeed = 2000f;
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

        // Reload
        if (Input.GetKeyDown(KeyCode.Mouse1) && clipAmmo < (clipSize / 2))
        {
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



    public void ShootCheck()
    {
        if (clipAmmo > 0)
        {
            if (canShoot)
            {
                //thisGun.Play();
                canShoot = false;
                shootTimer = shootCooldown;
                Shoot();
            }
        } else {
            //emptyClip.Play();
        }
    }

    void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            // Adds a random spread to the gun barrel location
            Vector3 spreadPos = gunBarrel.transform.right;
            spreadPos.x += Random.Range(-spreadFactor, spreadFactor);
            spreadPos.y += Random.Range(-spreadFactor, spreadFactor);
            spreadPos.z = 0;

            Rigidbody2D newBul = Instantiate(bulletPrefab, gunBarrel.transform.position,   this.transform.rotation * Quaternion.Euler(0f, 0f, -90f)).GetComponent<Rigidbody2D>();

            newBul.GetComponent<Bullet>().InstantiateStats(4, 5, damage, piercing, false);

            newBul.AddForce(spreadPos * bulletSpeed);
        }
        
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
                gunBarrel.transform.localPosition = new Vector3 (0.15f, -0.045f, 0f);
            }
        } else {
            this.GetComponent<SpriteRenderer>().flipY = false;
            if (barrelFlipped)
            {
                barrelFlipped = false;
                gunBarrel.transform.localPosition = new Vector3 (0.15f, 0.045f, 0f);
            }
        }
    }



    public void InstantiateStats(int tempTotalAmmo, int tempClipAmmo, int tempClipSize, float tempShootCooldown, int tempDamage, int tempPiercing, float tempSpreadFactor, int tempPelletCount)
    {
        totalAmmo = tempTotalAmmo;
        clipAmmo = tempClipAmmo;
        clipSize = tempClipSize;
        shootCooldown = tempShootCooldown;
        damage = tempDamage;
        piercing = tempPiercing;
        spreadFactor = tempSpreadFactor;
        pelletCount = tempPelletCount;
    }

    public float[] SaveStats()
    {
        float[] stats = {(float)totalAmmo, (float)clipAmmo, (float)clipSize, shootCooldown, (float)damage, (float)piercing, spreadFactor, (float)pelletCount};
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

    public bool ReduceSpread(int spreadReduced)
    {
        float tempSpread = spreadFactor + spreadReduced;

        if (spreadFactor <= MAXspreadFactor)
        {
            return false;
        }
        else if (tempSpread < MAXspreadFactor)
        {
            spreadFactor = MAXspreadFactor;
            return true;
        } 
        else {
            spreadFactor -= spreadReduced;
            return true;
        }
    }

    public bool IncreasePellet(int pelltsAdded)
    {
        float tempPellets = pelletCount + pelltsAdded;

        if (pelletCount == MAXpelletCount)
        {
            return false;
        }
        else if (tempPellets > MAXpelletCount)
        {
            pelletCount = MAXpelletCount;
            return true;
        } 
        else {
            pelletCount += pelltsAdded;
            return true;
        }
    }
}
