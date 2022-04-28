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
/// Dervied ability projectile ability, which makes the enemy launch a projectile at the player.
/// </summary>
public sealed class ProjectileAbility : Ability
{
    // Constant that's used to determines the final speed of the mini-dash used during the attack.
    private float projectileSpeed;

    private GameObject Projectile;
    // Stores the cooldown value, used to reset the cooldown after the ability was used.
    private float DefaultCooldown;


    /// <summary>
    /// Constructor that automatically sets the damage, cooldown and projectile game object.
    /// </summary>
    /// <param name="monsterLevel">Level of the monster using this ability</param>
    public override void Init(int monsterLevel)
    {
        name = "Projectile";

        // Damage increased by monstter level doubled (Base = 2 damage, increase = +1 damage).
        damage = 2 + monsterLevel;

        // Cooldown is reduced by monster level halved (Base = 10 seconds, per level decrease = -0.5 seconds).
        cooldown = 10.5f - (monsterLevel / 2);
        DefaultCooldown = cooldown;

        duration = 0.0f;

        hasDuration = false;
    }

    /// <summary>
    /// The enemy throws a projectile
    /// <param name="EliteEnemy">The enemy to generate the attack.</param>
    /// <param name="facingRight">The direction the enemy is facing.</param>
    /// </summary>
    public override void UseAbility(GameObject eliteEnemy, bool facingRight)
    {
        if (cooldown < 0.0f)
        {
            // Create projectile using the stored template.
            GameObject aProjectile = Projectile;

            // Set it's position to the enemy's position.
            aProjectile.transform.position = eliteEnemy.transform.position;

            // Move projectile either 1 unit to the left or right depending on facing direction for realism.
            aProjectile.transform.Translate(facingRight ? Vector2.right : Vector2.left);

            // Give the projectile the projectile component, which handles projectile movement and collision handling.
            aProjectile.AddComponent<Projectile>();
            // Also add necessary components for collision detection (2D collider) and downward force due to gravity (RigidBody 2D)
            aProjectile.AddComponent<BoxCollider2D>();
            aProjectile.AddComponent<Rigidbody2D>();
            //todo: ^^ actually, do this in awake() call of projectile

            // Reset cooldown.
            cooldown = DefaultCooldown;
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