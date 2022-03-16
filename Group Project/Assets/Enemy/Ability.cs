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

    // Duration of the ability. (-1.0f means the ability's duration does not need to be measured).
    public float duration;

    /// <summary>
    /// Virtual method that makes the enemy use it's ability.
    /// </summary>
    /// <param name="eliteEnemy">The enemy game object (It is automatically passed as a reference so all changes to this game object will affect the original).</param>
    /// <param name="facingRight">Boolean that determines the enemy's facing direction (used to determine, for example, which way to dash or aim).</param>
    public virtual void UseAbility(GameObject eliteEnemy, bool facingRight) { Debug.Log("Error: An elite enemy is not using a derived ability!"); }

    /// <summary>
    /// Virtual method that ends the effects of an ability that has a use time
    /// </summary>
    /// <param name="eliteEnemy"> The elite enemy object whose ability is being ended. </param>
    public virtual void StopAbility(GameObject eliteEnemy) {Debug.Log("Error: Elite enemy attempted to stop an ability with no use time!"); }
}
