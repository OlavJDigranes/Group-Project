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
        float normalizedFacingDirection = (player.transform.position - eliteEnemy.transform.position).normalized.x;
        return normalizedFacingDirection > 0.0;
    }

    public override void Move(GameObject eliteEnemy, bool facingRight)
    {
        // Init movement offset vector with pre-defined movement speed value.
        Vector2 movement = new Vector2(moveSpeed, 0.0f) * Time.deltaTime;

        // Check the direction the enemy is facing, if positive leave it alone, if negative multiply by -1, multiply by dt.
        movement.x *= ((facingRight) ? 1.0f : -1.0f);

        // Move the monster by the calculated offset vector.
        eliteEnemy.GetComponent<Rigidbody2D>().AddForce(movement, ForceMode2D.Impulse);
    }
}
