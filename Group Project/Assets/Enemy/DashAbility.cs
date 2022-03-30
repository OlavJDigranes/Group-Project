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

    // Stores the cooldown value, used to reset the cooldown after the ability was used.
    private float DefaultCooldown;

    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and dash speed. Monster level scales and improves the ability's damage and cooldown.
    /// <param name="monsterLevel">Level of the monster.</param>
    /// </summary>
    public override void Init(int monsterLevel)
    {
        // Damage is increased by the monster level doubled (Base 6 damage, per level increase = +2 damage).
        damage = 4 + (monsterLevel * 2);

        // Cooldown is reduced by monster level halved (Base = 5 seconds, per level decrease = -0.5 seconds).
        cooldown = 5.5f - (monsterLevel / 2);
        DefaultCooldown = cooldown;

        // Velocity of the dash is increased by monster level doubled (Base = 50 velocity multiplier, per level increase = +2 velocity multiplier).
        dashForceMultiplier = 48 + (monsterLevel * 2);

        // No duration
        duration = 0.0f;
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
            cooldown = DefaultCooldown;
        }
    }
}