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

    // Box collider of the attack.
   private BoxCollider2D BashHitbox;


    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and dash speed.
    /// Also sets the bash force (Force applied when the attack hits the player) and duration (The duration of the hitbox before it despawns).
    /// </summary>
    public BashAbility()
    {
        damage = 6;
        cooldown = 5.0f;
        dashForceMultiplier = 20;
        duration = 999.0f;
    }

    /// <summary>
    /// The enemy uses a shield bash attack
    /// </summary>
    /// <param name="EliteEnemy">The enemy to generate the attack.</param>
    /// <param name="facingRight">The direction the enemy is facing.</param>
    public override void UseAbility(GameObject eliteEnemy, bool facingRight)
    {
        if (cooldown < 0.0f)
        {
            // Get the last box collider which is the bash collider
            BoxCollider2D bashCollider = eliteEnemy.GetComponents<BoxCollider2D>().Last();

            // Push out the hitbox to the direction of travel infront of the enemy, essentially the enemy attacking this location.
            bashCollider.size = new Vector2(0.75f, 0.5f);
            bashCollider.offset = new Vector2((facingRight) ? 0.5f : -0.5f , 0);

            // Set duration to 1.0f, where after one second the hitbox will despawn.
            duration = 1.0f;

            // Generate the impulse force for the mini dash used with this ability, and set the cooldown for the ability after the dash force is computed.
            Vector2 dashImpulse = ((facingRight) ? Vector2.right : Vector2.left) * dashForceMultiplier;
            eliteEnemy.GetComponent<Rigidbody2D>().AddForce(dashImpulse, ForceMode2D.Impulse);
            cooldown = 5.0f;
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
}