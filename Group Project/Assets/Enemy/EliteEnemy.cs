using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // Modifiable elite enemy health, damage and speed.
    // Modifiable elite enemy AI (Aggressive: 0 or defensive: 1) and attack type (Melee: 0 or ranged: 1).
    [SerializeField]
    public int AttackType;
    public int AIType;
    public int Health;
    public int Damage;
    public int Speed;

    // The enemy's ability. It is set initially as the ability abstract but will be derived to a specific ability in the start function.
    private Ability EliteAbility;

    // Boolean to determine which way the enemy is facing.
    private bool facingRight;

    // Boolean that determines if the ability relies on a timer to function, used to, for example remove new hitboxes or to finish an attack.
    private bool timedAbilty = false;

    // Hitbox of the bash ability, set to null initially for cases where the enemy has a different ability, otherwise it will be set to the component.
    private BoxCollider2D bashHitBox = null;

    // Boolean that determines if an ability hit the player, used for certain abilities to ensure that it only interacts with the player
    // once per ability usage (to prevent multiple hits at once).
    private bool abilityHitPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        // Determine the ability the enemy should use based on it's attack type and AI type.
        // Uses a nested switch statement to handle this.
        switch (AttackType)
        {
            // Melee
            case 0:
                switch (AIType)
                {
                    // Melee aggressive enemy
                    case 0:
                        EliteAbility = gameObject.AddComponent<DashAbility>();
                        break;

                    // Melee defensive enemy
                    case 1:
                        EliteAbility = gameObject.AddComponent<BashAbility>();
                        bashHitBox = gameObject.AddComponent<BoxCollider2D>();
                        timedAbilty = true;
                        EliteAbility.duration = 999.0f;
                        break;

                    default:
                        Debug.Log("Error: invalid attack type value was assigned for this enemy.");
                        break;
                }
                break;

            // Ranged
            case 1:
                switch (AIType)
                {
                    // Ranged aggressive enemy
                    case 0:
                        // Some ability...
                        break;

                    case 1:
                        // Another ability...
                        break;

                    default:
                        Debug.Log("Error: Invalid AI type value was assigned for this enemy.");
                        break;
                }
                break;
        }
        // Init the facing direction of the enemy.
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Access the enemy's ability component and call it's attack function (which runs on a cooldown-based system)
        EliteAbility.UseAbility(gameObject, facingRight);
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
    }
}
