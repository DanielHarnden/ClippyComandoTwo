using UnityEngine;
using Pathfinding;

public class RulerAI : MonoBehaviour
{
    [Header ("Stats from Enemy.cs")]
    private Enemy enemyStats;
    private float health;                  
    private float damage;
    private float moveSpeed;
    private bool playerSeen;
    private float idleTime;
    private float chargePrepTime;
    private float chargePrepTimer;
    private float chargeTime;
    private float chargeTimer;
    private int sharpness;
    private bool charging;
    private bool dead;

    [Header ("Movement and Player Location")]
    public AIPath aiPath;               // A* Pathfinding for Unity https://arongranberg.com/astar/
    private Rigidbody2D rulerRigidbody; // Rigidbody used for movement

    private Transform playerTransform;  // Used in AimRuler() to determine the player's position
    private Vector2 playerPos;          
    private Vector2 rulerPos;    
    private Vector2 idlePos;  
    private Vector2 tempPos; 

    private bool playerInRange;
    private float idleTimer;

    public Sprite[] sharpnessLevelSprites;
    


    // Functions relating to initializing and updating stats
    void Start()
    {
        enemyStats = this.gameObject.GetComponent<Enemy>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rulerRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    public void UpdateStats(float tempHealth, float tempDamage, float tempMoveSpeed, float tempIdleTime, float tempChargePrep, float tempChargeTime, int tempSharpness)
    {
        health = tempHealth;
        damage = tempDamage;
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

    public void Enrage()
    {
        damage *= 2;
        moveSpeed *= 1.5f;
        aiPath.maxSpeed = moveSpeed;
        chargeTime *= 1.5f;
        chargePrepTime /= 2;


        // TODO: change sprite
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }



    // Ruler "looks" towards player (default speed 750)
    void AimRuler(float speed)
    {
        playerPos = playerTransform.position;
        rulerPos = this.transform.position;

        // Compares player and enemy positions, stores in playerPos
        playerPos.x = playerPos.x - rulerPos.x;
        playerPos.y = playerPos.y - rulerPos.y;

        // Determines the angle needed to rotate
        float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

        // Angle - 90 in Vector3 due to sprite being vertical and rotate towards pointing right, not up
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * speed);
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

        if (playerSeen)
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
        } else {
            aiPath.enabled = false;

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






    void IdleRuler()
    {
        rulerPos = this.transform.position;

        // Compares player and enemy positions, stores in playerPos
        tempPos.x = idlePos.x - rulerPos.x;
        tempPos.y = idlePos.y - rulerPos.y;

        // Determines the angle needed to rotate
        float angle = Mathf.Atan2(tempPos.y, tempPos.x) * Mathf.Rad2Deg;

        // Angle - 90 in Vector3 due to sprite being vertical and rotate towards pointing right, not up
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * 750f);

        rulerRigidbody.MovePosition(rulerRigidbody.position + tempPos * (moveSpeed * 0.05f) * Time.fixedDeltaTime);
    }







/********************************************************************************************************
AI Difficulty Levels
********************************************************************************************************/
    
    // Lowest difficulty (unsharpened)
    // Only aims at the player and moves towards them using bad movement. Speed is lower when closer to simulation caution
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

    // Slower than the base difficulty and can now charge, but charge resets when the player leaves charge vicinity
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
                chargePrepTime = chargePrepTime;
                aiPath.enabled = true;
            }
        } else {
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

    // Base difficulty (medium sharpness)
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

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 5f) * Time.fixedDeltaTime); 
        }
    }


    // Highest difficulty (most sharpened)
    int i = 0;
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

            rulerRigidbody.MovePosition(rulerRigidbody.position + new Vector2(this.transform.up.x, this.transform.up.y) * (moveSpeed * 7.5f) * Time.fixedDeltaTime); 
        }
    }
}