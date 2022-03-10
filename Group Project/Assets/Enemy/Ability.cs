using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract ability class.
/// </summary>
public abstract class Ability : MonoBehaviour
{
    // Damage inflicted by this ability.
    protected int damage;
    // Cooldown timer thats triggered on ability use.
    public float cooldown;

    /// <summary>
    /// Virtual method that makes the enemy use it's ability.
    /// </summary>
    /// <param name="eliteEnemy">The enemy game object (It is automatically passed as a reference so all changes to this game object will affect the original).</param>
    /// <param name="facingRight">Boolean that determines the enemy's facing direction (used to determine, for example, which way to dash or aim).</param>
    public virtual void UseAbility(GameObject eliteEnemy, bool facingRight) {}
}
