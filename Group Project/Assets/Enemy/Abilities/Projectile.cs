using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

// Controls movement of summoned projectiles and how they interact with the world.
public class Projectile : MonoBehaviour
{
    void Start()
    {
        // Get player object
        GameObject player = GameObject.Find("Player");

        // Calculate vector to fire the projectile in the direction of the player, normalized.
        Vector2 normalizedPlayerDirection = (player.transform.position - transform.position).normalized;

        // Scale normalized vector and apply it to the projectile as an impulse force.
        GetComponent<Rigidbody2D>().AddForce(normalizedPlayerDirection * 9f, ForceMode2D.Impulse);
    }

    // If projectile hits the world geometry (tagged as floor), destroy it.
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor") { Destroy(gameObject); }
    }

    // OnBecameInvisible is automatically called when the object exits the camera boundaries.
    // Destroy the object if the projectile exits the player view.
    void OnBecameInvisible() { Destroy(gameObject); }
}