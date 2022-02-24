using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Common enemy script. Attach to common enemy prefabs.
/// REQUIRED COMPONENTS FOR COMMON ENEMIES: Collider2D, RigidBody2D.
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

    // Timer float that regulates how often a platform collision check is done for efficiency.
    // (Does a check approximately every 0.25 seconds instead of per frame).
    private float platformCheckTimer = 0.0f;

    /// <summary>
    /// Runs first and once on scene start.
    /// </summary>
    public void Start()
    {
        // Bounding box dimensions are initialized. Note that the x component is halved due to .size returning
        // the total width and length of the collision box, while we only want to find the distance between the centre and edge (half).
        boundBox = GetComponent<BoxCollider2D>().bounds.size;
        boundBox.x *= 0.5f;
    }

    /// <summary>
    /// Runs once per frame after start.
    /// </summary>
    public void Update()
    {
        // Update the platform check timer by dt.
        platformCheckTimer = -Time.deltaTime;
        // Check if the monster reached the platform edge and resolve, if the timer expired.
        if(platformCheckTimer < 0.0f) { CheckForPlatformEdge(); }

        // Init movement offset vector with pre-defined movement speed value.
        Vector2 movement = new Vector2(moveSpeed, 0.0f) * Time.deltaTime;

        // Check the direction the enemy is facing, if positive leave it alone, if negative multiply by -1, multiply by dt.
        movement.x *= ((facingRight) ? 1.0f : -1.0f);

        // Move the monster by the calculated offset vector.
        transform.Translate(movement);

    }

    /// <summary>
    /// Check if the monster moved off a platform (by using a short raycast from the edge of it's collision box), resolve if it is off it's platform.
    /// Runs on a timer so that it performs a raycast and collision check 4 times per second, instead of performing it every frame, improving efficiency.
    /// </summary>
    private void CheckForPlatformEdge()
    {
        // Reset timer
        platformCheckTimer = 0.25f;

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

    /// <summary>
    /// Called automatically when the enemy registers a collision with another object with a rigidbody2D component.
    /// Because of how an enemy-player collision is detected, the player MUST use the "Player" tag.
    /// </summary>
    /// <param name="other"> The Collision2D object, which can be used to retrieve the other gameObject that collided with the enemy.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Player.health -= contactDamage;

            // -- Call player function to check player's health to see if the inflicted damage is fatal.

            // Apply a pushback effect (impulse) to the player to prevent multiple instant collisions, to signify that
            // the player was damaged, etc: "PlayerRB2D.AddForce(new Vector2(-contactNormal * forceMultiplier), ForceMode.Impulse);"
        }
    }
}
