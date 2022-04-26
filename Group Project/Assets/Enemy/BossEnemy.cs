using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;


// CHECKLIST:
// --> BIG SCARY ENEMY
// --> ANIMATED
// --> SOUNDS
// --> (Multiple? ABILITIES)
//     --> THROW ROCK
// --> CHASE BEHAVIOUR (Reuse elite movement).
// --> COLLISIONS WITH
//     --> PLAYER
//     --> ENVIRONMENT
//     --> IT'S OWN PROJECTILE
// --> OTHER COMPLICATIONS THAT WILL TRIP ME UP
public class BossEnemy : MonoBehaviour
{
    [SerializeField] private int bossLevel;

    // Boss abilities are predefined and cannot be changed.
    private List<Ability> bossAbilities = new List<Ability>();
    public int BOSS_ABILITY_COUNT;

    // Boss drops
    private int expOnDeath;
    private int goldOnDeath;

    // Generic monster stats: damage health and movement speed.
    private int contactDamage;
    private int health;
    private int moveSpeed;


    // Movement script, will be set to aggressive.
    private Movement EliteMovement;

    // Used only for the projectile ability;
    private bool timedAbilty = false;

    // Keep track of the position of the player. May be integrated into an enemy manager class for efficiency later on, but this'll do.
    private Vector2 playerPosition;

    // Boolean to determine which way the enemy is facing.
    private bool facingRight = false;
    // Determines if enemy is touching the floor, used to determine if it is able to jump.
    private bool onGround;


    // Rigidbody component of the enemy
    private Rigidbody2D rb;

    // Enemy width * 0.5 (Used to set the start position for the ray cast for obstacle detection).
    private float enemyHalfWidth;

    // Start is called before the first frame update
    void Start()
    {
        // Init the facing direction of the enemy.
        facingRight = true;

        // Setup boss stats
        // Being the boss, it will have incredibly high stats as he is a dangerous foe.
        contactDamage = 8 + bossLevel * 6;
        health = bossLevel * 50;

        expOnDeath = 500 + bossLevel * 500;
        goldOnDeath = bossLevel * 1200;

        // Moves slower than other enemies to make the fight more fair for the player.
        moveSpeed = 2;

        // Get rigidbody component and calculate + store the half-width of the enemy.
        rb = gameObject.GetComponent<Rigidbody2D>();

        enemyHalfWidth = gameObject.GetComponent<BoxCollider2D>().bounds.size.x * 0.5f;

        // Setup abilities and init
        EliteMovement = gameObject.AddComponent<AggressiveMovement>();
        EliteMovement.Init(moveSpeed);
        gameObject.AddComponent<DashAbility>();
       // bossAbilities.Add(gameObject.AddComponent<DashAbility>());
        bossAbilities.Add(gameObject.AddComponent<BashAbility>());
        foreach (Ability a in bossAbilities) { a.Init(bossLevel);}
    }

    // Update is called once per frame
    void Update()
    {
        // Get player position
        playerPosition = GameObject.Find("Player").transform.position;



        // Update enemy's facing direction in relation to the player.
        facingRight = EliteMovement.FaceTowardsPlayer(transform.position, playerPosition);

        // Move the enemy.
        EliteMovement.Move(gameObject, facingRight);

        // Enemy will jump when 3 conditions are satisfied:
        // - He is on the ground (onGround == true)
        // - The player is close enough to the enemy (Horizontal distance between enemy and player <= 5 units).
        // - The player is at least 2 units above the enemy (Player is high enough that the enemy will consider jumping to inflict contact damage).
        if (Mathf.Abs(transform.position.x - playerPosition.x) <= 5 && onGround &&
            transform.position.y < playerPosition.y - 2)
        {
            EliteMovement.Jump(gameObject);
            onGround = false;
        }


        // Second check for jumping: If the enemy needs to jump over an obstacle.
        // ANOMALIES:
        //   - This is not pathfinding. The enemy can and will get trapped inside structures unless intentionally lured out.
        //   - Because of the function that checks if the enemy is able to jump again, the enemy may sometimes jump multiple times to jump over the obstacle, resulting in a huge
        // burst of vertical speed.
        RaycastHit2D hitCheck = Physics2D.Raycast((Vector2)transform.position + new Vector2(enemyHalfWidth, 0),
            (facingRight) ? Vector2.right : Vector2.left,
            0.8f);

        // Enemy will jump over obstacles when 3 conditions are satisfied:
        // -  If the collider is not null (This is more done due to a nullReferenceException error when trying to access a non-existent collider).
        // - The collider has identified the tag as "Floor".
        // - Enemy is currently on the ground.
        {
            if (hitCheck.collider && hitCheck.collider.gameObject.tag == "Floor" && onGround)
            {
                EliteMovement.Jump(gameObject);
                onGround = false;
            }
        }

        for (int i = 0; i < BOSS_ABILITY_COUNT; i++)
        {

            // Call the ability check function of the ability. If it returns true, the ability can be used, otherwise it won't be used, even if the cooldown has expired.
            if (bossAbilities[i].cooldown < 0.0 && bossAbilities[i].CheckAbilityUsage(transform.position, playerPosition))
            {
                // Access the enemy's ability component and call it's attack function (which runs on a cooldown-based system)
                bossAbilities[i].UseAbility(gameObject, facingRight);
            }

            // Decrement the ability cooldown by dt.
            bossAbilities[i].cooldown -= Time.deltaTime;

            // If the elite ability relies on a after-use timer, decrement it by dt.
            // Once this timer hits 0, run another function to finish/despawn the attack.
            if (timedAbilty)
            {
                bossAbilities[i].duration -= Time.deltaTime;

                if (bossAbilities[i].duration <= 0.0f)
                {
                    bossAbilities[i].StopAbility(gameObject);
                }
            }

        }
        // Custom implementation of gravity. This is due to the enemy's linear drag being set to 5 to offset the incredible speed burst from dash attacks.
        // Linear drag massively slows down the acceleration of the enemy while falling, so this counters this slowdown for more realistic falling.
        rb.AddForce(new Vector2(0, -9.81f));
        }

    /// <summary>
    /// Handle all collisions, called when two colliders touch.
    /// </summary>
    /// <param name="col"> The --Other-- collider that touched the collider on this object.</param>
    public void OnCollisionEnter2D(Collision2D col)
    {
        // col: other object
        // col.otherCollider: this object

        // Enemy itself hits the player
        if (col.gameObject.tag == "Player")
        {
            Vector2 normalizedHitDirection = (col.gameObject.transform.position - col.otherCollider.gameObject.transform.position).normalized;
            normalizedHitDirection.y = 0.4f;

            col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(3 * normalizedHitDirection, ForceMode2D.Impulse);
        }

        // Collision with any object labelled "Floor" (Which should be the tag set for all walkable surfaces in the game): allow enemy to jump again if he is walking on the floor
        // ANOMALIES:
        //   - Enemy could "wall climb" by repeatedly running into a wall and jumping. (Can be fixed by also testing with a raycast, but it's a minor (and entertaining) bug.
        //   - Enemy does not lose onGround status when moving off a floating/tall platform, allowing it to jump in midair once.
        else if (col.gameObject.tag == "Floor" &&
                 col.gameObject.transform.position.y < gameObject.transform.position.y)
        {
            onGround = true;
        }
    }
}