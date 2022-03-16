using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// Derived ability "Dash", which denotes an attack where the enemy rapidly moves left or right in an attempt to run into the player.
/// </summary>
public sealed class DashAbility : Ability
{
    // Constant that's used to determines the final speed of the dash.
    private int dashForceMultiplier;

    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and dash speed.
    /// </summary>
    public DashAbility()
    {
        damage = 6;
        cooldown = 5.0f;
        dashForceMultiplier = 50;

        // Negative duration = ignore this property, the ability has no duration.
        duration = -1.0f;
    }

    /// <summary>
    /// Makes the enemy use a dash attack.
    /// </summary>
    /// <param name="EliteEnemy">The enemy game object.</param>
    /// <param name="facingRight">Boolean that indicates whether the enemy is facing left or right.</param>
    public override void UseAbility(GameObject EliteEnemy, bool facingRight)
    {
        // If the ability is off-cooldown (cooldown is negative), generate a vector 2 that only uses the x component (positive for facing right, negative
        // for facing left) multiplied by the dash force multiplier. Then apply the vector as an impulse force to the elite enemy and reset the cooldown.
        if (cooldown < 0.0f)
        {
            Vector2 dashImpulse = ((facingRight) ? Vector2.right : Vector2.left) * dashForceMultiplier;
            EliteEnemy.GetComponent<Rigidbody2D>().AddForce(dashImpulse, ForceMode2D.Impulse);
            cooldown = 5.0f;
        }
    }
}

