using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DashAbility : MonoBehaviour
{
    protected new string name;
    protected int damage;
    public float cooldown;
    protected bool projectile;
    private int dashForceMultiplier;

    public DashAbility()
    {
        name = "Dash";
        damage = 6;
        cooldown = 5.0f;
        projectile = false;
        dashForceMultiplier = 50;
    }

    public void dashAttack(GameObject EliteEnemy, bool facingRight)
    {
        if (cooldown < 0.0f)
        {
            Vector2 dashImpulse = ((facingRight) ? Vector2.right : Vector2.left) * dashForceMultiplier;
            EliteEnemy.GetComponent<Rigidbody2D>().AddForce(dashImpulse, ForceMode2D.Impulse);
            cooldown = 5.0f;
        }
    }
}

