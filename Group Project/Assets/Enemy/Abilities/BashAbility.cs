using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

/// <summary>
/// Derived ability "Shield Bash", where the enemy will attempt to push the player back with a quick, short ranged melee attack.
/// </summary>
public sealed class BashAbility : Ability
{
    // Constant that's used to determines the final speed of the mini-dash used during the attack.
    private int dashForceMultiplier;

    // Stores the cooldown value, used to reset the cooldown after the ability was used.
    private float DefaultCooldown;



    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and dash speed.
    /// Also sets the bash force (Force applied when the attack hits the player) and duration (The duration of the hitbox before it despawns).
    /// </summary>
    /// <param name="monsterLevel">Level of the monster using this ability</param>
    public override void Init(int monsterLevel)
    {
        name = "Bash";

        // Damage increased by monstter level doubled (Base = 4 damage, increase = +2 damage).
        damage = 2 * monsterLevel;

        // Cooldown is reduced by monster level halved (Base = 5 seconds, per level decrease = -0.5 seconds).
        cooldown = 5.5f - (monsterLevel / 2);
        DefaultCooldown = cooldown;

        // Velocity of the dash does not get scaled
        dashForceMultiplier = 15;

        // Set duration to dummy value to prevent it triggering (Hitting 0.0) outside the function that activates the ability.
        duration = 999.0f;

        hasDuration = true;
    }

    /// <summary>
    /// The enemy uses a shield bash attack.
    /// <param name="EliteEnemy">The enemy to generate the attack.</param>
    /// <param name="facingRight">The direction the enemy is facing.</param>
    /// </summary>
    public override void UseAbility(GameObject eliteEnemy, bool facingRight)
    {
        if (cooldown < 0.0f)
        {
            // Get the last box collider which is the bash collider
            BoxCollider2D bashCollider = eliteEnemy.GetComponents<BoxCollider2D>().Last();

            // Push out the hitbox to the direction of travel infront of the enemy, essentially the enemy attacking in that direction.
            bashCollider.size = new Vector2(0.75f, 0.5f);
            bashCollider.offset = new Vector2((facingRight) ? 0.5f : -0.5f , 0);

            // Set duration to 1.0f, where after one second the hitbox will be shrunk and unusable again.
            duration = 1.0f;

            // Generate the impulse force for the mini dash used with this ability.
            Vector2 dashImpulse = ((facingRight) ? Vector2.right : Vector2.left) * dashForceMultiplier;
            eliteEnemy.GetComponent<Rigidbody2D>().AddForce(dashImpulse, ForceMode2D.Impulse);

            // Once ability is used, set cooldown to the default value determined in the Init function + duration (so that cooldown doesn't tick down while the ability
            // is running, or when duration > 0).
            cooldown = DefaultCooldown + duration;
        }
    }

    /// <summary>
    /// Stop the shield bash by removing the new hitbox (After a set time delay)
    /// </summary>
    /// <param name="eliteEnemy">The enemy object who has this ability</param>
    public override void StopAbility(GameObject eliteEnemy)
    {
        // Get the last box collider which is the bash collider
        BoxCollider2D bashCollider = eliteEnemy.GetComponents<BoxCollider2D>().Last();

        // The ability has ended so shrink the hitbox and hide it inside the enemy hitbox (Instead of creating and destroying hitbox components per ability usage).
        bashCollider.offset = new Vector2(0, 0);
        bashCollider.size = new Vector2(0.1f, 0.1f);

        // Reset duration back to the dummy value 999 so that this ability won't get accidentally triggered before the next ability execution.
        duration = 999.0f;
    }


    /// <summary>
    /// Check if the bash ability can be used.
    /// </summary>
    /// <param name="eliteEnemyPosition">Position of the elite enemy. </param>
    /// <param name="playerPosition">Position of the player. </param>
    /// <returns>bool True = Ability will be used in this frame, False = Ability cannot/will not be used this frame.</returns>
    public override bool CheckAbilityUsage(Vector2 eliteEnemyPosition, Vector2 playerPosition)
    {
        // Get horizontal and vertical distances from the player to the enemy.
        float horizontalDistance = Mathf.Abs(eliteEnemyPosition.x - playerPosition.x);
        float verticalDistance = Mathf.Abs(eliteEnemyPosition.y - playerPosition.y);

        // Too close to perform a bash attack. Also minimizes the chances of generating a physics glitch where the player will be thrown
        // incredibly far from the attack.
        if (horizontalDistance < 1.0f) { return false; }

        // If the player is 3 units or closer away from the elite enemy and more or less aligned on the y-axis, return true.
        return horizontalDistance < 3.0f && verticalDistance < 0.5f;
    }
}