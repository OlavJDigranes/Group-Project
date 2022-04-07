using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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
    [SerializeField]
    private int monsterLevel;

    // Attack type, which dictates how the enemy attacks the player (Melee - 0; Attack with melee attacks of any kind, Ranged - 1; Attack from afar with projectiles ).
    [SerializeField]
    private int AttackType;

    // AI type, which dictates how the enemy moves in relation to the player (Aggressive - 0; charge for the player, Defensive - 1; keep their distance).
    [SerializeField]
    private int AIType;

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

    // Boolean to determine which way the enemy is facing.
    private bool facingRight = false;

    // Determines if enemy is touching the floor, used to determine if it is able to jump.
    private bool onGround;

    // Boolean that determines if the ability relies on a timer to function, used to, for example remove new hitboxes or to finish an attack.
    private bool timedAbilty = false;

    // Hitbox of the bash ability, set to null initially for cases where the enemy has a different ability, otherwise it will be set to the component.
    private BoxCollider2D bashHitBox = null;

    // Boolean that determines if an ability hit the player, used for certain abilities to ensure that it only interacts with the player
    // once per ability usage (to prevent multiple hits at once).
    private bool abilityHitPlayer = false;

    // Keep track of the position of the player. May be integrated into an enemy manager class for efficiency later on, but this'll do.
    private Vector2 playerPosition;


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

        // Determine the ability the enemy should use based on it's attack type and AI type.
        // Uses a nested switch statement to handle this.
        switch (AIType)
        {
            // Aggressive Enemy
            case 0:
                switch (AttackType)
                {
                    // Melee aggressive enemy
                    case 0:
                        EliteAbility = gameObject.AddComponent<DashAbility>();
                        EliteAbility.Init(monsterLevel);
                        break;

                    // Ranged defensive enemy
                    case 1:
                        // Some ability...
                        break;

                    default:
                        Debug.Log("Error: invalid AI type value was assigned for this enemy.");
                        break;
                }
                EliteMovement = gameObject.AddComponent<AggressiveMovement>();
                EliteMovement.Init(moveSpeed);
                break;

            // Defensive Enemy
            case 1:
                switch (AttackType)
                {
                    // Melee defensive enemy
                    case 0:
                        EliteAbility = gameObject.AddComponent<BashAbility>();
                        EliteAbility.Init(monsterLevel);
                        bashHitBox = gameObject.AddComponent<BoxCollider2D>();
                        timedAbilty = true;
                        EliteAbility.duration = 999.0f;
                        break;

                    // Ranged defensive enemy
                    case 1:
                        // Another ability...
                        break;

                    default:
                        Debug.Log("Error: Invalid attack type value was assigned for this enemy.");
                        break;
                }
                // Attach defensive movement component script.
                break;
        }
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
        if (Mathf.Abs(transform.position.x - playerPosition.x) <= 5 && onGround && transform.position.y < playerPosition.y - 2)
        {
            EliteMovement.Jump(gameObject);
            onGround = false;
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

        else if (col.gameObject.tag == "Floor" && col.gameObject.transform.position.y < gameObject.transform.position.y)
        {
            onGround = true;
        }
    }
}
