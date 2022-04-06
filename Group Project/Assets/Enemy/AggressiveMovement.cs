using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AggressiveMovement : Movement
{

    /// <summary>
    /// Get which direction (left or right) to face
    /// </summary>
    /// <param name="eliteEnemy">Enemy whose movement is being calculated</param>
    /// <param name="player">The player</param>
    /// <returns>True for facing right, false for facing left</returns>
    public override bool FaceTowardsPlayer(GameObject eliteEnemy, GameObject player)
    {
        // Get horizontal facing direction and return true if it's positive (facing right) or negative otherwise (facing left).
        float normalizedFacingDirection = (player.transform.position - eliteEnemy.transform.position).normalized.x;
        return normalizedFacingDirection > 0.0;
    }
    
    /// <summary>
    /// Move the enemy in it's facing direction.
    /// </summary>
    /// <param name="eliteEnemy"></param>
    /// <param name="facingRight"></param>
    public override void Move(GameObject eliteEnemy, bool facingRight)
    {
        // Calculate move speed based on enemy's moveSpeed scalar.
        Vector2 movement = new Vector2(moveSpeed, 0.0f) * Time.deltaTime;

        // Set movement direction to the enemy's facing direction.
        movement.x *= ((facingRight) ? 1.0f : -1.0f);

        // Move the enemy by the movement vector..
        eliteEnemy.transform.Translate(movement);
    }
}
