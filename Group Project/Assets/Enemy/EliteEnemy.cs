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
// -> Abilities for enemies (such as projectiles, teleportation, summoning enemies, speed/power enhancements..). // Melee abilities done \\
// -> Ranged enemies

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

    // Hitbox of the bash ability, set to null initially for cases where the enemy has a different ability, otherwise it will be set to the component.
    private BoxCollider2D bashHitBox = null;

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
                EliteAbility = gameObject.AddComponent<BashAbility>();
                EliteMovement = gameObject.AddComponent<AggressiveMovement>();
                break;
        }

        // Call init/constructor functions for movement and ability.
        EliteAbility.Init(monsterLevel);
        EliteMovement.Init(moveSpeed);

        // If the enemy has the bash ability, give it a hitbox to do the bash attack with. Also set it's size to 0 so that it doesn't accidentally hit the player
        // outside of ability use.
        if (EliteAbility.name == "Bash") 
        {
            bashHitBox = gameObject.AddComponent<BoxCollider2D>(); 
            bashHitBox.size = Vector2.zero;
            bashHitBox.name = "Bash";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get player position
        playerPosition = GameObject.Find("Player").transform.position;

        // If asleep, check if player moved close enough to the enemy to activate/wake up the enemy.
        // Enemy will begin to attack the player when the player is around 35 metres away from it.
        // Active check is done first so that when the enemy becomes active, it will always check for the bool first, removing the need of performing
        // an expensive distance check per frame.
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
            // burst of vertical speed. This can be alleviated by having straight, axis-aligned terrain.
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
            if ( EliteAbility.cooldown < 0.0 && EliteAbility.CheckAbilityUsage(transform.position, playerPosition))
            {
                // Access the enemy's ability component and call it's attack function (which runs on a cooldown-based system)
                EliteAbility.UseAbility(gameObject, facingRight);
            }

            // Decrement the ability cooldown by dt.
            EliteAbility.cooldown -= Time.deltaTime;

            // If the elite ability relies on a after-use timer, decrement it by dt.
            // Once this timer hits 0, run another function to finish/despawn the attack.
            if (EliteAbility.hasDuration)
            {
                EliteAbility.duration -= Time.deltaTime;

                if (EliteAbility.duration <= 0.0f)
                {
                    EliteAbility.StopAbility(gameObject);
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

        // Collision with any object labelled "Floor" (Which should be the tag set for all walkable surfaces in the game): allow enemy to jump again if he is walking on the floor
        // ANOMALIES:
        //   - Enemy could "wall climb" by repeatedly running into a wall and jumping. (Can be fixed by also testing with a raycast, but it's a minor (and entertaining) bug.
        //   - Enemy does not lose onGround status when moving off a floating/tall platform, allowing it to jump in midair once.
        if (col.gameObject.tag == "Floor" &&
                 col.gameObject.transform.position.y < gameObject.transform.position.y)
        {
            onGround = true;
        }

        // Shield bash hits player
        else if (bashHitBox.IsTouching(GameObject.Find("Player").gameObject.GetComponent<BoxCollider2D>()))
        {
            Debug.Log("ppog");
            // Get the normalized hit direction and set the y value manually to add height and impact to this hit.
            Vector2 normalizedHitDirection = (col.gameObject.transform.position - col.otherCollider.gameObject.transform.position).normalized;
            normalizedHitDirection.y = 0.66f;

            // Push the player back by this normalized hit direction as an impulse force.
            // ANOMALY: Huge horizontal force is also added when the player is directly next to the enemy as he uses this ability. Likely
            // due to collision wackiness from the player being directly inside the ability hitbox. Might fix but is overall low priority
            // due to player being very unlikely to be that close to the enemy (When the ability is used) in actual gameplay.
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(7 * normalizedHitDirection, ForceMode2D.Impulse);
            EliteAbility.StopAbility(this.gameObject);
        }
        
        // Enemy itself hits the player
        else if (col.gameObject.tag == "Player")
        {
            Vector2 normalizedHitDirection = (col.gameObject.transform.position - col.otherCollider.gameObject.transform.position).normalized;
            normalizedHitDirection.y = 0.4f;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(3 * normalizedHitDirection, ForceMode2D.Impulse);
        }
    }
}
