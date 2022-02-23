using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Common enemy script. Attach to common enemy prefabs.
/// LIMITATIONS: The platform the enemy travels on MUST have a collision box (Any Collider2D component).
/// </summary>
public class CommonEnemy : MonoBehaviour
{
    // Customizable monster health, movement speed multiplier and contact damage values.
    [SerializeField]
    public int health;
    public float moveSpeed;
    public int contactDamage;

    // Bool that determines what way the monster is facing, managed by the monster.
    private bool facingRight = false;

    // Collision boundaries of the monster.
    private Vector2 boundBox;

    /// <summary>
    /// Runs first and once on scene start.
    /// </summary>
    public void Start()
    {
        // Bounding box dimensions are initialized. Note that the x component is halved due to .size returning
        // the entire x length of the monster collision box, not half (from centre -> edge).
        boundBox = GetComponent<BoxCollider2D>().bounds.size;
        boundBox.x *= 0.5f;
    }

    /// <summary>
    /// Runs once per frame after start.
    /// </summary>
    public void Update()
    {

        // Check if the monster reached the platform edge and resolve.
        CheckForPlatformEdge();

        // Init movement offset vector with pre-defined movement speed value.
        Vector2 movement = new Vector2(moveSpeed, 0.0f) * Time.deltaTime;

        // Check the direction the enemy is facing, if positive leave it alone, if negative multiply by -1, multiply by dt.
        movement.x *= ((facingRight) ? 1.0f : -1.0f);

        // Move the monster by the calculated offset vector.
        transform.Translate(movement);
    }

    /// <summary>
    /// Check if the monster moved off a platform (by using a short raycast from the edge of it's collision box), resolve if it is off it's platform.
    /// </summary>
    private void CheckForPlatformEdge()
    {
        // Generate a raycast from the bottom corner of the monster's collision box (For x value, this is also the corner that the monster is moving/facing).
        // to a position 0.05 units down from it.
        RaycastHit2D hitCheck = Physics2D.Raycast((Vector2) transform.position - boundBox, Vector2.down, 0.05f);

        // If the raycast does not detect any collisions, then the monster is about to move off the platform. So, reverse it's direction and
        // swap the raycast x position from one side of the collision box to the other.
        if (hitCheck.collider == null)
        {
            facingRight = !facingRight;
            boundBox.x *= -1.0f;
        }

    }
}
