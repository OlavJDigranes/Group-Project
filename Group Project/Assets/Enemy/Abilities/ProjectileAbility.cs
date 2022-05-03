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

    // The physical projectile object.
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
        cooldown = 10.5f - (monsterLevel * 0.5f);
        DefaultCooldown = cooldown;

        duration = 0.0f;
        
        // Loads projectile prefab from "Enemy/Resources/Projectile.prefab"
        Projectile = Resources.Load<GameObject>("Projectile");

        // Projectiles have infinite duration, though each projectile handles whether it should destroy itself if it collides with geometry
        // or exits the player view.
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
            GameObject aProjectile = Instantiate(Projectile, 
                (eliteEnemy.transform.position) + (facingRight ? Vector3.right : Vector3.left),
                        Quaternion.identity);

            // Set it's position to the enemy's position.
            aProjectile.transform.position = eliteEnemy.transform.position;

            // Move projectile either 1 unit to the left or right depending on facing direction for realism.
            aProjectile.transform.Translate(facingRight ? Vector2.right : Vector2.left);

            // Reset cooldown.
            cooldown = DefaultCooldown;
        }
    }


    /// <summary>
    /// Check if the bash ability can be used.
    /// </summary>
    /// <param name="eliteEnemyPosition">Position of the elite enemy. </param>
    /// <param name="playerPosition">Position of the player. </param>
    /// <returns>bool True = Ability will be used in this frame, False = Ability cannot/will not be used this frame.</returns>
    public override bool CheckAbilityUsage(Vector2 eliteEnemyPosition, Vector2 playerPosition)
    {
        // Get horizontal distance between enemy and player.
        float horizontalDistance = Mathf.Abs(eliteEnemyPosition.x - playerPosition.x);

        // If player is too close (horizontally), don't fire. Otherwise, fire.
        return !(horizontalDistance < 5f);
    }
}