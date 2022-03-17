using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

/// <summary>
/// Derived ability "Shield Bash", where the enemy will attempt to push the player back with a quick, short ranged melee attack.
/// </summary>
public sealed class ShieldBashAbility : Ability
{
    // Constant that's used to determines the final speed of the mini-dash used during the attack.
    private int dashForceMultiplier;

    // Constant used to determine the impulse force when the player is hit with the shield bash.
   private int bashForce;

   // Box collider of the attack.
   private BoxCollider2D shieldBashHitbox;


    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and dash speed.
    /// Also sets the bash force (Force applied when the attack hits the player) and duration (The duration of the hitbox before it despawns).
    /// </summary>
    public ShieldBashAbility()
    {
        damage = 6;
        cooldown = 5.0f;
        dashForceMultiplier = 10;
        bashForce = 60;
        duration = 999.0f;
    }

    /// <summary>
    /// The enemy uses a shield bash attack
    /// </summary>
    /// <param name="EliteEnemy">The enemy to generate the attack.</param>
    /// <param name="facingRight">The direction the enemy is facing.</param>
    public override void UseAbility(GameObject EliteEnemy, bool facingRight)
    {
        if (cooldown < 0.0f)
        {
            // Create the hitbox for the shield bash.
            shieldBashHitbox = EliteEnemy.AddComponent<BoxCollider2D>();
            shieldBashHitbox.size = new Vector2(1, 0.5f);
            shieldBashHitbox.offset = new Vector2((facingRight) ? 0.5f : -0.5f, 0);

            // Set duration to 1.0f, where after one second the hitbox will despawn.
            duration = 1.0f;

            // Generate the impulse force for the mini dash used with this ability, and set the cooldown for the ability after the dash force is computed.
            Vector2 dashImpulse = ((facingRight) ? Vector2.right : Vector2.left) * dashForceMultiplier;
            EliteEnemy.GetComponent<Rigidbody2D>().AddForce(dashImpulse, ForceMode2D.Impulse);
            cooldown = 5.0f;
        }
    }

    /// <summary>
    /// Stop the shield bash by removing the new hitbox (After a set time delay)
    /// </summary>
    /// <param name="eliteEnemy">The enemy object who has this ability</param>
    public override void StopAbility(GameObject eliteEnemy)
    {
        // Get the list of box colliders on this enemy object.
        BoxCollider2D[] Colliders = eliteEnemy.GetComponents<BoxCollider2D>();

        // Because getComponents adds components in order of oldest to newest, destroy the last element in the list of colliders, which will be the shield bash hitbox.
        Destroy(Colliders[Colliders.Length - 1]);

        // Reset duration back to the dummy value 999 so that this ability won't get accidentally triggered before the next ability execution.
        duration = 999.0f;
    }
}