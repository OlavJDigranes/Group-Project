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
    public void Init(int enemyMoveSpeed)
    {
        moveSpeed = enemyMoveSpeed;
    }

    /// <summary>
    /// Change enemy's direction so that it faces the player, used to move the enemy in the correct direction.
    /// </summary>
    /// <param name="eliteEnemy">Enemy object</param>
    /// <param name="player">Player object</param>
    /// <returns>Bool the direction of the player (true being facing right, false being facing left)</returns>
    public virtual bool FaceTowardsPlayer(GameObject eliteEnemy, GameObject player)  { return false; }

    /// <summary>
    /// Move enemy in relation to it's facing direction.
    /// </summary>
    /// <param name="eliteEnemy">Enemy object.</param>
    /// <param name="facingRight">Direction the enemy is facing.</param>
    public virtual void Move(GameObject eliteEnemy, bool facingRight)  {}

}