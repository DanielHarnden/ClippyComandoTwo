using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Red = 1, Orange = 2, Yellow = 3, Green = 4, Teal = 5, Blue = 6
    private int ammoType = 2;
    private float deathTimer = 5f;
    private int damage = 1;
    private int piercing = 1;
    private bool die = false;
    private bool wallStick = false;
    private bool wallStuck = false;



    public void InstantiateStats(int tempAmmoType, float tempDeathTimer, int tempDamage, int tempPiercing, bool tempWallStick)
    {
        ammoType = tempAmmoType;
        deathTimer = tempDeathTimer;
        damage = tempDamage;
        piercing = tempPiercing;
        wallStick = tempWallStick;
    }

    // Timer to delete old bullets
    private void Update() 
    {
        if (deathTimer > 0f)
        {
            deathTimer -= Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D colObj) 
    {
        // Switch statement since each enemy has a different script.
        switch (colObj.gameObject.tag)
        {
            case "Wall":
                if (wallStick)
                {
                    wallStuck = true;
                    this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                } else {
                    die = true;
                }
                break;
                
            case "Enemy":
                colObj.gameObject.GetComponent<Enemy>().Damaged(damage);
                piercing -= 1;
                break;

            case "Player":
                if (wallStuck)
                {
                    // TODO: add pickup 
                }
                die = true;
                break;

            default:
                break;
        }

        if (piercing <= 0)
        {
            die = true;
        }

        if (die)
        {
            Destroy(this.gameObject);
        }
    }
}
