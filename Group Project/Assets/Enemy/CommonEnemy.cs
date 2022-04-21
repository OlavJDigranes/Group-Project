using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using UnityEngine;

// todo Checklist:
// -> Animated sprites (Walking, Hurt, Dead) : Actual sprites unavailable for now, can be implemented once sprite is acquired.
// -> Handling of other player attacks (Secondary, Ultimate) : Other attacks not implemented/designed yet, once developed come back to this.
// -> After-death effects (Increment global kill count (if implemented), award player exp and gold)
// -> Sounds (Walking, on player hit, on incoming damage, on death, possible idle chatter/noises) : Will be implemented as the sound files for them are developed.
// -> (MoSCoW Could Have) Scaling health/damage stats (On player upgrade count, level..) 


/// <summary>
/// Common enemy script. Attach to common enemy prefabs.
/// REQUIRED COMPONENTS FOR COMMON ENEMIES: Collider2D, RigidBody2D.
/// --------------------
/// Note: The player MUST have the tag "Player".
/// Additionally, the player's weapon object must have the tag "PlayerAttack"
/// </summary>
public class CommonEnemy : MonoBehaviour
{
    // Monster level, used to scale the monster's damage and health.
    [SerializeField]
    private int monsterLevel;

    // Monster drops
    private int expOnDeath;
    private int goldOnDeath;

    // Generic monster stats: damage health and movement speed.
    private int contactDamage;
    private int health;
    private int moveSpeed;

    // Bool that determines what way the monster is facing, managed by the monster.
    private bool facingRight = false;

    // Collision boundaries of the monster.
    private Vector2 boundBox;

    // Timer float that regulates how often a platform collision check is done for efficiency.
    // (Does a check approximately every 0.25 seconds instead of per frame).
    private float platformCheckTimer = 0.0f;

    // Rigidbody component
    private Rigidbody2D rb;


    /// <summary>
    /// Runs first and once on scene start.
    /// </summary>
    public void Start()
    {
        // Bounding box dimensions are initialized. Note that the x component is halved due to .size returning
        // the total width and length of the collision box, while we only want to find the distance between the centre and edge (half).
        boundBox = GetComponent<BoxCollider2D>().bounds.size;
        boundBox.x *= 0.5f;

        // Get rigidbody component
        rb = GetComponent<Rigidbody2D>();

        // Setup monster movement speed and drops.
        // Monster level might be determined later based on current level/player level? Hardcoded stats is also an option depending on game length.
        contactDamage = monsterLevel * 5;
        health = monsterLevel * 10;

        expOnDeath = monsterLevel * 50;
        goldOnDeath = monsterLevel * 10;

        moveSpeed = 5;
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
    /// <br></br><br></br>
    /// Note: All floor tiles must have a 2D rigidbody and a 2D collider otherwise the enemy will fall through the floor or be unable to check for
    /// collision with it's raycasts.
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
}
