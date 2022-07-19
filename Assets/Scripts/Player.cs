using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{      
    [Header ("Player Stats")]
    /* 
        0: Health
        1: Base Move Speed
        2: Sprint Move Speed
        3: Sprint Time
        4: Sprint Cooldown
        5: Dodge Time
        6: Dodge Cooldown
    */
    private float[] clippyBaseStats = {100f, 7f, 14f, 2f, 2f, 0.1f, 2f};
    private float[] clippyMaxStats = {1f};

    private float[] currentStats = {100f, 10f, 17f, 2f, 2f, 0.25f, 2f};
    public int enemiesLeft;

    private float currentMoveSpeed;

    private float sprintTimeRemaining;
    private float sprintCooldownRemaining;

    public bool dodging;
    private float dodgeTimeRemaining;   
    private float dodgeCooldownRemaining;

    private int gluePuddle;

    private Rigidbody2D playerRigidbody;
    private Vector2 moveVector;
    


    [Header ("Weapons")]
    public Transform[] weaponPrefabs;

    private int scrollWheelWeaponInt;



    [Header ("Audio Visual Effects")]
    private AudioSource playerAudio;
    private Animation playerAnimation;



    private void Start() 
    {
        playerRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        playerAudio = this.GetComponent<AudioSource>();

        // TODO: Ensure that each character's animations are their own
        playerAnimation = this.gameObject.transform.GetChild(0).GetComponent<Animation>();

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            if (i != 0)
            {
                weaponPrefabs[i].gameObject.SetActive(false);
            } else {
                weaponPrefabs[i].gameObject.SetActive(true);
            }
        }
    }  

    // Use FixedUpdate to handle physics calculations
    private void FixedUpdate() 
    {
        if (!dodging)
        {
            // TODO: Change layermask to an actual layer lmao
            // this.gameObject.layer = LayerMask.NameToLayer("EnemyInteractable");

            for (int i = 0; i < gluePuddle; i++)
            {
                // TODO: Ensure glue puddle is correctly set up in this script
                currentMoveSpeed /= 2f;
            }
        }

        playerRigidbody.MovePosition(playerRigidbody.position + moveVector * currentMoveSpeed * Time.fixedDeltaTime);
    } 

    // Use Update to handle user input
    void Update() 
    {
        // Switch weapon using scroll wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            //scrollWheelWeaponInt keeps track of the current weapon (even when switching using alphanumeric number keys)
            scrollWheelWeaponInt = (scrollWheelWeaponInt += (int)Input.mouseScrollDelta.y) % weaponPrefabs.Length;
            SwitchGun(scrollWheelWeaponInt);
        }

        // Switch weapon using alphanumeric number keys
        SwitchGunAlphaNumeric();

        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        Sprint();
        Dodge();

        AnimateClippy();
    }

    // TODO: Make this work for each character
    void AnimateClippy()
    {
        if (moveVector == Vector2.zero)
        {
            if ((playerAnimation["HatBounce"].normalizedTime >= 0.495f || playerAnimation["HatBounce"].normalizedTime <= 0.505f) || (playerAnimation["HatBounce"].normalizedTime >= 0.995f || playerAnimation["HatBounce"].normalizedTime <= 0.005f))
            {
                playerAnimation["HatBounce"].normalizedSpeed = 0f;
            }
        } else {

            if (Input.GetKey(KeyCode.LeftShift) && sprintTimeRemaining > 0)
            {
                playerAnimation["HatBounce"].normalizedSpeed = 2f;
            } else {
                playerAnimation["HatBounce"].normalizedSpeed = 1f;
            }
        }   
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && sprintTimeRemaining > 0)
        {
            sprintTimeRemaining -= Time.deltaTime;
            
            currentMoveSpeed = currentStats[2];
            sprintCooldownRemaining = currentStats[4];
        } else {
            currentMoveSpeed = currentStats[1];

            if (sprintCooldownRemaining > 0)
            {
                sprintCooldownRemaining -= Time.deltaTime;
            } else {
                if (sprintTimeRemaining < currentStats[3])
                {
                    sprintTimeRemaining += Time.deltaTime;
                }
            }
        }
    }

    void Dodge()
    {
        if (dodgeCooldownRemaining > 0)
        {
            dodgeCooldownRemaining -= Time.deltaTime;
        } else {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dodging = true;
                dodgeCooldownRemaining = currentStats[6];
                dodgeTimeRemaining = currentStats[5];
            }
        }

        // TODO: Change layermask to an actual layer lmao
        // this.gameObject.layer = LayerMask.NameToLayer("Water");
        if (dodgeTimeRemaining > 0)
        {
            dodgeTimeRemaining -= Time.deltaTime;
            currentMoveSpeed = -Mathf.Pow((dodgeTimeRemaining - currentStats[5])/(currentStats[5] * 0.25f), 2f) + (3 * currentStats[2]);
        } else {
            dodging = false;
        }
    }



    // TODO: Probably have to update this for the rest of the weapons in the game.
    void SwitchGunAlphaNumeric()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchGun(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchGun(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchGun(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchGun(3);
        }
    }

    // Switches the current weapon
    public void SwitchGun(int weaponIndex)
    {
        // Turns off all weapons before transitioning to the new one.
        foreach(Transform weapon in weaponPrefabs)
        {
            weapon.gameObject.SetActive(false);
        }

        if (weaponIndex < 0)
        {
            // Accounts for scroll wheel being negative
            weaponPrefabs[weaponPrefabs.Length + weaponIndex].gameObject.SetActive(true);
        } else {
            weaponPrefabs[weaponIndex].gameObject.SetActive(true);
        }

        scrollWheelWeaponInt = weaponIndex;
    }



    private void OnCollisionEnter2D(Collision2D colObj) 
    {
        if (!dodging)
        {
            /*switch (colObj.gameObject.tag)
            {
                case "Enemy":
                    health -= (int)colObj.gameObject.GetComponent<Enemy>().damage;
                    playerAudio.Play();

                case "Staple":
                    playerAudio.Play();
                    break;

                default:
                    break;
            }*/
        }
    }



    public void Damaged(int damageInflicted)
    {
        currentStats[0] -= damageInflicted;
    }
}