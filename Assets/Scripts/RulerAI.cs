using UnityEngine;
using Pathfinding;

/********************************************************************************************************
========================================== Organization of Code =========================================

Variable Instantiation:
    Stats from Enemy.cs - Local duplicates of stats from Enemy.cs
    Movement and Player - Variables relating to A* movement and player locating
    Audio and Visual Effects - Variables pertaining to sprites and audio

Unity Event Functions:
    Start - Gets components
    FixedUpdate - Updates player locating and changes AI based on Ruler sharpness

Stat Instantiation and Updating:
    UpdateStats - Called from Enemy.cs, sets local stats
    UpdateSharpness - Changes sprite based on new sharpness level
    Damage - Plays audio and particles when damaged (called from Enemy.cs)
    Enrage - Has a chance to become more powerful upon taking damage (changes sprite)

Ruler Movement Part I:
    AimRuler - Points towards the player
    IdleRuler - Points towards the idle point chosen in FixedUpdate

Ruler Movement Part II:
    LevelZero - Lowest level yellow AI
    LevelOne 
    LevelTwo 
    LevelThree 
    LevelFour 
    LevelFive 
    LevelSix - Highest level yellow AI

    LevelZeroRage - Lowest level red AI (Yellow Level4)
    LevelOneRage 
    LevelTwoRage 
    LevelThreeRage 
    LevelFourRage 
    LevelFiveRage 
    LevelSixRage - Highest level red AI

********************************************************************************************************/

public class RulerAI : MonoBehaviour
{
    [Header ("Stats from Enemy.cs")]
    private Enemy enemyStats;
    private float moveSpeed;
    private bool playerSeen;
    private bool playerInRange;
    private float idleTime;
    private float idleTimer;
    private float chargePrepTime;
    private float chargePrepTimer;
    private float chargeTime;
    private float chargeTimer;
    private int sharpness;
    private bool charging;
    private bool enraged;



    [Header ("Movement and Player Location")]
    public AIPath aiPath;               // A* Pathfinding for Unity https://arongranberg.com/astar/
    private Rigidbody2D rulerRigidbody; // Rigidbody used for movement
    private Transform playerTransform;  // Used in AimRuler() to determine the player's position
    private Vector2 playerPos;          
    private Vector2 rulerPos;    
    private Vector2 idlePos;  
    private Vector2 tempPos; 



    [Header ("Audio Visual Effects")]
    public Sprite[] sharpnessLevelSprites;
    public Sprite[] rulerBits;

    public AudioClip damageAudio;
    public AudioClip chargeAudio;
    public AudioClip enrageAudio;

    private AudioSource thisAudio;
    private ParticleSystem thisParticles;
    


/******************************************************************************************************************************************************************************************************************/



    void Start()
    {
        enemyStats = this.gameObject.GetComponent<Enemy>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rulerRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        thisAudio = this.gameObject.GetComponent<AudioSource>();
        thisParticles = this.gameObject.GetComponent<ParticleSystem>();
        aiPath.repathRate = Random.Range(0.01f, 1.5f);
    }



    void FixedUpdate() 
    {
        playerSeen = enemyStats.playerSeen;

        // Uses a raycast to determine if the player is in range or not
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 8f);

        if (hit.collider != null && hit.transform.tag == "Player")
        {
            playerInRange = true;
        } else {
            playerInRange = false;
        }

        // Yellow AI
        if (playerSeen && !enraged)
        {
            switch(sharpness)
            {
                case 0:
                    LevelZero();
                    break;

                case 1:
                    LevelOne();
                    break;

                case 2:
                    LevelTwo();
                    break;

                case 3:
                    LevelThree();
                    break;

                case 4:
                    LevelFour();
                    break;

                case 5:
                    LevelFive();
                    break;

                case 6:
                    LevelSix();
                    break;

                default:
                    Debug.Log("No sharpness assigned");
                    break;
            } 
        // Red AI
        } else if (playerSeen && enraged) {
            switch(sharpness)
            {
                case 0:
                    LevelZeroRage();
                    break;

                case 1:
                    LevelOneRage();
                    break;

                case 2:
                    LevelTwoRage();
                    break;

                case 3:
                    LevelThreeRage();
                    break;

                case 4:
                    LevelFourRage();
                    break;

                case 5:
                    LevelFiveRage();
                    break;

                case 6:
                    LevelSixRage();
                    break;

                default:
                    Debug.Log("No sharpness assigned");
                    break;
            } 
        // Idle AI
        } else {
            aiPath.enabled = false;

            // Randomly selects a point to move to within 10 units every idleTimer seconds
            if (idleTimer > 0)
            {
                idleTimer -= Time.deltaTime;
            } else {
                idlePos = this.transform.position + new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0f);
                idleTimer = idleTime;   
            }

            IdleRuler();
        }
    }



/******************************************************************************************************************************************************************************************************************/


    // This function may be overloaded in the future
    public void UpdateStats(float tempMoveSpeed, float tempIdleTime, float tempChargePrep, float tempChargeTime, int tempSharpness)
    {
        moveSpeed = tempMoveSpeed;
        aiPath.maxSpeed = moveSpeed;
        idleTime = tempIdleTime;
        chargePrepTime = tempChargePrep;
        chargePrepTimer = chargePrepTime;
        chargeTime = tempChargeTime;
        chargeTimer = chargeTime;
        UpdateSharpness(tempSharpness);
    }



    public void UpdateSharpness(int tempSharpness)
    {
        sharpness = tempSharpness;

        // Changes sprite depending on sharpness
        switch (tempSharpness)
        {
            case 0:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[0];
                break;

            case 1:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[1];
                break;

            case 2:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[2];
                break;

            case 3:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[3];
                break;

            case 4:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[4];
                break;

            case 5:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[5];
                break;

            case 6:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[6];
                break;

            default:
                Debug.Log("That's not a valid sharpness value");
                break;
        }
    }



    public void Damage()
    {
        if (Random.Range(0, 255) == 69)
        {
            Enrage();
        }

        thisAudio.clip = damageAudio;
        thisAudio.Play();
    }



    public void Enrage()
    {
        thisAudio.clip = enrageAudio;
        thisAudio.Play();
    
        // Increases stats (damage is in Enemy.cs)
        enraged = true;
        this.gameObject.GetComponent<Enemy>().damage *= 2;
        moveSpeed *= 1.5f;
        aiPath.maxSpeed = moveSpeed;
        chargeTime *= 1.5f;
        chargePrepTime /= 2;

        // Changes to red ruler sprite
        switch (sharpness)
        {
            case 0:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[7];
                break;

            case 1:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[8];
                break;

            case 2:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[9];
                break;

            case 3:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[10];
                break;

            case 4:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[11];
                break;

            case 5:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[12];
                break;

            case 6:
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sharpnessLevelSprites[13];
                break;

            default:
                Debug.Log("That's not a valid sharpness value");
                break;
        }
    }



/******************************************************************************************************************************************************************************************************************/



    // Default speed 750
    void AimRuler(float speed)
    {
        // Compares player and enemy positions, stores in playerPos
        playerPos = playerTransform.position;
        rulerPos = this.transform.position;
        playerPos.x = playerPos.x - rulerPos.x;
        playerPos.y = playerPos.y - rulerPos.y;

        // Determines the angle needed to rotate
        float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

        // Angle - 90 due to sprite being vertical and rotate towards pointing right, not up
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * speed);
    }



    void IdleRuler()
    {
        // Compares ruler and idle positions, stores in tempPos
        rulerPos = this.transform.position;
        tempPos.x = idlePos.x - rulerPos.x;
        tempPos.y = idlePos.y - rulerPos.y;

        // Determines angle and rotates
        float angle = Mathf.Atan2(tempPos.y, tempPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * 750f);
        rulerRigidbody.MovePosition(rulerRigidbody.position + tempPos * (moveSpeed * 0.05f) * Time.fixedDeltaTime);
    }



/******************************************************************************************************************************************************************************************************************/
    


    // Only aims at the player and moves towards them using simple "point and go" movement. Speed is lower when closer to simulate caution
    void LevelZero()
    {
        AimRuler(50f);

        if (playerInRange)
        {
            Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y);
            rulerRigidbody.MovePosition(newPos + playerPos * (0.05f * moveSpeed) * Time.fixedDeltaTime);
        } else {
            Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y);
            rulerRigidbody.MovePosition(newPos + playerPos * (0.1f * moveSpeed) * Time.fixedDeltaTime);
        }
    }

    // Aims at the player and moves towards them, using A* when farther away.
    void LevelOne()
    {
        AimRuler(250f);

        if (playerInRange)
        {
            aiPath.enabled = false;
            Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y);
            rulerRigidbody.MovePosition(newPos + playerPos * (0.05f * moveSpeed) * Time.fixedDeltaTime);
        } else {
            aiPath.enabled = true;
        }
    }

    // Slower than the base difficulty and can now charge, charge resets when the player leaves charge vicinity
    void LevelTwo()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(500f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.1f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }
            } else {
                chargePrepTimer = chargePrepTime;
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
            
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                charging = false;
                chargeTimer = chargeTime;
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 2f) * Time.fixedDeltaTime); 
        }
    }

    // Slower than base difficulty, charge does not reset when player leaves charge vicinity
    void LevelThree()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(600f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 250f, rulerRigidbody.position.y - this.transform.right.y / 250f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.1f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
            
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                charging = false;
                chargeTimer = chargeTime;
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 2f) * Time.fixedDeltaTime); 
        }
    }

    // Base difficulty
    void LevelFour()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(750f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 100f, rulerRigidbody.position.y - this.transform.right.y / 100f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.1f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
            
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                charging = false;
                chargeTimer = chargeTime;
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 4f) * Time.fixedDeltaTime); 
        }
    }

    int i = 0;
    // Faster than base difficulty, charges twice back-to-back
    void LevelFive()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(1000f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 25f, rulerRigidbody.position.y - this.transform.right.y / 25f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 2)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 4.5f) * Time.fixedDeltaTime); 
        }
    }


    // Faster than base difficulty, charges thrice back-to-back
    void LevelSix()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(1500f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 10f, rulerRigidbody.position.y - this.transform.right.y / 10f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 3)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 5f) * Time.fixedDeltaTime); 
        }
    }

    // Base difficulty
    void LevelZeroRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(750f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 100f, rulerRigidbody.position.y - this.transform.right.y / 100f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.1f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
            
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                charging = false;
                chargeTimer = chargeTime;
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 4f) * Time.fixedDeltaTime); 
        }
    }

    // Faster than base difficulty, charges twice back-to-back
    void LevelOneRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(1000f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 25f, rulerRigidbody.position.y - this.transform.right.y / 25f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 2)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 4.5f) * Time.fixedDeltaTime); 
        }
    }


    // Faster than base difficulty, charges thrice back-to-back
    void LevelTwoRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(1500f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 10f, rulerRigidbody.position.y - this.transform.right.y / 10f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 3)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 5f) * Time.fixedDeltaTime); 
        }
    }

    // Faster than base difficulty, charges fource back-to-back
    void LevelThreeRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(2000f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 10f, rulerRigidbody.position.y - this.transform.right.y / 10f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 4)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 5.5f) * Time.fixedDeltaTime); 
        }
    }

    // Faster than base difficulty, charges fivece back-to-back
    void LevelFourRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(2000f);

            if (playerInRange)
            {
                aiPath.enabled = false;

                Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 10f, rulerRigidbody.position.y - this.transform.right.y / 10f);
                rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                aiPath.enabled = true;
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 5)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 6f) * Time.fixedDeltaTime); 
        }
    }

    // Faster than base difficulty, charges fivece back-to-back
    private bool swap;
    void LevelFiveRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(2000f);

            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                idleTimer = 1f;
                swap = !swap;
            }

            if (playerInRange)
            {
                aiPath.enabled = false;

                if (swap)
                {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y - this.transform.right.y / 50f);
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);
                } else {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y + this.transform.right.y / 10f);
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);
                }
                

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                if (swap)
                {
                    aiPath.enabled = true;
                } else {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 5f, rulerRigidbody.position.y - this.transform.right.y / 5f);
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.5f * moveSpeed) * Time.fixedDeltaTime);
                }
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 5)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 6.5f) * Time.fixedDeltaTime); 
        }
    }

    // Faster than base difficulty, charges fivece back-to-back
    void LevelSixRage()
    {
        if (!charging)
        {
            rulerRigidbody.freezeRotation = false;
            AimRuler(2000f);

            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                idleTimer = 1f;
                swap = !swap;
            }

            if (playerInRange)
            {
                aiPath.enabled = false;

                if (swap)
                {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y - this.transform.right.y / Random.Range(10f, 100f));
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);
                } else {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x, rulerRigidbody.position.y + this.transform.right.y / Random.Range(10f, 100f));
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.2f * moveSpeed) * Time.fixedDeltaTime);
                }
                

                chargePrepTimer -= Time.deltaTime;
                if (chargePrepTimer <= 0f)
                {
                    charging = true;
                    chargePrepTimer = chargePrepTime;
                }

            } else {
                if (swap)
                {
                    aiPath.enabled = true;
                } else {
                    Vector2 newPos = new Vector2 (rulerRigidbody.position.x - this.transform.up.x / 5f, rulerRigidbody.position.y - this.transform.right.y / 5f);
                    rulerRigidbody.MovePosition(newPos + playerPos * (0.5f * moveSpeed) * Time.fixedDeltaTime);
                }
            }
        } else {
            if (!thisAudio.isPlaying)
            {
                thisAudio.clip = chargeAudio;
                thisAudio.Play();
            }

            rulerRigidbody.freezeRotation = true;
        
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                playerPos = playerTransform.position;
                rulerPos = this.transform.position;

                playerPos.x = playerPos.x - rulerPos.x;
                playerPos.y = playerPos.y - rulerPos.y;

                float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                chargeTimer = chargeTime;
                i++;

                if (i == 5)
                {   
                    charging = false;
                    chargeTimer = chargeTime;
                    i = 0;
                }
            }

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 7f) * Time.fixedDeltaTime); 
        }
    }
}