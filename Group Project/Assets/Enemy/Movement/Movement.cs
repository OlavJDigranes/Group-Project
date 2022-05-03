using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract movement class
/// </summary>
public abstract class Movement : MonoBehaviour
{
    // Move speed of the enemy
    protected float moveSpeed;

    /// <summary>
    /// Init the enemy's movement speed to be used in the movement function
    /// </summary>
    /// <param name="enemyMoveSpeed">Enemy's movement speed</param>
    public void Init(float enemyMoveSpeed)
    {
        moveSpeed = enemyMoveSpeed;
    }

    /// <summary>
    /// Change enemy's direction so that it faces the player, used to move the enemy in the correct direction.
    /// </summary>
    /// <param name="eliteEnemy">Enemy object position.</param>
    /// <param name="player">Player object position.</param>
    /// <returns>Bool the direction of the player (true being facing right, false being facing left)</returns>
    public virtual bool FaceTowardsPlayer(Vector2 eliteEnemyPosition, Vector2 playerPosition)  { return false; }

    /// <summary>
    /// Move enemy in relation to it's facing direction.
    /// </summary>
    /// <param name="eliteEnemy">Enemy object.</param>
    /// <param name="facingRight">Direction the enemy is facing.</param>
    public virtual void Move(GameObject eliteEnemy, bool facingRight)  {}

    /// <summary>
    /// Give elite enemy vertical upwards movement, or make him jump. Uses an impulse instead of translation.
    /// If used in timing with an ability that gives the enemy a huge burst of speed (Such as a dash), the enemy will gain a massive burst of diagonal speed. Happens
    /// Very rarely though due to both events needing to trigger at nearly the same time.
    /// </summary>
    /// <param name="eliteEnemy">Enemy object.</param>
    public void Jump(GameObject eliteEnemy)  { eliteEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, moveSpeed * 9), ForceMode2D.Impulse); }

}