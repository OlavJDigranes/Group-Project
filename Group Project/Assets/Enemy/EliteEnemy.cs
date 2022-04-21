using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

//todo checklist:
// -> Abilities for enemies (such as projectiles, teleportation, summoning enemies, speed/power enhancements..).
// -> Enemy movement:
// ->   -> Collision with the platforms.
// ->   -> "AI" (Aggressive: chase the player, Defensive: Keep distance from the player).
// ->   -> Jumping
// -> Melee enemies (animated weapon attack or on-touch damage?)
// -> Ranged enemies
// -> Collision triggers with player attacks
// -> Damaging the player on collision (+ Separate damage when using their ability).

public class EliteEnemy : MonoBehaviour
{
    // Monster level, used to scale the monster's damage and health.
    [SerializeField] private int monsterLevel;

    // List of abilities elite enemies can have.
    public enum ABILITYNAME
    {
        // MELEE ABILITIES
        DASH, // Aggressive dash.
        BASH // Aggressive with a high-knockback attack.
    };

    // Retrieve ability selected for this enemy.
    [SerializeField] public ABILITYNAME abilityName;


// Monster drops
    private int expOnDeath;
    private int goldOnDeath;

    // Generic monster stats: damage health and movement speed.
    private int contactDamage;
    private int health;
    private int moveSpeed;


    // The enemy's ability. It is set initially as the ability abstract but will be derived to a specific ability in the start function.
    private Ability EliteAbility;

    // Movement script, used to dictate how the enemy moves in the game.
    private Movement EliteMovement;

    // Boolean that determines if the ability relies on a timer to function, used to, for example remove new hitboxes or to finish an attack.
    private bool timedAbilty = false;

    // Hitbox of the bash ability, set to null initially for cases where the enemy has a different ability, otherwise it will be set to the component.
    private BoxCollider2D bashHitBox = null;

    // Boolean that determines if an ability hit the player, used for certain abilities to ensure that it only interacts with the player
    // once per ability usage (to prevent multiple hits at once).
    private bool abilityHitPlayer = false;


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

    // Enemy does nothing when not in use. Once an enemy activates, it can never be de-activated again.
    private bool active = false;


    // Start is called before the first frame update
    void Start()
    {

        // Init the facing direction of the enemy.
        facingRight = true;

        // Setup monster movement speed and drops.
        // Being elite enemies, they naturally will have improved stats and drops compared to common enemies.
        contactDamage = 2 + monsterLevel * 6;
        health = monsterLevel * 15;

        expOnDeath = 50 + monsterLevel * 100;
        goldOnDeath = monsterLevel * 50;

        moveSpeed = 3;

        // Get rigidbody component and calculate + store the half-width of the enemy.
        rb = gameObject.GetComponent<Rigidbody2D>();

        enemyHalfWidth = gameObject.GetComponent<BoxCollider2D>().bounds.size.x * 0.5f;

        // Read the input ability name and setup the corresponding ability script.
        switch (abilityName)
        {
            // Dash
            case ABILITYNAME.DASH:
                EliteAbility = gameObject.AddComponent<DashAbility>();
                EliteMovement = gameObject.AddComponent<AggressiveMovement>();
                break;

            // Bash
            case ABILITYNAME.BASH:
                EliteAbility = gameObject.AddComponent<DashAbility>();
                EliteMovement = gameObject.AddComponent<AggressiveMovement>();
                break;
        }

        // Call init functions for movement and ability.
        EliteAbility.Init(monsterLevel);
        EliteMovement.Init(moveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // Get player position
        playerPosition = GameObject.Find("Player").transform.position;

        // If asleep, check if player moved close enough to the enemy to activate/wake up the enemy.
        // Enemy will begin to attack the player when the player is around 35 metres away from it.
        if (!active && Vector2.Distance(playerPosition, (Vector2)transform.position) < 35) { active = true; }

        // Enemy is awake, and so it's update function will run.
        if(active)
        {

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
            RaycastHit2D hitCheck = Physics2D.Raycast((Vector2) transform.position + new Vector2(enemyHalfWidth, 0),
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

            // Call the ability check function of the ability. If it returns true, the ability can be used, otherwise it won't be used, even if the cooldown has expired.
            if (EliteAbility.CheckAbilityUsage(transform.position, playerPosition, EliteAbility.cooldown))
            {
                // Access the enemy's ability component and call it's attack function (which runs on a cooldown-based system)
                EliteAbility.UseAbility(gameObject, facingRight);
            }

            // Decrement the ability cooldown by dt.
            EliteAbility.cooldown -= Time.deltaTime;

            // If the elite ability relies on a after-use timer, decrement it by dt.
            // Once this timer hits 0, run another function to finish/despawn the attack.
            if (timedAbilty)
            {
                EliteAbility.duration -= Time.deltaTime;

                if (EliteAbility.duration <= 0.0f)
                {
                    EliteAbility.StopAbility(gameObject);
                    abilityHitPlayer = false;
                }
            }

            // Custom implementation of gravity. This is due to the enemy's linear drag being set to 5 to offset the incredible speed burst from dash attacks.
            // Linear drag massively slows down the acceleration of the enemy while falling, so this counters this slowdown for more realistic falling.
            rb.AddForce(new Vector2(0, -9.81f));
        }
    }

    /// <summary>
    /// Handle all collisions, called when two colliders touch.
    /// </summary>
    /// <param name="col"> The --Other-- collider that touched the collider on this object.</param>
    public void OnCollisionEnter2D(Collision2D col)
    {
        // col: other object
        // col.otherCollider: this object

        // Shield bash hits player
        if ((col.gameObject.tag == "Player" && col.otherCollider.Equals(bashHitBox)) && !abilityHitPlayer)
        {
            // Get the normalized hit direction and set the y value manually to add height and impact to this hit.
            Vector2 normalizedHitDirection = (col.gameObject.transform.position - col.otherCollider.gameObject.transform.position).normalized;
            normalizedHitDirection.y = 0.50f;

            // Push the player back by this normalized hit direction as an impulse force.
            // ANOMALY: Huge horizontal force is also added when the player is directly next to the enemy as he uses this ability. Likely
            // due to collision wackiness from the player being directly inside the ability hitbox. Might fix but is overall low priority
            // due to player being very unlikely to be that close to the enemy (When the ability is used) in actual gameplay.
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(4 * normalizedHitDirection, ForceMode2D.Impulse);

            // Bool that checks if the ability hit is set to true, meaning that the bash will not push the player for the remainder of it's duration.
            abilityHitPlayer = true;
        }
        
        // Enemy itself hits the player
        else if (col.gameObject.tag == "Player")
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
