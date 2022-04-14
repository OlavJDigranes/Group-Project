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

        // Velocity of the dash is increased by monster level doubled (Base = 40 velocity multiplier, per level increase = +2 velocity multiplier).
        dashForceMultiplier = 38 + (monsterLevel * 2);

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

    /// <summary>
    /// Check if the dash ability can be used.
    /// </summary>
    /// <param name="eliteEnemyPosition">Position of the elite enemy. </param>
    /// <param name="playerPosition">Position of the player. </param>
    /// <param name="cooldown">Current cooldown of the ability. </param>
    /// <returns></returns>
    public override bool CheckAbilityUsage(Vector2 eliteEnemyPosition, Vector2 playerPosition, float cooldown)
    {
        // Ability can't be used if the cooldown has not expired yet.
        if (cooldown > 0.0f) { return false; }

        // Get horizontal and vertical distances from the player to the enemy.
        float horizontalDistance = Mathf.Abs(eliteEnemyPosition.x - playerPosition.x);
        float verticalDistance = Mathf.Abs(eliteEnemyPosition.y - playerPosition.y);

        // Player too close to do a good dash attack.
        if (horizontalDistance < 2.0f) { return false; }

        // Return true if the enemy is between 2 and 15 units away from the player horizontally and almost lined up vertically (Dash attack can be used) or false otherwise.
        return horizontalDistance < 15.0f && verticalDistance < 1.5f;
    }
}